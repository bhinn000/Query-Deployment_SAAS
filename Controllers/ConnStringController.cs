using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Data;
using SAAS_Query_API.Services;

namespace SAAS_Query_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnStringController : ControllerBase
    {
        private readonly MyDBContext _myDBContext;

        public ConnStringController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext;
        }

        [HttpGet]
        public ActionResult GetConnectionString()
        {
            //ConnectionString connString= _myDBContext.dbConnectionString.ToList();
            //return Ok(_myDBContext.CentralDBConnectionString.ToList());
            return Ok(_myDBContext.COMPANY_DATABASE_INFO.ToList()); //must be table names from the database
        } 
    }
}
