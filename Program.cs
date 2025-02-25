using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Data;
using Serilog;

    var builder = WebApplication.CreateBuilder(args);

File.WriteAllText("C:\\Hello\\myapp.txt", string.Empty);
builder.Host.UseSerilog((context, configuration)=>
    configuration.ReadFrom.Configuration(context.Configuration)
);

// Add services to the container.

builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //built in and link to the connection string too
    builder.Services.AddDbContext<MyDBContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));  
        }
        );

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
