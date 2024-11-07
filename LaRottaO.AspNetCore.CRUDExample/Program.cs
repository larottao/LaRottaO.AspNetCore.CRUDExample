using LaRottaO.AspNetCore.CRUDExample;
using LaRottaO.AspNetCore.CRUDExample.Context;
using LaRottaO.AspNetCore.CRUDExample.Interfaces;
using LaRottaO.AspNetCore.CRUDExample.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//*****************************************************************//
// Avoids swagger showing unnecessary data
//*****************************************************************//

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();  // Enable Swagger annotations support
});

builder.Services.AddControllers()
    .AddJsonOptions(optionsJson =>
    {
        optionsJson.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        optionsJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//*****************************************************************//
// Add every new Inteface and Service you create
//*****************************************************************//

builder.Services.AddScoped<ICollaboratorData, CollaboratorDataService>();

//*****************************************************************//
// A must for Entity Framework
//*****************************************************************//

builder.Services.AddDbContext<RepositoryContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Verbose()
           .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
           .WriteTo.Debug()
           .WriteTo.File(GlobalVariables.PATH_LOGGING_FILE, rollingInterval: RollingInterval.Day)
           .CreateLogger();

app.Run();