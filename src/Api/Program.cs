using Application.Interfaces;
using Application.UseCases;
using Domain.Repositories;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces.Excel;
using Infrastructure.AI;
using Infrastructure.Excel;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 1. Add Controllers
// -----------------------------------------------------------
builder.Services.AddControllers();

// -----------------------------------------------------------
// 2. DbContext (MySQL)
// -----------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TalentoPlusDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Identity (obligatorio por IdentityDbContext)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TalentoPlusDbContext>()
    .AddDefaultTokenProviders();

// -----------------------------------------------------------
// 3. Application e Infrastructure (repos, servicios, use cases)
// -----------------------------------------------------------
builder.Services.AddScoped<IExcelEmployeeReader, ExcelEmployeeReader>();
builder.Services.AddScoped<ImportEmployeesUseCase>();

builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAiAnalysisService, OpenAiService>();

builder.Services.AddScoped<IEmailService, MailtrapEmailService>();
builder.Services.AddScoped<IAiAnalysisService, OpenAiService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        policy =>
        {
            policy.AllowAnyOrigin() 
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


// -----------------------------------------------------------
// 4. JWT Authentication
// -----------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];


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

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

// -----------------------------------------------------------
// 5. Swagger / OpenAPI
// -----------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TalentoPlus API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Ingresa: Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});


var app = builder.Build();

// -----------------------------------------------------------
// 6. Ejecutar Seeder (programas pre-cargados)
// -----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TalentoPlusDbContext>();
    await ProgramSeeder.SeedAsync(db);
}

// -----------------------------------------------------------
// 7. Pipeline middleware
// -----------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();




// -----------------------------------------------------------
// 8. Map Controllers
// -----------------------------------------------------------
app.MapControllers();

app.Run();