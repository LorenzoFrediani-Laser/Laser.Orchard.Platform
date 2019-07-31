﻿using Laser.Orchard.Policy.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Laser.Orchard.Policy.Handlers {
    public class PolicyTextInfoPartHandler : ContentHandler {
        public PolicyTextInfoPartHandler(IRepository<PolicyTextInfoPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}