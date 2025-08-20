using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TM.BLL.GoogleDriveService;
using FluentValidation;
using TM.DAL;
using AutoMapper;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities;
using TM.DAL.Entities.AppEntities;
using FluentValidation.AspNetCore;
using TM.BLL.Utilities.ValidationRules;
using Dtos;
using TM.BLL.Mappings;

var builder = WebApplication.CreateBuilder(args);

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




builder.Services.AddScoped<ITaskRepository, UserTaskRepository>();
builder.Services.AddScoped<ITaskVersionRepository, TaskVersionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

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
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
