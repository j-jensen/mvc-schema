using MvcSchema.Analyzer;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvcSchemaTests.Analyzer
{
    public partial class TypeParserTests
    {
        //[TestCase(typeof(A))]
        [TestCase(typeof(Type))]
        //[TestCase(typeof(Branch))]
        public void Circular_types_should_not_throw_StackOverflowException(Type type)
        {
            var sut = new TypeParser();

            var actual = sut.ParseType(type);

            Assert.AreEqual(actual.ID, TypeDescriptor.GetID(type));
        }
    }

    class A
    {
        public B B { get; set; }
    }
    class B
    {
        public C C { get; set; }
    }
    class C
    {
        public A A { get; set; }
    }
}
