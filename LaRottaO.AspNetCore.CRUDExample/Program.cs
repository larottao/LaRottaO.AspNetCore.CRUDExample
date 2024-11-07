using LaRottaO.AspNetCore.CRUDExample;
using LaRottaO.AspNetCore.CRUDExample.Context;
using LaRottaO.AspNetCore.CRUDExample.Interfaces;
using LaRottaO.AspNetCore.CRUDExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//*****************************************************************//
// Configure Swagger to use the Bearer Token
//*****************************************************************//

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();  // Enable Swagger annotations support

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field like: 'Bearer <your_jwt_token>'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(optionsJson =>
    {
        optionsJson.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        optionsJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//*****************************************************************//
// Add Interfaces and Services
//*****************************************************************//

builder.Services.AddScoped<ICollaboratorData, CollaboratorDataService>();

//*****************************************************************//
// A must for Entity Framework
//*****************************************************************//

builder.Services.AddDbContext<RepositoryContext>();

//*****************************************************************//
// JWT Specific code (Authentication setup)
//*****************************************************************//

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Ensure this matches the issuer in the token
        ValidAudience = builder.Configuration["Jwt:Audience"],  // Ensure this matches the audience in the token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  // Ensure this matches the key used to sign the token
    };
});

//*****************************************************************//
// Add Logging (Serilog configuration)
//*****************************************************************//

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Verbose()
           .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
           .WriteTo.Debug()
           .WriteTo.File(GlobalVariables.PATH_LOGGING_FILE, rollingInterval: RollingInterval.Day)
           .CreateLogger();

//*****************************************************************//
// Build and configure the application pipeline
//*****************************************************************//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ensure that authentication is added before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();