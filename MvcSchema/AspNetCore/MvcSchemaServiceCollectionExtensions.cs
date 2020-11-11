using Microsoft.Extensions.DependencyInjection;
using MvcSchema.Analyzer;

namespace MvcSchema.AspNetCore
{
    public static class MvcSchemaServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcSchema(this IServiceCollection services)
        {
            services.AddSingleton<IMvcSchemaAnalyzer, MvcSchemaAnalyzer>();
            return services;
        }
    }
}
