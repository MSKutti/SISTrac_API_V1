using BusinessEntity;
using BusinessService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEOYONAPI.Controllers
{
   // [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BreakdownController : ControllerBase
    {

        private readonly IBreakdownService _breakdownservices;

        public BreakdownController (IBreakdownService breakdownService)
        {
            _breakdownservices = breakdownService;
        }

        [HttpPost]
        public IActionResult GetBreakdownData(BreakdownEntity Breakdown)
        {
            var BreakdownResponse = _breakdownservices.GetBreakdownData(Breakdown);
            return BreakdownResponse !=null ? Ok(BreakdownResponse):NotFound();
        }

        [HttpPost]
        public IActionResult SetBreakdownReport(BreakdownEntity breakdown)
        {
            
            var breakdownreport = _breakdownservices.SetBreakReport(breakdown);
            return breakdownreport != null ? Ok(breakdownreport) : NotFound();
        }
    }
}
