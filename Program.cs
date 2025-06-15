using Microsoft.EntityFrameworkCore;
using SAAS_Query_API.Controllers;
using SAAS_Query_API.Data;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

File.WriteAllText("C:\\Hello\\myapp.txt", string.Empty); // to overwrite the file
    builder.UseSerilog((context, configuration)=>
        configuration.ReadFrom.Configuration(context.Configuration)
    );

builder.ConfigureServices((context, services) =>
{
    services.AddScoped<QueriesExecution>();
    services.AddDbContext<MyDBContext>(
        options =>
        {
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
        }
        );
});

var app = builder.Build();

var controller = app.Services.GetRequiredService<QueriesExecution>();
await controller.GetConnectionString();

app.Run();

