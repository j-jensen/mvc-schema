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

        [TestCase(typeof(string), "System.String")]
        [TestCase(typeof(int), "System.Int32")]
        [TestCase(typeof(Nullable<int>), "System.Nullable<System.Int32>")]
        [TestCase(typeof(Nullable<float>), "System.Nullable<System.Single>")]
        [TestCase(typeof(Tuple<float,int>), "System.Tuple<System.Single,System.Int32>")]
        public void Typenaming_should_follow_rules(Type type, string exspected)
        {
            var actual = TypeDescriptor.GetID(type);
            Assert.AreEqual(exspected, actual);
        }
    }
}
