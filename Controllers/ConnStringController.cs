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
        private readonly string? _path;

        public ConnStringController(MyDBContext myDBContext, IConfiguration configuration)
        {
            _myDBContext = myDBContext;
            _path = configuration["AppSettings:FolderPath"];
        }

        IEnumerable<string> GetPathAndTextFiles()
        {
            if (string.IsNullOrEmpty(_path)){
                throw new Exception("There is no given path in this device");
            }

            IEnumerable<string> txtFiles;
            Console.WriteLine($"The path is {_path}");
            txtFiles = Directory.EnumerateFiles(_path, "*.txt"); //windows

            return txtFiles;
        }

        async Task  RunQueryFromFilesAsync(List<string> connectionStringFormatArray)
        {
            try
            {
                IEnumerable<string> txtFiles =GetPathAndTextFiles();
               

                foreach (string currentFile in txtFiles)
                {
                    using StreamReader streamReader = new StreamReader(currentFile);
                    string fromTextFile = await streamReader.ReadToEndAsync();

                    foreach (var connString in connectionStringFormatArray)
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            string query = fromTextFile;
                            SqlCommand cmd1 = new SqlCommand(query, conn);
                            await cmd1.Connection.OpenAsync();
                            SqlDataReader retrievedValue =await cmd1.ExecuteReaderAsync();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        [HttpGet]
        public async Task<ActionResult> GetConnectionString()
        {
            string connectionStringformat;
            List<string> connectionStringFormatArray=new List<string>();
            ConnectionStringEnt cse = new ConnectionStringEnt();

            try
            {
                List<ConnectionStringEnt> connStringList = await _myDBContext.COMPANY_DATABASE_INFO.ToListAsync();
                var ServerDBInfoList = connStringList.Select(each => new
                {
                    connStringServerName = each.SERVERNAME,
                    connStringDatabaseName = each.DATABASENAME,
                    connStringUserName= each.DBUSER,
                    connStringPassword = each.DBPASSWORD
                }).ToList();

                bool IntegratedSecurity = false;
                bool TrustServerCertificate = true;

                foreach (var col in ServerDBInfoList)
                {      
                    connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};User ID = {col.connStringUserName}; Password = {col.connStringPassword}; Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
                    Console.WriteLine($"The connection string is : {connectionStringformat}");
                    connectionStringFormatArray.Add(connectionStringformat);
                }

                await RunQueryFromFilesAsync(connectionStringFormatArray);
                
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
            
        } 

    }
}

