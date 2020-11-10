using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSchema.Impl
{
    public class MvcSchemaController : Controller
    {
        private IMvcSchemaAnalyzer _mvcSchemaAalyzer;
        public MvcSchemaController(IMvcSchemaAnalyzer mvcSchemaAalyzer)
        {
            _mvcSchemaAalyzer = mvcSchemaAalyzer;
        }
        public IActionResult GetSchema()
        {
            var infos = _mvcSchemaAalyzer.GetSchema();
            string ret = "";
            foreach (var info in infos)
            {
                ret += info.ToString() + "\n";
            }
            return Content(ret);
        }
    }
}
