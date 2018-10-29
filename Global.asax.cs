using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Net;


namespace SagicorNow
{
	public class Global : HttpApplication
	{
		protected void Application_Start()
		{
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			AreaRegistration.RegisterAllAreas();
			//GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
