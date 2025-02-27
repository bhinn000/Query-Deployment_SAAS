using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Data;
using SAAS_Query_API.Services;

namespace SAAS_Query_API.Controllers
{
    public class QueriesExecution 
    {
        private readonly MyDBContext _myDBContext;
        private readonly string? _path;
        private readonly ILogger<QueriesExecution> _logger;

        public QueriesExecution(MyDBContext myDBContext, IConfiguration configuration, ILogger<QueriesExecution> logger)
        {
            _myDBContext = myDBContext;
            _path = configuration["AppSettings:FolderPath"];
            _logger = logger;
        }

        IEnumerable<string> GetPathAndTextFiles()
        {
            if (string.IsNullOrEmpty(_path)){
                throw new Exception("No folder path provided in configuration.");
            }

            IEnumerable<string> txtFiles;
            txtFiles = Directory.EnumerateFiles(_path, "*.txt"); 

            return txtFiles;
        }

        IEnumerable<string> txtFiles;
        async Task RunQueryFromFilesAsync(string connectionStringFormat , string dbName)
        {
            IEnumerable<string> txtFiles = GetPathAndTextFiles();
            int filePosition = 1;
            int totalFilesInFolder = txtFiles.Count();
            foreach (string currentFile in txtFiles)
            {
                using StreamReader streamReader = new StreamReader(currentFile);
                string fromTextFile = await streamReader.ReadToEndAsync();
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionStringFormat))
                    {
                        string query = fromTextFile;

                        SqlCommand cmd1 = new SqlCommand(query, conn);
                        await cmd1.Connection.OpenAsync();

                        SqlDataReader retrievedValue = await cmd1.ExecuteReaderAsync();

                        _logger.LogInformation($"Database Name:  {dbName} \n {filePosition} executed sucessfully");
                        filePosition++;
                        if(filePosition > totalFilesInFolder)
                        {
                            _logger.LogInformation($"****Queries from all files run successfully in database named {dbName}****");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Queries from FileNo. \'{filePosition}\' couldn't run in database named {dbName}; \n Error Detail: {ex.Message}");
                    filePosition++;
                }
            }
        }

        public async Task GetConnectionString()
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
                    await RunQueryFromFilesAsync(connectionStringformat, col.connStringDatabaseName);
                }
   
            }
            catch(Exception ex)
            {
                _logger.LogError($"\nError detail: {ex.Message}");
 
            }

        }
    }
}

