using Microsoft.AspNetCore.Mvc;

namespace SAAS_Query_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionToAllDBController: ControllerBase
    {

        [HttpGet]
        public ActionResult MakeConnectionToAllDB()
        {

            return Ok();
        }
    }
}
