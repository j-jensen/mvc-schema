using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MvcSchema.Mvc
{
    internal class Router : IRouter
    {
        private readonly IRouter m_defaultRouter;
        private readonly string m_routePath;

        public Router(IRouter defaultRouter, string routePath)
        {
            m_defaultRouter = defaultRouter;
            m_routePath = routePath;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Path == m_routePath)
            {
                RouteData routeData = new RouteData(context.RouteData);
                routeData.Routers.Add(m_defaultRouter);
                routeData.Values["controller"] = "mvcschema";
                routeData.Values["action"] = "getschema";
                context.RouteData = routeData;
                await m_defaultRouter.RouteAsync(context);
            }
        }
    }
}
