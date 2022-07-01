using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessEntity;
using BusinessService;


namespace SEOYONAPI.Controllers
{
   // [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _LoginService;
        public LoginController(ILoginService LoginService)
        {
            _LoginService = LoginService;
        }
               
        [HttpPost]
        public IActionResult Login(LoginEntity loginEntity)
        {            
             var ObjResponse = _LoginService.GetOtp(loginEntity);
             return ObjResponse != null ? Ok(ObjResponse) : NotFound();           
        }

        [HttpPost]
        public IActionResult Save(LoginEntity loginEntity)
        {
             var ObjResponse = _LoginService.SaveOtp(loginEntity);
             return ObjResponse != null ? Ok(ObjResponse) : NotFound(); 
        }

        [HttpPost]
        public IActionResult GetLocaton(LoginEntity loginEntity)
        {
            var ObjResponse = _LoginService.GetLocaton(loginEntity);
            return ObjResponse != null ? Ok(ObjResponse) : NotFound();
        }
    }
}
