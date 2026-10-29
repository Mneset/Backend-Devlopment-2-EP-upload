using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using ClinicAPI.Data;
using ClinicAPI.Models.Config;
using ClinicAPI.Services.AppointmentCategories;
using ClinicAPI.Services.Appointments;
using ClinicAPI.Services.Auth;
using ClinicAPI.Services.Clinics;
using ClinicAPI.Services.Doctors;
using ClinicAPI.Services.Patients;
using ClinicAPI.Services.Specialities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Clinic API",
        Description = "An ASP.NET Core Web API to manage clinics, doctors, patients and appointments.",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste your JWT token here. Do not include the word 'Bearer'.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(openApiDocument =>
    {
        var bearerSchemeReference =
            new OpenApiSecuritySchemeReference("bearer", openApiDocument);

        var securityRequirement = new OpenApiSecurityRequirement();

        securityRequirement[bearerSchemeReference] = new List<string>();

        return securityRequirement;
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddDbContext<ClinicDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IClinicService, ClinicService>();

builder.Services.AddScoped<ISpecialityService, SpecialityService>();

builder.Services.AddScoped<IAppointmentCategoryService, AppointmentCategoryService>();

builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<IAuthService, AuthService>();


// JWT
// Configure JWT settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

builder.Services.AddSingleton(jwtSettings);

builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            )
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    if (db.Database.IsRelational())
        db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "doc/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "doc";
        options.SwaggerEndpoint("/doc/v1/swagger.json", "Clinic API v1");
    });
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
