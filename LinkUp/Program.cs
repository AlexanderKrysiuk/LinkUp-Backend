using LinkUp.Services.Contractors;
using LinkUp.Services.Contractors.Interfaces;
using LinkUp.Services.Clients;
using LinkUp.Services.Clients.Interfaces;
using Microsoft.EntityFrameworkCore;
using LinkUp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins("https://localhost:5173/")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IContractorService, ContractorService>(); 
builder.Services.AddScoped<IClientService, ClientService>();
var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
     app.UseSwagger();
     app.UseSwaggerUI();
    }
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();