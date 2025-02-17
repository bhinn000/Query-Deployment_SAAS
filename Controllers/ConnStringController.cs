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
            string connectionStringformat;
            int numOfRows= _myDBContext.COMPANY_DATABASE_INFO.Count();
            List<string> connectionStringFormatArray=new List<string>();
            List<ConnectionStringEnt> connStringList = _myDBContext.COMPANY_DATABASE_INFO.ToList();
            ConnectionStringEnt cse = new ConnectionStringEnt();

            var ServerDBInfoList=connStringList.Select(each => new
            {
                connStringServerName=each.SERVERNAME,
                connStringDatabaseName=each.DATABASENAME
            }).ToList();

            bool IntegratedSecurity = true;
            bool TrustServerCertificate = true;

            foreach(var col in ServerDBInfoList)
            {
                connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
                Console.WriteLine($"The connection string is : {connectionStringformat}");
                connectionStringFormatArray.Add(connectionStringformat); 
            }

            foreach(var connString in connectionStringFormatArray)
            {
                Console.WriteLine($"From the array , here  is : {connString}");
            }
            return Ok(_myDBContext.COMPANY_DATABASE_INFO.ToList()); //must be table names from the database
        } 

    }
}
