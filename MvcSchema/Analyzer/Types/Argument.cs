
namespace MvcSchema.Analyzer.Types
{
    public class Argument : Identifier
    {
    }
    public class Property : Identifier
    {
    }
    public class Identifier
    {
        public string Name { get; set; }
        public TypeDescriptor Type { get; set; }
    }
}
