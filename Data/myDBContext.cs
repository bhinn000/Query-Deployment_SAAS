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
        public DbSet<ConnectionStringEnt> COMPANY_DATABASE_INFO { get; set; }



    }
}

//public abstract class AppDBContext<T> : DbContext where T : DbContext
//{
//    public AppDBContext(DbContextOptions<T> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<User> User { get; set; }
//    ...
//}

//public class AdminDbContext : AppDBContext<AdminDbContext>
//{
//    public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
//    {
//    }
//}

//public class UserDbContext : AppDBContext<UserDbContext>
//{
//    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
//    {
//    }
//}