using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Data;
using SAAS_Query_API.Services;
using System.Runtime.InteropServices;

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

        //[HttpGet]
        [HttpGet("{path}")]
        public ActionResult GetConnectionString(string path)
        {
            string connectionStringformat;
            List<string> connectionStringFormatArray=new List<string>();
            ConnectionStringEnt cse = new ConnectionStringEnt();

            try
            {
                List<ConnectionStringEnt> connStringList = _myDBContext.COMPANY_DATABASE_INFO.ToList();
                var ServerDBInfoList = connStringList.Select(each => new
                {
                    connStringServerName = each.SERVERNAME,
                    connStringDatabaseName = each.DATABASENAME
                }).ToList();

                bool IntegratedSecurity = true;
                bool TrustServerCertificate = true;

                foreach (var col in ServerDBInfoList)
                {
                    connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
                    Console.WriteLine($"The connection string is : {connectionStringformat}");
                    connectionStringFormatArray.Add(connectionStringformat);
                }

                //string filename = @"H:\DOT NET INTERNSHIP\SAAS-Project\QueryOutside.txt";
                //using StreamReader streamReader = new StreamReader(filename);
                //string fromTextFile = streamReader.ReadToEnd();
                //Console.WriteLine($"++++++ {fromTextFile}");

                IEnumerable<string> txtFiles;
                 if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                     Console.WriteLine("Linux");
                     txtFiles = Directory.EnumerateFiles(path);
                    Console.WriteLine($"The path is {path}");
                    //txtFiles = Directory.EnumerateFiles(@"/dev/sdh/DOT NET INTERNSHIPSAAS-Project/SAAS Query API/SQL Query Files Folder/", "*.txt");//linux
                }
                else
                {
                    Console.WriteLine("Windows");
                    txtFiles = Directory.EnumerateFiles(path);
                    Console.WriteLine($"The path is {path}");
                    //txtFiles = Directory.EnumerateFiles(@"H:\DOT NET INTERNSHIP\SAAS-Project\SAAS Query API\SQL Query Files Folder\", "*.txt"); //windows
                }
              

                foreach (string currentFile in txtFiles)
                {
                    using StreamReader streamReader = new StreamReader(currentFile);
                    string fromTextFile = streamReader.ReadToEnd();

                    foreach (var connString in connectionStringFormatArray)
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            string query = fromTextFile;
                            SqlCommand cmd1 = new SqlCommand(query, conn);
                            cmd1.Connection.Open();
                            SqlDataReader retrievedValue = cmd1.ExecuteReader();
                        }
                    }
                }
                
                
                return Ok(connectionStringFormatArray);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
            

        } 

    }
}


//ifnotfoundpath , there existsnocolumnIDindifferentDB , whatifblank , incorrect syntax of sql inside code