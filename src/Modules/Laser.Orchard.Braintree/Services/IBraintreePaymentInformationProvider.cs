using Laser.Orchard.Braintree.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laser.Orchard.Braintree.Services {
    public interface IBraintreePaymentInformationProvider : IDependency {

        PaymentVM AddInformation(PaymentVM model);
    }
}
