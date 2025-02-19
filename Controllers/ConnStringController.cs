using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        //public ActionResult GetConnectionString()
        public List<string> GetConnectionString()
        {
            string connectionStringformat;
            //int numOfRows= _myDBContext.COMPANY_DATABASE_INFO.Count();
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

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "update Basic_Details set NumberOfWardOffice= 40 ;";
                    
                    SqlCommand cmd1 = new SqlCommand(query , conn);
                    cmd1.Connection.Open();
                    SqlDataReader retrievedValue =cmd1.ExecuteReader();   
                } 
            }
            //return Ok(_myDBContext.COMPANY_DATABASE_INFO.ToList()); //must be table names from the database
            return connectionStringFormatArray;
        } 

    }
}
