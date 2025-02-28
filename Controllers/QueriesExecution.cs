﻿
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
        int totalFilesInFolder;

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
        Dictionary<string, Dictionary<int, string>> errorDetail = new Dictionary<string, Dictionary<int, string>>();

        //async Task RunQueryFromFilesAsync(string connectionStringFormat , string dbName)
        //{
        //    IEnumerable<string> txtFiles = GetPathAndTextFiles();
        //    int filePosition = 1;
        //    int totalFilesInFolder = txtFiles.Count();
        //    List<int> filesWithError = new List<int>();
        //    List<int> filesRunSuccessfully = new List<int>();
        //    foreach (string currentFile in txtFiles)
        //    {
        //        using StreamReader streamReader = new StreamReader(currentFile);
        //        string fromTextFile = await streamReader.ReadToEndAsync();
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionStringFormat))
        //            {
        //                string query = fromTextFile;

        //                SqlCommand cmd1 = new SqlCommand(query, conn);
        //                await cmd1.Connection.OpenAsync();

        //                SqlDataReader retrievedValue = await cmd1.ExecuteReaderAsync();
        //                filesRunSuccessfully.Append(filePosition);
        //                if(filesRunSuccessfully.Count == totalFilesInFolder)
        //                {
        //                    string allOrNot = "All";
        //                }
        //                //_logger.LogInformation($"Database Name:  {dbName} \n {filePosition} executed sucessfully");
        //                filePosition++;
        //                if(filePosition > totalFilesInFolder)
        //                {
        //                    //_logger.LogInformation($"****Queries from all files run successfully in database named {dbName}****");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //_logger.LogError($"Queries from FileNo. \'{filePosition}\' couldn't run in database named {dbName}; \n Error Detail: {ex.Message}");
        //            filesWithError.Append(filePosition);
        //            filePosition++;
        //        }

        //        _logger.LogInformation($"{filesRunSuccessfully.Count()} executed sucessfully");
        //    }


        //}

        async Task<int> RunQueryFromFilesAsync(string connectionStringFormat, string dbName)
        {
            IEnumerable<string> txtFiles = GetPathAndTextFiles();
            int filePosition = 1;
            totalFilesInFolder = txtFiles.Count();
            List<int> filesWithErrorInThisDB = new List<int>();
            List<int> filesRunSuccessfullyInThisDB = new List<int>();

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
                        filesRunSuccessfullyInThisDB.Add(filePosition);
                  
                    }
                }
                catch (Exception ex)
                {
                    filesWithErrorInThisDB.Add(filePosition);
                    errorDetail[dbName] = new Dictionary<int, string>();
                    errorDetail[dbName][filePosition] = ex.Message;
                }

                filePosition++;

            }

            return filesRunSuccessfullyInThisDB.Count;

        }

        //public async Task GetConnectionString()
        //{
        //    string connectionStringformat;
        //    List<string> connectionStringFormatArray=new List<string>();
        //    ConnectionStringEnt cse = new ConnectionStringEnt();

        //    try
        //    {
        //        List<ConnectionStringEnt> connStringList = await _myDBContext.COMPANY_DATABASE_INFO.ToListAsync();
        //        var ServerDBInfoList = connStringList.Select(each => new
        //        {
        //            connStringServerName = each.SERVERNAME,
        //            connStringDatabaseName = each.DATABASENAME,
        //            connStringUserName= each.DBUSER,
        //            connStringPassword = EncryptionHelper.Encrypt(EncryptionHelper.FromHexString((each.DBPASSWORD).ToString()))
        //        }).ToList();

        //        bool IntegratedSecurity = false;
        //        bool TrustServerCertificate = true;

        //        foreach (var col in ServerDBInfoList)
        //        {      
        //            connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};User ID = {col.connStringUserName}; Password = {col.connStringPassword}; Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";
        //            await RunQueryFromFilesAsync(connectionStringformat, col.connStringDatabaseName);
        //            //_logger.LogInformation($"****Queries from all files run successfully in database named {dbName}****");
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError($"\nError detail: {ex.Message}");

        //    }

        //}
         

        public async Task GetConnectionString()
        {
            string connectionStringformat;
            int DBWithAllFileSuccess = 0;
            List<string> connectionStringFormatArray = new List<string>();
            ConnectionStringEnt cse = new ConnectionStringEnt();

            try
            {
                List<ConnectionStringEnt> connStringList = await _myDBContext.COMPANY_DATABASE_INFO.ToListAsync();
                var ServerDBInfoList = connStringList.Select(each => new
                {
                    connStringServerName = each.SERVERNAME,
                    connStringDatabaseName = each.DATABASENAME,
                    connStringUserName = each.DBUSER,
                    connStringPassword = EncryptionHelper.Encrypt(EncryptionHelper.FromHexString((each.DBPASSWORD).ToString()))
                }).ToList();

                bool IntegratedSecurity = false;
                bool TrustServerCertificate = true;

                foreach (var col in ServerDBInfoList)
                {
                    connectionStringformat = $"Data Source={col.connStringServerName};Initial Catalog={col.connStringDatabaseName};User ID = {col.connStringUserName}; Password = {col.connStringPassword}; Integrated Security={IntegratedSecurity};Trust Server Certificate={TrustServerCertificate}";

                    int filesExecutedNum= await RunQueryFromFilesAsync(connectionStringformat, col.connStringDatabaseName);

                    if (filesExecutedNum == totalFilesInFolder)
                    {
                        DBWithAllFileSuccess++;
                    }
                    else
                    {
                        errorDetail[col.connStringDatabaseName][-1] = filesExecutedNum.ToString();
                    }

                }

                if (DBWithAllFileSuccess == ServerDBInfoList.Count())
                {
                    _logger.LogInformation($"Run all queries successfully in all {DBWithAllFileSuccess} databases****");
                }
                else
                {
                    foreach (var db in errorDetail)
                    {
                        _logger.LogInformation($"Database: {db.Key} \n {db.Value[-1]} files executed successfully");
                        foreach (var error in db.Value)
                        {
                            if (error.Key != -1)
                            {
                                _logger.LogInformation($"  FileNo {error.Key}: {error.Value}");
                            }
                           
                        }
                    }
                    _logger.LogInformation($"All queries run successfully only in {DBWithAllFileSuccess} databases****");  
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"\nError detail: {ex.Message}");

            }

        }


    }
}

