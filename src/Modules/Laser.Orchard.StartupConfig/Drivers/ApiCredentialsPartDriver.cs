﻿using Laser.Orchard.StartupConfig.Models;
using Laser.Orchard.StartupConfig.Security;
using Laser.Orchard.StartupConfig.Services;
using Laser.Orchard.StartupConfig.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laser.Orchard.StartupConfig.Drivers {
    [OrchardFeature("Laser.Orchard.BearerTokenAuthentication")]
    public class ApiCredentialsPartDriver : ContentPartDriver<ApiCredentialsPart> {
        private readonly IApiCredentialsManagementService _apiCredentialsMembershipService;
        private readonly IAuthorizer _authorizer;

        public ApiCredentialsPartDriver(
            IApiCredentialsManagementService apiCredentialsMembershipService,
            IAuthorizer authorizer) {

            _apiCredentialsMembershipService = apiCredentialsMembershipService;
            _authorizer = authorizer;

            T = NullLocalizer.Instance;
        }

        protected override string Prefix {
            get { return "ApiCredentialsPart"; }
        }
        public Localizer T { get; set; }

        protected override DriverResult Editor(ApiCredentialsPart part, dynamic shapeHelper) {
            // this is the driver in GET. There really is nothing to do to the credentials 
            // when saving the user. They are generated by a separate action than the management
            // of the user they belong to, to avoid confusing situations where editing
            // fails and we end up with weird in between conditions.
            var shapes = new List<DriverResult>();

            if (!AuthorizeEdit(part)) {
                shapes.Add(ContentShape("Parts_ApiCredentialsPart_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: "Parts/ApiCredentialsPartUnauthorized",
                        Prefix: Prefix)));
            } else {
                // only show the credentials in existing Users (i.e. not while creating)
                if (part.Id == 0) {
                    shapes.Add(ContentShape("Parts_ApiCredentialsPart_Edit",
                        () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts/ApiCredentialsPartNew",
                            Prefix: Prefix)));
                } else {
                    shapes.Add(ContentShape("Parts_ApiCredentialsPart_Edit",
                        () => shapeHelper.EditorTemplate(
                            TemplateName: "Parts/ApiCredentialsPart",
                            Model: new ApiCredentialsPartViewModel {
                                Key = part.Key,
                                Secret = part.Secret
                            },
                            Key: part.Key,
                            Secret: _apiCredentialsMembershipService.GetSecret(part),
                            Prefix: Prefix)));
                }
            }


            return Combined(shapes.ToArray());
        }

        protected override DriverResult Editor(ApiCredentialsPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }

        private bool AuthorizeEdit(ApiCredentialsPart part) {
            return _authorizer.Authorize(ApiCredentialsPermissions.ManageApiCredentials);
        }
        

        protected override void Importing(ApiCredentialsPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            part.Key = context.Attribute(part.PartDefinition.Name, "Key");
            part.Secret = context.Attribute(part.PartDefinition.Name, "Secret");
            //part.HashAlgorithm = context.Attribute(part.PartDefinition.Name, "HashAlgorithm");
            //part.SecretSalt = context.Attribute(part.PartDefinition.Name, "SecretSalt");
        }

        protected override void Exporting(ApiCredentialsPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Key", part.Key);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Secret", part.Secret);
            //context.Element(part.PartDefinition.Name).SetAttributeValue("HashAlgorithm", part.HashAlgorithm);
            //context.Element(part.PartDefinition.Name).SetAttributeValue("SecretSalt", part.SecretSalt);
        }
    }
}