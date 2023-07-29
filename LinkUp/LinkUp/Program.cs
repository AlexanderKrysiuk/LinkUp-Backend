using LinkUp.Services.Contractors;
using LinkUp.Services.Contractors.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IContractorService, ContractorService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();
app.Run();