using System.Web.Mvc;
using System.Web.Routing;
using ChatApp;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
[assembly: OwinStartup(typeof(Startup))]

namespace ChatApp
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterRoutes(RouteTable.Routes);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/home/login"),
                AuthenticationType = Settings.CookieName

            });
            app.MapSignalR();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

    public class Settings
    {
        public static readonly string CookieName = "chatCookie";
    }
}