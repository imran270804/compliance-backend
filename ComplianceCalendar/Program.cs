using ComplianceCalendar.Data;
using ComplianceCalendar.Endpoints;
using ComplianceCalendar.MappingConfig;
using ComplianceCalendar.Repositories;
using ComplianceCalendar.Repository;
using ComplianceCalendar.Repository.IRepository;
using ComplianceCalendar.Services;
using ComplianceCalendar.Services.EmailService;
using ComplianceCalendar.Services.EMailService;
using ComplianceCalendar.Services.Scheduler;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); 


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IFilingsRepository, FilingsRepository>();
builder.Services.AddScoped<IFilingSchedulerRepository, FilingSchedulerRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmailRepository, EMailRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUploadFilingsRepository, UploadFilingsRepository>();

builder.Services.AddScoped<IPendingFilingsService, PendingFilingsService>();
builder.Services.AddScoped<IDocNotUploadedService, DocNotUploadedService>();
builder.Services.AddScoped<IFilingMailService, FilingMailService>();
builder.Services.AddScoped<IFilingsService, FilingsService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Register your jobs
builder.Services.AddTransient<FilingJob>();
builder.Services.AddTransient<UpdateFilingStatusJob>();
builder.Services.AddTransient<PendingFilingsJob>();
builder.Services.AddTransient<DocNotUploadedJob>();

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Get email settings from .env file
var emailUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
var emailHost = Environment.GetEnvironmentVariable("EMAIL_HOST");
var emailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT"));

// Configure email settings using Options pattern
builder.Services.Configure<EmailSettings>(options =>
{
    options.SenderEmail = emailUsername;
    options.SenderPassword = emailPassword;
    options.SmtpServer = emailHost;
    options.SmtpPort = emailPort;
});

// Get connection string from .ENV file
var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");

builder.Services.AddDbContext<APIContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddDbContext<AdminContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddDbContext<ManageUserContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});


// Add Quartz services
builder.Services.AddSingleton<IJobFactory, SchedulerJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
/*builder.Services.AddScoped<IJob, FilingJob>();*/

// Define job schedules
builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(FilingJob),
    cronExpression: "0 30 16 * * ?"));

builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(UpdateFilingStatusJob),
    cronExpression: "0 00 00 * * ?"));

builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(PendingFilingsJob),
    cronExpression: "0 28 17 * * ?"));

builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(DocNotUploadedJob),
    cronExpression: "0 34 16 * * ?"));

builder.Services.AddHostedService<QuartzHostedService>();

// Configure CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificOrigin",
        builder => builder
            .WithOrigins('https://compliance-calendar-hosting-kkph.vercel.app')
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("SpecificOrigin");
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.MapDepartmentEndpoints();
app.MapFilingEndpoints();
app.MapAuthenticationEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
