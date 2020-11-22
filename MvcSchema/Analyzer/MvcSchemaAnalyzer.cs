using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MvcSchema.Analyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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
            List<ActionDescriptor> ret = new List<ActionDescriptor>();

            IReadOnlyList<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor> routes = m_actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor ad in routes)
            {
                ActionDescriptor info = new ActionDescriptor();

                // Path and Invocation of Controller/Action
                if (ad is ControllerActionDescriptor)
                {
                    ControllerActionDescriptor cad = ad as ControllerActionDescriptor;
                    if (info.Path == "")
                    {
                        info.Path = $"/{cad.ControllerName}/{cad.ActionName}";
                    }
                    info.Invocation = $"{cad.ControllerName}Controller.{cad.ActionName}";
                }

                // Area
                if (ad.RouteValues.ContainsKey("area"))
                {
                    info.Area = ad.RouteValues["area"];
                }

                // Path and Invocation of Razor Pages
                if (ad is PageActionDescriptor)
                {
                    PageActionDescriptor pad = ad as PageActionDescriptor;
                    info.Path = pad.ViewEnginePath;
                    info.Invocation = pad.RelativePath;
                }

                // Path of Route Attribute
                if (ad.AttributeRouteInfo != null)
                {
                    info.Path = $"/{ad.AttributeRouteInfo.Template}";
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

                // Return type
                if (ad is ControllerActionDescriptor)
                {
                    ControllerActionDescriptor cad = ad as ControllerActionDescriptor;

                    info.ReturnType = new Identifier { 
                        Kind = cad.MethodInfo.ReturnType.GetKind(),
                        Type = _typeparser.ParseType(cad.MethodInfo.ReturnType).TypeName
                    };
                }

                // Additional information of invocation
                info.Invocation += $" ({ad.DisplayName})";

                // Special controller path
                if (info.Path == "/MvcSchema/GetSchema")
                {
                    continue;
                }

                // Generating List
                ret.Add(info);
            }

            return new Schema
            {
                Actions = ret.ToArray(),
                Types = _typeparser.TypeDescriptors.ToArray()
            };
        }
    }
}
