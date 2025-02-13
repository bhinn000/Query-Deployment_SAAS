using Microsoft.AspNetCore.Hosting.Server;

namespace SAAS_Query_API.Services
{
    public class ConnectionString
    {

        //"Data Source=DESKTOP-OINPHM8\\SQLEXPRESS01;Initial Catalog=CENTRAL;Integrated Security=True;Trust Server Certificate=True"

        //generate connection string for each row which is actually info for each database

        //these are not nullable and cant be empty too
        public string DataSource { get; set; } = string.Empty; //server name : SERVERNAME
        public string InitialCatalog { get; set; } = string.Empty; //DATABASENAME
        public bool IntegratedSecurity { get; set; } = false;
        public string UserName { get; set; } = string.Empty; //DBUSER
        public string Password { get; set; } = string.Empty; //DBPASSWORD
        public bool TrustServerCertificate { get; set; } = true; //

         
    

    }
}
