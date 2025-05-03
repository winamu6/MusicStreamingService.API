using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyClone.API.Data;
using SpotifyClone.API.Repositories.AuthRepositories.AuthRepositoriesInterfaces;
using SpotifyClone.API.Repositories.AuthRepositories;
using SpotifyClone.API.Services.AuthServices;
using SpotifyClone.API.Services.AuthServices.Interfaces;
using SpotifyClone.API.Services.SupabaseStorageServices;
using System.Text;
using SpotifyClone.API.Repositories.AlbumRepositories.AlbumRepositoriesInterfaces;
using SpotifyClone.API.Repositories.AlbumRepositories;
using SpotifyClone.API.Services.AlbumServices.AlbumInterfaces;
using SpotifyClone.API.Services.AlbumServices;
using SpotifyClone.API.Repositories.PlaylistRepositories.PlaylistRepositoriesInterfaces;
using SpotifyClone.API.Repositories.PlaylistRepositories;
using SpotifyClone.API.Services.PlaylistServices.PlaylistInterfaces;
using SpotifyClone.API.Services.PlaylistServices;
using SpotifyClone.API.Repositories.SongRepositories.SongRepositoriesInterfaces;
using SpotifyClone.API.Repositories.SongRepositories;
using SpotifyClone.API.Services.SongServices.SongInterfaces;
using SpotifyClone.API.Models.Entities;
using SpotifyClone.API.Services.SongServices;
using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using SpotifyClone.API.Repositories.LikeRepositories.LikeRepositoriesInterfaces;
using SpotifyClone.API.Repositories.LikeRepositories;
using SpotifyClone.API.Services.LikeServices.LikeInterfaces;
using SpotifyClone.API.Services.LikeServices;
using SpotifyClone.API.Repositories.GenreRepositories.GenreRepositoriesInterfaces;
using SpotifyClone.API.Repositories.GenreRepositories;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
var pythonServiceUrl = builder.Configuration.GetSection("PythonService:BaseUrl").Value;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/myapp.txt")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SpotifyClone.API", Version = "v1" });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "¬ведите токен в формате: Bearer {your JWT token}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("AudioFeatureService", client =>
{
    client.BaseAddress = new Uri(pythonServiceUrl);
});


builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();

builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();

builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

builder.Services.AddScoped<ILikeRepository, LikeRepository>(); 
builder.Services.AddScoped<ILikeService, LikeService>();

builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRoles(roleManager);
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();