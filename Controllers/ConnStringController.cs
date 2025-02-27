using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Data;
using SAAS_Query_API.Services;

namespace SAAS_Query_API.Controllers
{
    public class ConnStringController 
    {
        private readonly MyDBContext _myDBContext;
        private readonly string? _path;
        private readonly ILogger<ConnStringController> _logger;

        public ConnStringController(MyDBContext myDBContext, IConfiguration configuration, ILogger<ConnStringController> logger)
        {
            _myDBContext = myDBContext;
            _path = configuration["AppSettings:FolderPath"];
            _logger = logger;
        }

        IEnumerable<string> GetPathAndTextFiles()
        {
            if (string.IsNullOrEmpty(_path)){
                _logger.LogWarning("No folder path provided in configuration.");
                throw new Exception("There is no given path in this device");
            }

            IEnumerable<string> txtFiles;
            _logger.LogInformation($"Fetching text files from path: {_path}");
            txtFiles = Directory.EnumerateFiles(_path, "*.txt"); 
            _logger.LogInformation($"Found {txtFiles.Count()} text files.");

            return txtFiles;
        }

        async Task RunQueryFromFilesAsync(List<string> connectionStringFormatArray)
        {    
            try
            {
                IEnumerable<string> txtFiles =GetPathAndTextFiles();
                _logger.LogInformation("Starting query execution from text files.");

                foreach (string currentFile in txtFiles)
                {
                    _logger.LogInformation($"Reading SQL from file: {currentFile}");
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
                            _logger.LogInformation($"Query executed successfully");


                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while executing queries: {ex.Message}");
            }
            
        }

        public async Task<string> GetConnectionString()
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
                    connStringPassword = EncryptionHelper.Encrypt(EncryptionHelper.FromHexString((each.DBPASSWORD).ToString()))
                }).ToList();

                bool IntegratedSecurity = false;
                bool TrustServerCertificate = true;

                foreach (var col in ServerDBInfoList)
                {      
                    connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};User ID = {col.connStringUserName}; Password = {col.connStringPassword}; Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
                    _logger.LogInformation($"Connecting to {col.connStringDatabaseName} of {col.connStringServerName}.");
                    connectionStringFormatArray.Add(connectionStringformat);
                }

                await RunQueryFromFilesAsync(connectionStringFormatArray);
                return "Ok";
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to get connection strings: {ex.Message}");
                return ex.Message;

            }

        }
    }
}

