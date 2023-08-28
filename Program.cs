using LinkUpBackend.Domain;
using LinkUpBackend.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging());

builder.Services.AddIdentity<User, Role>(options =>
                    {
                        options.User.RequireUniqueEmail = true;

                        options.Password.RequireDigit = true;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequiredLength = 8;

                        //options.SignIn.RequireConfirmedEmail = true;

                    }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.Run();

//var roleManager = builder.
//var roles = new List<string> { "Admin", "Moderator", "User" };

//foreach (var role in roles)
//{
//    if (!roleManager.RoleExistsAsync(role).Result)
//    {
//        roleManager.CreateAsync(new IdentityRole(role)).Wait();
//    }
//}
