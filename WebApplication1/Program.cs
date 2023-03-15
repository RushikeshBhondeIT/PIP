using Entities;
using Microsoft.EntityFrameworkCore;
using EmployeeServiceContracts;
using EmployeeServicesRepo;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

//Logging
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IEmployeeService, EmployeesServices>();

//Data Source=(localdb)\ProjectModels;Initial Catalog=EmployeeDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

//app.Logger.LogDebug("Debug-message");
//app.Logger.LogInformation("information-message"); 
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("Debug-message");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
