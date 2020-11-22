using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MvcSchemaTests.Analyzer.Types
{
    public partial class TyeParserTests
    {
        [Test]
        public void Object_with_array_props()
        {
            var type = typeof(Tuple<IEnumerable<int>>);

            var parser = new TypeParser();
            var actual = (ObjectDescriptor)parser.ParseType(type);

            Assert.AreEqual(Kind.Array, actual.Properties[0].Kind);
            Assert.AreEqual("System.Int32", actual.Properties[0].Type);
        }
        [Test]
        public void Object_with_nullable_props()
        {
            var type = typeof(Tuple<int?>);

            var parser = new TypeParser();
            var actual = (ObjectDescriptor)parser.ParseType(type);

            Assert.AreEqual(Kind.Nullable, actual.Properties[0].Kind);
            Assert.AreEqual("System.Int32", actual.Properties[0].Type);
        }
    }
}
