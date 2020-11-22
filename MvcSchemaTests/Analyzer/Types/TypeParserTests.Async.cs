using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MvcSchema.Analyzer.Types;
using NUnit.Framework;


namespace MvcSchemaTests.Analyzer.Types
{
    public partial class TypeParserTests
    {
        [TestCase(typeof(Task<int>), typeof(int))]
        [TestCase(typeof(Task<ConsoleColor>), typeof(ConsoleColor))]
        [TestCase(typeof(Task<Tuple<int,int>>), typeof(Tuple<int, int>))]
        [TestCase(typeof(Task), typeof(void))]
        public void Task_should_be_unwrapped_and_return_result_type(Type taskType, Type type)
        {
            var sut = new TypeParser();
            var actual = sut.ParseType(taskType);

            Assert.AreEqual(type.GetNamespacedName(), actual.TypeName);
        }
    }
}
