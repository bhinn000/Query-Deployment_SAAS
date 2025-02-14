using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Services;

namespace SAAS_Query_API.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options) 
        {
            
        }

        //add table
        //public DbSet<ConnectionStringEnt> CentralDBConnectionString { get; set; }
        public DbSet<ConnectionStringEnt> COMPANY_DATABASE_INFO { get; set; }



    }
}
