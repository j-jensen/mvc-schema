
namespace MvcSchema.Analyzer.Types
{
    public class Argument : IArgument
    {
        public string Name { get; set; }
        public IType Type { get; set; }
    }
}
