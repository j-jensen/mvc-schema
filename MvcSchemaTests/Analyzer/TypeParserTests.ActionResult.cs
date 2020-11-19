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

            Assert.AreEqual(result.GetNamespacedName(), actual.TypeName);
        }

        [TestCase(typeof(JsonResult), "JsonString")]
        [TestCase(typeof(ViewResult), "ViewString")]
        [TestCase(typeof(ContentResult), "System.String")]
        public void Derived_from_ActionResult_could_return_type_or_string(Type resultType, string id)
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(resultType);

            Assert.AreEqual(id, actual.TypeName);
        }
    }
}
