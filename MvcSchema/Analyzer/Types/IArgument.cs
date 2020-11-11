
namespace MvcSchema.Analyzer.Types
{
    public interface IArgument
    {
        public string Name { get; }
        public IType Type { get; }
    }
}
