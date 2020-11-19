using MvcSchema.Analyzer.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MvcSchemaTests.Analyzer.Types
{
    public class TypeExtensionsTests
    {
        [TestCase(typeof(string[]))]
        [TestCase(typeof(IEnumerable<string>))]
        [TestCase(typeof(StringArray))]
        public void GetKind_arraytypes(Type type)
        {
            var actual = type.GetKind();
            Assert.AreEqual(Kind.Array, actual);
        }

        [TestCase(typeof(string[]))]
        [TestCase(typeof(IEnumerable<string>))]
        [TestCase(typeof(StringArray))]
        public void GetSimplifiedType_arraytypes(Type type)
        {
            var actual = type.GetSimplifiedType();
            Assert.AreEqual(typeof(string[]), actual);
        }

        [TestCase(typeof(string), "None:System.String")]
        [TestCase(typeof(int), "None:System.Int32")]
        [TestCase(typeof(Nullable<int>), "Nullable:System.Nullable[None:System.Int32]")]
        [TestCase(typeof(Nullable<float>), "Nullable:System.Nullable[None:System.Single]")]
        [TestCase(typeof(Tuple<float, int>), "None:System.Tuple[None:System.Single,None:System.Int32]")]
        public void GetID_should_follow_rules(Type type, string exspected)
        {
            var actual = type.GetID();
            Assert.AreEqual(exspected, actual);
        }

    }

    class StringArray : IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
