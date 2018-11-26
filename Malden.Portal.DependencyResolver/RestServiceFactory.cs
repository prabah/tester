using Ninject.Extensions.Wcf;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Malden.Portal.DependencyResolver
{
    public class RestServiceFactory<TServiceContract> : NinjectWebServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost host = base.CreateServiceHost(serviceType, baseAddresses);
            var webBehavior = new WebHttpBehavior
            {
                AutomaticFormatSelectionEnabled = true,
                HelpEnabled = true,
                FaultExceptionEnabled = true
            };
            var endpoint = host.AddServiceEndpoint(typeof(TServiceContract), new WebHttpBinding(), "Rest");
            endpoint.Name = "rest";
            endpoint.Behaviors.Add(webBehavior);
            return host;
        }
    }
}
