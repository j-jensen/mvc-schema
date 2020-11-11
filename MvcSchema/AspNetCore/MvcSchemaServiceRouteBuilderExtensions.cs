using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MvcSchema.Analyzer;
using MvcSchema.Mvc;

namespace MvcSchema.AspNetCore
{
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
