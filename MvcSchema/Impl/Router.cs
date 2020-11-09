using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MvcSchema.Impl
{
    class Router : IRouter
    {
        IRouter m_defaultRouter;
        string m_routePath;

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
                var routeData = new RouteData(context.RouteData);
                routeData.Routers.Add(m_defaultRouter);
                routeData.Values["controller"] = "mvc-schema";
                routeData.Values["action"] = "get-schema";
                context.RouteData = routeData;
                await m_defaultRouter.RouteAsync(context);
            }
        }
    }
}
