using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.ViewModels
{
    public class ConsentViewModel: InputConsentViewModel
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientlogoUrl { get; set; }

        public string ClientUrl { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> RecourceScopes { get; set; }

    }
}
