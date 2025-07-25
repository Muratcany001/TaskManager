using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities;
using TM.DAL.Entities.AppEntities;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000", // Example: your frontend development server
                                              "https://myproductionapp.com") // Example: your production frontend domain
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                          // .AllowCredentials(); // Uncomment if you need to allow cookies/authentication headers
                      });
});

// Repository baðýmlýlýklarý
builder.Services.AddScoped<ITaskRepository, UserTaskRepository>();
builder.Services.AddScoped<ITaskVersionRepository, TaskVersionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Controller ve JSON ayarlarý
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Development ortamý için Swagger aktif
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
