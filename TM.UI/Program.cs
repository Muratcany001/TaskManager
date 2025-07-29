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
                      policyBuilder =>
                      {
                          policyBuilder.WithOrigins("https://localhost:4200", "http://localhost:4200" , "localhost:4200")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod().AllowCredentials();
                      });
});

// Repository bağımlılıkları
builder.Services.AddScoped<ITaskRepository, UserTaskRepository>();
builder.Services.AddScoped<ITaskVersionRepository, TaskVersionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Controller ve JSON ayarları
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

// Swagger dev ortamında aktif
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🟡 CORS politikası burada aktif edilmeli!
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
