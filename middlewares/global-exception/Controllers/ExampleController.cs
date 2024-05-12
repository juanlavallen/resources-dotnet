using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("v1/globalexception")]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult RandomException()
        {
            throw new Exception("[Random Exception]");
        }
    }
}