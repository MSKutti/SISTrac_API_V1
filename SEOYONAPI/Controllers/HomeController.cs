using Microsoft.AspNetCore.Mvc;
using BusinessEntity;
using BusinessService;
using Microsoft.AspNetCore.Authorization;

namespace SEOYONAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly IHomeService _homeService;
      
        public HomeController(IHomeService homeservice)
        {
            _homeService = homeservice;
        }

        [HttpPost]
        public IActionResult Home(HomeEntity homeentity)
        {               
             var ObjResponse = _homeService.GetLocaton(homeentity);
             return ObjResponse != null ? Ok(ObjResponse) : NotFound();
        }

        [HttpPost]
        public IActionResult GetChecklistData(CheckListEntity ChkEntity)
        {
             var objchklist = _homeService.GetChecklistData(ChkEntity);
             return objchklist != null ? Ok(objchklist) : NotFound();           
        }


        [HttpPost]
        public IActionResult SaveChecklistData(CheckListEntity ChkEntity)
        {            
             var objchklist = _homeService.SavechecklistData(ChkEntity);
             return objchklist != null ? Ok(objchklist) : NotFound();            
        }

    }
}
