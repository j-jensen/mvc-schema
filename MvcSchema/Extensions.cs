using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcSchema.Impl;

namespace MvcSchema
{
    public static class MvcSchemaServiceCollectionExtensions
    {
        public static IServiceCollection AddRouteAnalyzer(this IServiceCollection services)
        {
            services.AddSingleton<IMvcSchema, MvcSchemaService>();
            return services;
        }
    }

    public static class MvcSchemaServiceRouteBuilderExtensions
    {
        public static string MvcSchemaUrlPath { get; private set; } = "";

        public static IRouteBuilder MapRouteAnalyzer(this IRouteBuilder routes, string routeAnalyzerUrlPath)
        {
            MvcSchemaUrlPath = routeAnalyzerUrlPath;
            routes.Routes.Add(new Router(routes.DefaultHandler, routeAnalyzerUrlPath));
            return routes;
        }
    }
}
