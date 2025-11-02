using SistemaDeNominas.Server.Data;
using System;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Connection string (LocalDB - Visual Studio)


// Configura los controladores para que NO usen camelCase en la serialización de JSON.
// Esto hace que las propiedades PascalCase de C# (ej: Nombre) se mantengan como
// PascalCase en el JSON (ej: "Nombre"), coincidiendo con lo que espera el frontend de Vue.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
"Server=(localdb)\\MSSQLLocalDB;Database=Nomina;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
