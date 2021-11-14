using System;
using ExampleWebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ViewTestExample.Support
{
    public static class RoutingHelper
    {
        public static RouteData GetRouteData(IServiceProvider serviceProvider)
        {
            var routeBuilder = new RouteBuilder(new ApplicationBuilder(serviceProvider))
            {
                DefaultHandler = new RouteHandler(null)
            };

            Routing.BuildRoutes(routeBuilder);

           return new RouteData
            {
                Routers = { routeBuilder.Build() }
            };
        }
    }
}