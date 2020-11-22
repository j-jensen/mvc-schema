using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MvcSchemaTests.Analyzer
{
    public class EnumDescriptorTests
    {
        [Test]
        public void Should_have_matching_values()
        {
            var enumClrType = typeof(BindingFlags);
            var enumValues = Enum.GetNames(enumClrType);

            var descriptor = new EnumDescriptor(enumClrType);
            
            Assert.That(enumValues, Is.EquivalentTo(descriptor.Values));
        }
    }
}
