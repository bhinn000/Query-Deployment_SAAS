using Microsoft.AspNetCore.Hosting.Server;
using System.ComponentModel.DataAnnotations;

namespace SAAS_Query_API.Services
{
    public class ConnectionStringEnt
    {

        //"Data Source=DESKTOP-OINPHM8\\SQLEXPRESS01;Initial Catalog=CENTRAL;Integrated Security=True;Trust Server Certificate=True"


        //generate connection string for each row which is actually info for each database

        //these are not nullable and cant be empty too

        [Key]
        [Required]
        public string CompanyID { get; set; }
        public string SERVERNAME  { get; set; } = string.Empty; //server name : DataSource
        public string DATABASENAME  { get; set; } = string.Empty; //InitialCatalog
        //public bool IntegratedSecurity { get; set; } = false;
        public string DBUSER { get; set; } = string.Empty; // UserName
        public string DBPASSWORD { get; set; } = string.Empty; // Password
        //public bool TrustServerCertificate { get; set; } = true; //
    }
}
