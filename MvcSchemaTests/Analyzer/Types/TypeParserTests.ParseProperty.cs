using NUnit.Framework;
using MvcSchema.Analyzer.Types;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace MvcSchemaTests.Analyzer.Types
{
    public partial class TypeParserTests
    {
        [TestCase(typeof(string[]))]
        [TestCase(typeof(ICollection<int>))]
        [TestCase(typeof(List<Guid>))]
        [TestCase(typeof(IEnumerable<Tuple<string, string>>))]
        [TestCase(typeof(IReadOnlyList<object>))]
        public void Arraylike_property_should_return_elementtype_and_kind_Array(Type type)
        {
            var sut = new TypeParser();
            var pi = CreatePI(type);
            var actual = sut.ParseProperty(pi, new string[0]);

            Assert.AreEqual(Kind.Array, actual.Kind);
            // TODO: Check if type is elementtype
        }

        [TestCase(typeof(int), Kind.Scalar)]
        [TestCase(typeof(int?), Kind.Nullable)]
        [TestCase(typeof(int[]), Kind.Array)]
        public void ParseProperty_should_set_Kind(Type type, Kind kind)
        {
            var sut = new TypeParser();
            var pi = CreatePI(type);
            var actual = sut.ParseProperty(pi, new string[0]);

            Assert.AreEqual(kind, actual.Kind);
        }

        [TestCase(typeof(int), Kind.Scalar)]
        [TestCase(typeof(int?), Kind.Nullable)]
        [TestCase(typeof(int[]), Kind.Array)]
        public void ParseParameter_should_set_Kind(Type type, Kind kind)
        {
            var sut = new TypeParser();
            var pi = CreatePI(type);
            var actual = sut.ParseParameter(new Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor { Name = "Test1", ParameterType = type });

            Assert.AreEqual(kind, actual.Kind);
        }

        protected PropertyInfo CreatePI(Type type)
        {
            var tuple = typeof(Tuple<>).MakeGenericType(type);
            return tuple.GetProperty("Item1", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
