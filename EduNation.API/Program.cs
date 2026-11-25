using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EduNation.API.Exceptions;
using System.Text;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.CreateStudent;
using EduNation.Application.Courses;
using EduNation.Application.DeactivateStudent;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.Attendances;
using EduNation.Application.CourseSections;
using EduNation.Application.Enrollments;
using EduNation.Application.Grades;
using EduNation.Application.SearchAttendances;
using EduNation.Application.SearchTeachers;
using EduNation.Application.Teachers;
using EduNation.Application.GetAllStudents;
using EduNation.Application.SearchStudents;
using EduNation.Application.GetStudentById;
using EduNation.Application.UpdateStudent;
using EduNation.Infrastructure.Persistence;
using EduNation.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "EduNationDevelopmentKey_ChangeMe_1234567890!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EduNation";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EduNation";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<EduNation.Application.Abstractions.Tenancy.ITenantContext, EduNation.API.Security.TenantContext>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAcademicPeriodRepository, AcademicPeriodRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ICourseSectionRepository, CourseSectionRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<CreateCourseUseCase>();
builder.Services.AddScoped<GetCourseByIdUseCase>();
builder.Services.AddScoped<GetAllCoursesUseCase>();
builder.Services.AddScoped<UpdateCourseUseCase>();
builder.Services.AddScoped<DeactivateCourseUseCase>();
builder.Services.AddScoped<CreateAcademicPeriodUseCase>();
builder.Services.AddScoped<GetAcademicPeriodByIdUseCase>();
builder.Services.AddScoped<GetAllAcademicPeriodsUseCase>();
builder.Services.AddScoped<UpdateAcademicPeriodUseCase>();
builder.Services.AddScoped<DeactivateAcademicPeriodUseCase>();
builder.Services.AddScoped<CreateEnrollmentUseCase>();
builder.Services.AddScoped<GetEnrollmentByIdUseCase>();
builder.Services.AddScoped<GetAllEnrollmentsUseCase>();
builder.Services.AddScoped<GetEnrollmentsByStudentUseCase>();
builder.Services.AddScoped<GetEnrollmentsByStudentIdUseCase>();
builder.Services.AddScoped<GetEnrollmentsByCourseSectionUseCase>();
builder.Services.AddScoped<GetEnrollmentsByCourseSectionIdUseCase>();

builder.Services.AddScoped<CancelEnrollmentUseCase>();
builder.Services.AddScoped<CreateTeacherUseCase>();
builder.Services.AddScoped<GetTeacherByIdUseCase>();
builder.Services.AddScoped<GetAllTeachersUseCase>();
builder.Services.AddScoped<UpdateTeacherUseCase>();
builder.Services.AddScoped<DeactivateTeacherUseCase>();
builder.Services.AddScoped<SearchTeachersUseCase>();
builder.Services.AddScoped<CreateCourseSectionUseCase>();
builder.Services.AddScoped<GetCourseSectionByIdUseCase>();
builder.Services.AddScoped<GetAllCourseSectionsUseCase>();
builder.Services.AddScoped<GetCourseSectionsUseCase>();
builder.Services.AddScoped<GetCourseSectionsByCourseUseCase>();
builder.Services.AddScoped<GetCourseSectionsByCourseIdUseCase>();
builder.Services.AddScoped<GetCourseSectionsByAcademicPeriodUseCase>();
builder.Services.AddScoped<GetCourseSectionsByAcademicPeriodIdUseCase>();
builder.Services.AddScoped<GetCourseSectionsByTeacherUseCase>();
builder.Services.AddScoped<GetCourseSectionsByTeacherIdUseCase>();
builder.Services.AddScoped<UpdateCourseSectionUseCase>();
builder.Services.AddScoped<DeactivateCourseSectionUseCase>();
builder.Services.AddScoped<CreateAttendanceUseCase>();
builder.Services.AddScoped<GetAttendanceByIdUseCase>();
builder.Services.AddScoped<GetAttendanceByStudentUseCase>();
builder.Services.AddScoped<GetAttendanceByCourseSectionAndDateUseCase>();
builder.Services.AddScoped<UpdateAttendanceUseCase>();
builder.Services.AddScoped<SearchAttendancesUseCase>();
builder.Services.AddScoped<CreateEvaluationUseCase>();
builder.Services.AddScoped<GetEvaluationByIdUseCase>();
builder.Services.AddScoped<GetEvaluationsByCourseSectionUseCase>();
builder.Services.AddScoped<UpdateEvaluationUseCase>();
builder.Services.AddScoped<DeactivateEvaluationUseCase>();
builder.Services.AddScoped<CreateGradeUseCase>();
builder.Services.AddScoped<GetGradeByIdUseCase>();
builder.Services.AddScoped<GetGradesByStudentUseCase>();
builder.Services.AddScoped<GetGradesByEvaluationUseCase>();
builder.Services.AddScoped<GetGradesByCourseSectionUseCase>();
builder.Services.AddScoped<UpdateGradeUseCase>();
builder.Services.AddScoped<SearchGradesUseCase>();
builder.Services.AddScoped<GetStudentSectionResultUseCase>();
builder.Services.AddScoped<CreateStudentUseCase>();
builder.Services.AddScoped<GetStudentByIdUseCase>();
builder.Services.AddScoped<GetAllStudentsUseCase>();
builder.Services.AddScoped<SearchStudentsUseCase>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
