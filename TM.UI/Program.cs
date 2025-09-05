using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json.Serialization;
using TM.BLL.Services.DocumentService;
using TM.BLL.Services.GoogleDriveService;
using TM.BLL.Services.TaskService;
using TM.BLL.Services.UserService;
using TM.BLL.Services.VersionService;
using TM.BLL.Utilities.ValidationRules;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(
            RelationalEventId.PendingModelChangesWarning,
            RelationalEventId.MultipleCollectionIncludeWarning
        ));
});

// CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policyBuilder =>
                      {
                          policyBuilder.WithOrigins("https://localhost:4200", "http://localhost:4200")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials();
                      });
});

// Repository & Service Registrations
builder.Services.AddScoped<ITaskRepository, UserTaskRepository>();
builder.Services.AddScoped<ITaskVersionRepository, TaskVersionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

// Controllers + JSON + FluentValidation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<LoginDtoValidator>();
    });

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
app.MapControllers();

app.Run();
