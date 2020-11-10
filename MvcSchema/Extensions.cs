using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcSchema.Impl;

namespace MvcSchema
{
    public static class MvcSchemaServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcSchema(this IServiceCollection services)
        {
            services.AddSingleton<IMvcSchemaAnalyzer, MvcSchemaAnalyzer>();
            return services;
        }
    }

    public static class MvcSchemaServiceRouteBuilderExtensions
    {
        public static string MvcSchemaUrlPath { get; private set; } = "";

        public static IRouteBuilder MapMvcSchema(this IRouteBuilder routes, string routeAnalyzerUrlPath)
        {
            MvcSchemaUrlPath = routeAnalyzerUrlPath;
            routes.Routes.Add(new Router(routes.DefaultHandler, routeAnalyzerUrlPath));
            return routes;
        }
        public static IEndpointRouteBuilder MapMvcSchema(this IEndpointRouteBuilder builder, string routeAnalyzerUrlPath)
        {
            MvcSchemaUrlPath = routeAnalyzerUrlPath;
            builder.MapControllerRoute("mvc-schema", MvcSchemaUrlPath, new { controller= "MvcSchema", action= "GetSchema" });
            return builder;
        }
    }
}
