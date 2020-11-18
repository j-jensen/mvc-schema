using Microsoft.AspNetCore.Mvc;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MvcSchemaTests.Analyzer
{
    public partial class TypeParserTests
    {
        [Test(Description = "ActionResults from dotNetCore")]
        [TestCase(typeof(ActionResult<POCO>), typeof(POCO))]
        [TestCase(typeof(Task<ActionResult<POCO>>), typeof(POCO))]
        [TestCase(typeof(Task<ActionResult>), typeof(string))]
        public void ActionResultOfT_shold_return_type_of_value_property(Type ar, Type result)
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(ar);

            Assert.AreEqual(result.GetID(), actual.ID);
        }

        [TestCase(typeof(JsonResult), "JsonString", typeof(string))]
        [TestCase(typeof(ViewResult), "ViewString", typeof(string))]
        [TestCase(typeof(ContentResult), "System.String", typeof(string))]
        public void Derived_from_ActionResult_could_return_type_or_string(Type resultType, string id, Type clrType)
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(resultType);

            Assert.AreEqual(id, actual.ID);
            Assert.AreEqual(clrType, actual.ClrType);
        }
    }
}
