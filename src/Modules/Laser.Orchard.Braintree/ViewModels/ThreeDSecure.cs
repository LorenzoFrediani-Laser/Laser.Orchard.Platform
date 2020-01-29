using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Laser.Orchard.Braintree.ViewModels {
    public class ThreeDSecure {
        // objects of this class contain the 3DS information for a transaction
        /// <summary>
        /// This is the only mandatory property
        /// </summary>
        [JsonIgnore]
        public decimal Amount { get; set; }
        [JsonProperty("amount")]
        public string AmountString =>
            Amount.ToString("0.00", CultureInfo.InvariantCulture);
        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Email { get; set; }

        public string ToJson() {
            return JsonConvert.SerializeObject(this);
        }
    }
}