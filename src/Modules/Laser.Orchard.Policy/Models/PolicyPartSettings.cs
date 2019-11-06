﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Laser.Orchard.Policy.Models {
    public class PolicyPartSettings {
        public PolicyPartSettings() {
            PolicyTextReferences = Enumerable.Empty<string>().ToArray();
        }
        public IncludePendingPolicyOptions IncludePendingPolicy { get; set; }
        public string[] PolicyTextReferences { get; set; }
    }
}