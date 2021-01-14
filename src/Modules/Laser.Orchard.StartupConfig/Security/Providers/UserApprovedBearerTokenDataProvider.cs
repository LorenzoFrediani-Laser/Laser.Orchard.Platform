﻿using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laser.Orchard.StartupConfig.Security.Providers {
    [OrchardFeature("Laser.Orchard.BearerTokenAuthentication")]
    public class UserApprovedBearerTokenDataProvider : BaseBearerTokenDataProvider {
        public UserApprovedBearerTokenDataProvider(
            // Inject any required IDependency    
            ) : base(false) {
            // MyImplementation ctor body
        }

        protected override string Value(IUser user) {
            var userPart = user.As<UserPart>();
            if (userPart == null) {
                return string.Empty;
            }
            return userPart.RegistrationStatus.ToString();
        }

        public override bool IsValid(IUser user, IDictionary<string, string> userData) {
            var userPart = user.As<UserPart>();
            if (userPart == null) {
                return false;
            }
            return userPart.RegistrationStatus == UserStatus.Approved;
        }
    }
}