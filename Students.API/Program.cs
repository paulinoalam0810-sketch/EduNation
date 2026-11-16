using Microsoft.EntityFrameworkCore;
using Students.API.Exceptions;
using Students.Application.Abstractions.Persistence;
using Students.Application.CreateStudent;
using Students.Application.DeactivateStudent;
using Students.Application.GetAllStudents;
using Students.Application.GetStudentById;
using Students.Application.UpdateStudent;
using Students.Infrastructure.Persistence;
using Students.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<CreateStudentUseCase>();
builder.Services.AddScoped<GetStudentByIdUseCase>();
builder.Services.AddScoped<GetAllStudentsUseCase>();
builder.Services.AddScoped<UpdateStudentUseCase>();
builder.Services.AddScoped<DeactivateStudentUseCase>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
