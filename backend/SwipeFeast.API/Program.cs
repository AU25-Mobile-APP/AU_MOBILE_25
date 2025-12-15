using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.OpenApi.Models;
using SwipeFeast.API.Services;
using System.Reflection;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SwipeFeast.API.Models;
using SwipeFeast.API.Services.Exceptions;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "_corsPolicy";
// Setup for JWT Checking middleware
var projectId = builder.Configuration["FIREBASE:PROJECT_ID"] ?? "au-25-mobile-auth";
// Setup Firestore connection
var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations", "serviceAccountKey.json");
System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

var firebaseApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.GetApplicationDefault()
});



// Add services to the container.
builder.Services.AddSingleton(firebaseApp);
builder.Services.AddSingleton(FirebaseAuth.GetAuth(firebaseApp));
builder.Services.AddSingleton<FirestoreDb>(provider => FirestoreDb.Create(projectId));
builder.Services.AddSingleton<IAuthService, AuthService>();
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.Authority = $"https://securetoken.google.com/{projectId}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/{projectId}",
        ValidateAudience = true,
        ValidAudience = projectId,
        ValidateLifetime = true,
    };
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
        
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter either `Bearer {token}` or the raw token.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new string[] { }
            }
        });
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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SwipeFeast.API V1");

        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.UseCors(corsPolicy);

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
