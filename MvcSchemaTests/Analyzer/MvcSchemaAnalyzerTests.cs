using NUnit.Framework;
using MvcSchema.Analyzer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NSubstitute;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace MvcSchemaTests.Analyzer
{
    public class MvcSchemaAnalyzerTests
    {

        [Test]
        public void GetSchema_should_return_Schema()
        {
            var arg = new { a = 1, b = new string[] { "2" } };
            var adcp = Substitute.For<IActionDescriptorCollectionProvider>();
            var adc = new ActionDescriptorCollection(new List<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor> {
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor {
                    Parameters = new ParameterDescriptor[]{
                    new ParameterDescriptor
                    {
                        Name="Test",
                        ParameterType= arg.GetType()
                    }
                    }
                }
            }.AsReadOnly(), 1);
            adcp.ActionDescriptors.Returns(adc);

            var sut = new MvcSchemaAnalyzer(adcp);
            var scheme = sut.GetSchema();

            Assert.NotNull(scheme);
        }

    }
}
