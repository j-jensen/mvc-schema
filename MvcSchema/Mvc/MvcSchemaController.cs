using Microsoft.AspNetCore.Mvc;
using MvcSchema.Analyzer;

namespace MvcSchema.Mvc
{
    public class MvcSchemaController : Controller
    {
        private IMvcSchemaAnalyzer _mvcSchemaAalyzer;
        public MvcSchemaController(IMvcSchemaAnalyzer mvcSchemaAalyzer)
        {
            _mvcSchemaAalyzer = mvcSchemaAalyzer;
        }
        public ActionResult<Schema> GetSchema()
        {
            var infos = _mvcSchemaAalyzer.GetSchema();

            return infos;
        }
    }
}
