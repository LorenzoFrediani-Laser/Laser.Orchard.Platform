using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Laser.Orchard.Braintree.ViewModels;

namespace Laser.Orchard.Braintree.Services {
    public class BasicBraintreePaymentInformationProvider
        : IBraintreePaymentInformationProvider {
        public PaymentVM AddInformation(PaymentVM model) {
            if (model != null && model.Record!= null) {
                model.ThreeDSecure.Amount = model.Record.Amount;
            }
            return model;
        }
    }
}