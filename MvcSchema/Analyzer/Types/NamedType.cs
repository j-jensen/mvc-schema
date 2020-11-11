
namespace MvcSchema.Analyzer.Types
{
    public class NamedType : IType
    {
        public string Name { get; set; }
        public TypeKind Kind { get; set; }
        public IType OfType { get; set; } = null;
        public IType[] Fields { get; set; } = null;
    }
}
