using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcCookieAuthSample.ViewModels;

namespace MvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private IClientStore _clientStore;
        private IResourceStore _resourceStore;
        private IIdentityServerInteractionService _identityServerInteractionServer;
        public ConsentService(IClientStore clientStore, IResourceStore resoruceStore, IIdentityServerInteractionService identityServerInteractionServer)
        {
            _clientStore = clientStore;
            _resourceStore = resoruceStore;
            _identityServerInteractionServer = identityServerInteractionServer;
        }

        #region private
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel model)
        {
            var rememberConsent = model?.RememberConsent ?? true;
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();
            return new ConsentViewModel
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ClientlogoUrl = client.LogoUri,
                ClientUrl = client.ClientUri,
                RememberConsent = rememberConsent, 
                IdentityScopes = resources.IdentityResources.Select(i => CreateScope(i, selectedScopes.Contains(i.Name) ||model==null)),
                RecourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScope(x, selectedScopes.Contains(x.Name)|| model==null))
            };
        }

        private ScopeViewModel CreateScope(IdentityResource resource,bool check)
        {
            return new ScopeViewModel
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                Emphasize = resource.Emphasize,
                Required = resource.Required,
                Check = check || resource.Required,
                Description = resource.Description,

            };
        }

        private ScopeViewModel CreateScope(Scope scope,bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Check = check||scope.Required,
                Description = scope.Description,

            };
        }
        #endregion

        public async Task<ConsentViewModel> BuildConsentViewModelAsync(string returnUrl, InputConsentViewModel model=null)
        {
            var request = await _identityServerInteractionServer.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                return null;
            }
            var client = await _clientStore.FindClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var vm = CreateConsentViewModel(request, client, resources,model);
            vm.ReturnUrl = returnUrl;

            return vm;

        }

        public async Task<ProcessConsetnResult> ProcessConsent(InputConsentViewModel model)
        {
            var result = new ProcessConsetnResult() { };
            ConsentResponse consentResponse=null;
            if (model.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = model.ScopesConsented
                    };
                }
                else
                {
                    result.ValidationError = "请至少选择一个";
                }
            }
            if (consentResponse != null)
            {
                var request = await _identityServerInteractionServer.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionServer.GrantConsentAsync(request, consentResponse);
                result.RedirectUrl = model.ReturnUrl;
            }

            var consentViewModel = await BuildConsentViewModelAsync(model.ReturnUrl, model);
            result.ViewModel = consentViewModel;
            return result;
        }
    }
}
