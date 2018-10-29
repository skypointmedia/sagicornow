using System.Web.Mvc;
using System.Web.Routing;

namespace SagicorNow
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
					"Static",
                    "pages/{page}",
					new { controller = "Static", action = "GetStaticContent" }
				);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Quote", action = "Index", id = UrlParameter.Optional }
			);


		}
	}
}
