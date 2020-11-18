using Microsoft.AspNetCore.Mvc;
using MvcSchema.Analyzer;

namespace MvcSchema.Mvc
{
    public class MvcSchemaController : Controller
    {
        private readonly IMvcSchemaAnalyzer _mvcSchemaAalyzer;
        public MvcSchemaController(IMvcSchemaAnalyzer mvcSchemaAalyzer)
        {
            _mvcSchemaAalyzer = mvcSchemaAalyzer;
        }
        public ActionResult<Schema> GetSchema()
        {
            Schema infos = _mvcSchemaAalyzer.GetSchema();

            return infos;
        }
    }
}
