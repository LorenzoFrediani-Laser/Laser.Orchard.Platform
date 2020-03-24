﻿using Laser.Orchard.NwazetIntegration.Models;
using Laser.Orchard.NwazetIntegration.Services;
using Laser.Orchard.NwazetIntegration.ViewModels;
using Laser.Orchard.PaymentGateway.Controllers;
using Laser.Orchard.PaymentGateway.ViewModels;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Mvc;
using Orchard.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Laser.Orchard.NwazetIntegration.Filters {
    public class InfoResultPaymentFilter : FilterProvider, IActionFilter {
        private readonly IContentManager _contentManager;
        private readonly IGTMProductService _GTMProductService;
        private readonly dynamic _shapeFactory;
        private readonly IWorkContextAccessor _workContextAccessor;

        public InfoResultPaymentFilter(
            IContentManager contentManager,
            IGTMProductService GTMProductService,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor) {
            _contentManager = contentManager;
            _GTMProductService = GTMProductService;
            _shapeFactory = shapeFactory;
            _workContextAccessor = workContextAccessor;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext) {
            if (filterContext.Controller is PaymentController
                && filterContext.ActionDescriptor.ActionName.Equals("Info")) {
                var viewResult = filterContext.Result as ViewResult;
                if (viewResult != null) {
                    var model = viewResult.Model as PaymentVM;
                    if (model != null 
                        && model.Record != null 
                        && model.Record.Success) {
                        
                        var productList = new List<GTMProductVM>();
                        #region add item to productList
                        // select the contentitemid which is the id of the order
                        var orderId = model.Record.ContentItemId;
                        // select order
                        var order = _contentManager.Get<OrderPart>(orderId);
                        var checkoutItems = order.Items.ToList();
                        var products = _contentManager
                           .GetMany<IContent>(
                               checkoutItems.Select(p => p.ProductId).Distinct(),
                               VersionOptions.Latest,
                               QueryHints.Empty)
                            .ToList();
                        foreach (var p in products) {
                            // populate list of GTMProductVM 
                            var part = p.As<GTMProductPart>();
                            _GTMProductService.FillPart(part);
                            var vm = new GTMProductVM(part);
                            var checkoutItem = checkoutItems
                                .Where(c => c.ProductId == p.Id)
                                .FirstOrDefault();
                            // add quantity
                            vm.Quantity = checkoutItem == null ? 0 : checkoutItem.Quantity;
                            productList.Add(vm);
                        }
                        #endregion 
                        // populate ViewModel to send at shape
                        var purchaseVM = new GTMPurchaseVM();
                        purchaseVM.ActionField = new GTMActionField {
                            Id = order.OrderKey,
                            Revenue = model.Record.Amount
                        };
                        purchaseVM.ProductList = productList;

                        _workContextAccessor.GetContext(filterContext)
                            .Layout.Zones.Head
                            .Add(_shapeFactory.GTMPurchase(GTMPurchaseVM: purchaseVM));
                    }
                }

            }
        }
        public void OnActionExecuting(ActionExecutingContext filterContext) {
        }
    }
}