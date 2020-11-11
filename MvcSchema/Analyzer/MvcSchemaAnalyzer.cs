using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MvcSchema.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvcSchema.Analyzer
{
    public class MvcSchemaAnalyzer : IMvcSchemaAnalyzer
    {
        private readonly IActionDescriptorCollectionProvider m_actionDescriptorCollectionProvider;
        private readonly TypeParser _typeparser;

        public MvcSchemaAnalyzer(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            m_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _typeparser = new TypeParser();
        }

        public Schema GetSchema()
        {
            List<RouteInformation> ret = new List<RouteInformation>();

            var routes = m_actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (ActionDescriptor ad in routes)
            {
                RouteInformation info = new RouteInformation();

                // Area
                if (ad.RouteValues.ContainsKey("area"))
                {
                    info.Area = ad.RouteValues["area"];
                }

                // Path and Invocation of Razor Pages
                if (ad is PageActionDescriptor)
                {
                    var pad = ad as PageActionDescriptor;
                    info.Path = pad.ViewEnginePath;
                    info.Invocation = pad.RelativePath;
                }

                // Path of Route Attribute
                if (ad.AttributeRouteInfo != null)
                {
                    info.Path = $"/{ad.AttributeRouteInfo.Template}";
                }

                // Path and Invocation of Controller/Action
                if (ad is ControllerActionDescriptor)
                {
                    var cad = ad as ControllerActionDescriptor;
                    if (info.Path == "")
                    {
                        info.Path = $"/{cad.ControllerName}/{cad.ActionName}";
                    }
                    info.Invocation = $"{cad.ControllerName}Controller.{cad.ActionName}";
                }

                // Extract HTTP Verb
                if (ad.ActionConstraints != null && ad.ActionConstraints.Select(t => t.GetType()).Contains(typeof(HttpMethodActionConstraint)))
                {
                    HttpMethodActionConstraint httpMethodAction =
                        ad.ActionConstraints.FirstOrDefault(a => a.GetType() == typeof(HttpMethodActionConstraint)) as HttpMethodActionConstraint;

                    if (httpMethodAction != null)
                    {
                        info.HttpMethod = string.Join(",", httpMethodAction.HttpMethods);
                    }
                }

                // Extract parameters
                if (ad.Parameters != null)
                {
                    info.Arguments = ad.Parameters.Select(_typeparser.ParseParameter).ToArray();
                }

                // Special controller path
                if (info.Path == "/MvcSchema/GetSchema")
                {
                    info.Path = MvcSchemaServiceRouteBuilderExtensions.MvcSchemaUrlPath;
                }

                // Additional information of invocation
                info.Invocation += $" ({ad.DisplayName})";

                // Generating List
                ret.Add(info);
            }

            return new Schema { 
            Routes= ret,
            Types = _typeparser.Types
            };
        }
    }
}
