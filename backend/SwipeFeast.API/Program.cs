using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.OpenApi.Models;
using SwipeFeast.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "_corsPolicy";

// Add services to the container.
builder.Services.AddSingleton<IGoogleService>(new GoogleService());
builder.Services.AddSingleton<IGroupService>(provider => new GroupService(provider.GetService<IGoogleService>()));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins(
          "http://localhost:5173",
          "http://localhost" // TODO: Add Prod Frontend Host
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add Cookie Policy Service to the container
builder.Services.AddCookiePolicy(options => {
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.None;
    options.Secure = CookieSecurePolicy.None; //is set to None for development purposes
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "SwipeFeast API", 
            Version = "v1",
            Description = "API backend for the SwipeFeast App",
            Contact = new OpenApiContact
            {
                Name = "informatigger",
                Email = "informatigger@zhaw.ch"
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json","SwipeFeast.API V1"));
}

app.UseCors(corsPolicy);

app.UseCookiePolicy();

app.UseAuthorization();

app.MapControllers();

app.Run();
