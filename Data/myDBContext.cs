using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Services;

namespace SAAS_Query_API.Data
{
    public class myDBContext : DbContext
    {
        public myDBContext(DbContextOptions<myDBContext> options) : base(options) 
        {
            
        }

        //add table
        DbSet<ConnectionString> dbConnectionString { get; set; }

    }
}
