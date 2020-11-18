namespace MvcSchema.Analyzer.Types
{
    public class Placeholder : TypeDescriptor
    {
        public Placeholder(string id): base(id, typeof(object), DataType.Undefined)
        {
        }

        public string Description { get; set; } = null;
    }
}