using MvcSchema.Analyzer;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvcSchemaTests.Analyzer
{
    public class TypeParserTests
    {
        [Test]
        public void Object_types_should_return_TypeKindObject_and_collect_Type()
        {
            var obj = new { a = 1, b = "2" };
            var sut = new TypeParser();
            var actual = sut.ParseType(obj.GetType());

            Assert.AreEqual(DataType.Object, actual.JsType);

            // Is TypeDescriptor collected
            var collectedType = sut.TypeDescriptors.First(td => td.Name == actual.Name);

            Assert.NotNull(collectedType);
        }

        [Test]
        public void Nested_object_types_should_not_stack_overflow()
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(typeof(Branch));

            Assert.AreEqual(DataType.Object, actual.JsType);
            Assert.AreEqual(1, sut.TypeDescriptors.Count());

            var collectedType = sut.TypeDescriptors.OfType<ObjectDescriptor>().First();

            Assert.AreEqual(1, collectedType.Properties.Length);
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(bool?))]
        public void Nullable_values_should_return_Nullable(Type type)
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(type);
            var underlyingType = Nullable.GetUnderlyingType(type);

            Assert.AreSame(underlyingType, actual.ClrType);
            Assert.AreEqual(Kind.Nullable, actual.Kind);
        }

        [Test]
        public void Enum_type_should_return_Enum()
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(typeof(BindingFlags));

            Assert.AreEqual(Kind.Enum, actual.Kind);
        }

        [TestCase(typeof(string[]))]
        [TestCase(typeof(ICollection<int>))]
        [TestCase(typeof(List<Guid>))]
        [TestCase(typeof(IReadOnlyList<object>))]
        public void Array_like_should_return_KindArray(Type arrayLike)
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(arrayLike);
            Assert.AreEqual(Kind.Array, actual.Kind);
        }

        [Test]
        public void String_should_return_primitive()
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(typeof(string));
            Assert.AreEqual(Kind.None, actual.Kind);
            Assert.AreEqual(DataType.String, actual.JsType);
        }
    }

    class Branch
    {
        public Branch Parent { get; set; }
    }

}
