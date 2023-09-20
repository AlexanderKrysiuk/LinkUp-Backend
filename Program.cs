using LinkUpBackend.Configurations;
using LinkUpBackend.Models;
using LinkUpBackend.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://localhost:5173", "http://localhost:5173") //TODO: check if it's 5173 or 5174!
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging());
//builder.Services.AddDbContext<MeetingsAPIDbContext>(options => options.UseInMemoryDatabase("MeetingsDb"));
//builder.Services.AddDbContext<MeetingsAPIDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>(options =>
                    {
                        options.User.RequireUniqueEmail = true;
                        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ";
                        
                        options.Password.RequireDigit = true;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequiredLength = 8;

                        //options.SignIn.RequireConfirmedEmail = true;

                    }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddOptions<JwtConfiguration>().Bind(builder.Configuration.GetSection(JwtConfiguration.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Authentication:Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["Authentication:Jwt:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Jwt:SigningKey"]!))
            };
        });
builder.Services.AddAuthorization(options =>
{
    var defualtAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme);
    defualtAuthorizationPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = defualtAuthorizationPolicyBuilder.Build();

    //add policies
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Include the JWT bearer token in the Swagger UI requests
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// Add the required Quartz.NET services
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("ArchiveMeetings");
    q.AddJob<ArchiveMeetingsJob>(options => options.WithIdentity(jobKey));
    q.AddTrigger(options => options.ForJob(jobKey).WithIdentity("ArchiveMeetings-trigger").WithCronSchedule("0 * * * * ?"));
});

// Add the Quartz.NET hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
