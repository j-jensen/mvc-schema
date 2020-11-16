using Microsoft.AspNetCore.Mvc;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;


namespace MvcSchemaTests.Analyzer
{
    public partial class TypeParserTests
    {
        [Test]
        [Ignore("To be implemented.")]
        public void ActionResultOfT_shold_return_type_of_value_property()
        {
            var ar = typeof(ActionResult<POCO>);
            var sut = new TypeParser();

            var actual = sut.ParseType(ar);

            Assert.AreEqual(TypeDescriptor.GetName(typeof(POCO)), actual.Name);
        }
    }
}
