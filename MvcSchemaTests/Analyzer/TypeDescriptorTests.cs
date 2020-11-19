using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcSchemaTests.Analyzer
{
    public class TypeDescriptorTests
    {
        [TestCase(typeof(string), DataType.String)]
        [TestCase(typeof(int), DataType.Number)]
        [TestCase(typeof(float), DataType.Number)]
        [TestCase(typeof(bool), DataType.Boolean)]
        [TestCase(typeof(DateTime), DataType.Object)]
        public void Instance_should_have_correct_JsType(Type clrType, DataType expected)
        {
            var type = new TypeDescriptor(clrType);

            Assert.AreEqual(expected, type.DataType);
        }
    }
}
