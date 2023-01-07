using Api.Data;
using Api.Data.Entities;
using Api.Features.V1.User;
using Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Settings
var dbSettings = builder.Configuration
    .GetSection(nameof(DbSettings))
    .Get<DbSettings>();
var jwtSettings = builder.Configuration
    .GetSection(nameof(JwtSettings))
    .Get<JwtSettings>();

if (dbSettings == null)
    throw new Exception("DbSettings cannot be null");
if (jwtSettings == null)
    throw new Exception("JwtSettings cannot be null");

builder.Services.AddSingleton<JwtSettings>(jwtSettings);

// Add controllers
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(dbSettings.ConnectionString);
    options.UseSnakeCaseNamingConvention();
    options.LogTo(Console.WriteLine);
});

// Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddSignInManager<SignInManager<AppUser>>()
.AddUserManager<UserManager<AppUser>>()
.AddDefaultTokenProviders();

// Services
builder.Services.AddScoped<IUserService, UserService>();

// Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
