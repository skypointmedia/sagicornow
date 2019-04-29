using System.ComponentModel.Composition.Hosting;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Net;
using System.Reflection;
using SagicorNow.Client.Bootstrapper;
using SagicorNow.Core;


namespace SagicorNow
{
	public class Global : HttpApplication
	{
		protected void Application_Start()
		{
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            CompositionContainer container = MEFLoader.Init(catalog.Catalogs);

            DependencyResolver.SetResolver(new MefDependencyResolver(container)); // view controllers
            GlobalConfiguration.Configuration.DependencyResolver = new MefAPIDependencyResolver(container); // web api controllers
        }
	}
}
