using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ExampleWebApplication
{
    public static class Routing
    {
        public static void BuildRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "player",
                template: "players/{id?}",
                defaults: new { controller = "Players", action = "Show" });
        }
    }
}