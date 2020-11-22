using MvcSchema.Analyzer;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvcSchemaTests.Analyzer.Types
{
    public partial class TypeParserTests
    {
        [Test]
        public void Object_types_should_return_TypeKindObject_and_collect_Type()
        {
            var obj = new { a = 1, b = "2" };
            var sut = new TypeParser();
            var actual = sut.ParseType(obj.GetType());

            Assert.AreEqual(DataType.Object, actual.DataType);

            // Is TypeDescriptor collected
            var collectedType = sut.TypeDescriptors.First(td => td.TypeName == actual.TypeName);

            Assert.NotNull(collectedType);
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(bool?))]
        public void Nullable_values_should_return_underlying_type(Type type)
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(type);
            var underlyingType = Nullable.GetUnderlyingType(type);

            Assert.AreEqual(underlyingType.GetNamespacedName(), actual.TypeName);
        }

        [Test]
        public void Enum_type_should_return_Enum()
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(typeof(BindingFlags));

            Assert.IsInstanceOf<EnumDescriptor>(actual);
        }

        [Test]
        public void Arraylike_with_same_type_should_return_same()
        {
            var sut = new TypeParser();

            var actual1 = sut.ParseType(typeof(string[]));
            var actual2 = sut.ParseType(typeof(List<string>));
            Assert.AreSame(actual1, actual2, $"{actual1.TypeName} == {actual2.TypeName}");
        }

        [Test]
        public void Array_types_should_have_array_type_id()
        {
            var array = typeof(string[]);
            var sut = new TypeParser();

            var actual1 = sut.ParseType(array);

            Assert.AreEqual(array.GetElementType().GetNamespacedName(), actual1.TypeName);
        }

        [Test]
        public void String_should_return_primitive()
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(typeof(string));

            Assert.IsNotInstanceOf<ObjectDescriptor>(actual);
            Assert.IsInstanceOf<TypeDescriptor>(actual);
            Assert.AreEqual(typeof(string).GetNamespacedName(), actual.TypeName);
        }

        [Test]
        public void Void_should_return_undefined_type()
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(typeof(void));
            Assert.AreEqual(DataType.Undefined, actual.DataType);
        }
        [Test]
        public void POCO_object_should_return_type_with_properties()
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(typeof(POCO));

            Assert.AreEqual(typeof(POCO).GetNamespacedName(), actual.TypeName);

            var objectType = sut.TypeDescriptors.OfType<ObjectDescriptor>().FirstOrDefault();
            Assert.AreEqual(3, objectType.Properties.Length);
        }
    }

    class POCO
    {
        public int PropA { get; set; }
        public string PropB { get; set; }
        public bool PropC { get; set; }
    }

    class Branch
    {
        public Branch Parent { get; set; }
    }

}
