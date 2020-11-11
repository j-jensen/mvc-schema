
namespace MvcSchema.Analyzer.Types
{
    public interface IType
    {
        public string Name { get; }
        public TypeKind Kind { get; }
        public IType OfType { get; }
        public IType[] Fields { get; }
    }
}
