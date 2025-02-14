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
        public string connectionStringformat;

        public ConnStringController(MyDBContext myDBContext)
        {
            _myDBContext = myDBContext; 
        }  
    
        [HttpGet]
        public ActionResult GetConnectionString()
        {
            
            List<ConnectionStringEnt> connStringList = _myDBContext.COMPANY_DATABASE_INFO.ToList();
            ConnectionStringEnt cse = new ConnectionStringEnt();
            string? connStringServerName = connStringList.Select(each => each.SERVERNAME).FirstOrDefault() ; //if no server name then send NULL
            string? connStringDatabaseName = connStringList.Select(each => each.DATABASENAME).FirstOrDefault();
            bool IntegratedSecurity = true;
            bool TrustServerCertificate = true;

            connectionStringformat = $"Data Source={connStringServerName};Initial Catalog={connStringDatabaseName};Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
            Console.WriteLine($"The connection string is : {connectionStringformat}");

            return Ok(_myDBContext.COMPANY_DATABASE_INFO.ToList()); //must be table names from the database
        } 
    }
}
