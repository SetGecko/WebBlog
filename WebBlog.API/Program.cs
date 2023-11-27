using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Reflection;
using WebBlog;
using WebBlog.BLL.Services;
using WebBlog.BLL.Services.Interfaces;
using WebBlog.DAL;
using WebBlog.DAL.Interfaces;
using WebBlog.DAL.Models;
using WebBlog.DAL.Repositories;
using WebBlog.Extensions;
using WebBlog.Filters;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{

    var builder = WebApplication.CreateBuilder(args);
    //���������� �����������
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


    //�������� �������� ��
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
    //�������� �������� � ����� ���������� ��������� � ����� ������
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // ����������� �������� ������������ ��� �������������� � ����� ������
    builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();


    // ���������� ����� ����� ������������� . �� ��������� ����� ����� ��������� ����������������� �����
    //� ��������� �������� ����������� ��� ������������� cookie
    builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<BlogRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddScoped<UserManager<BlogUser>>();

    //��������� ���������� ���  ��������������, ���������� � ������������
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
                // ��������� �������� �����������:
                // ����������� �� ���� ��������, ������� ���� �� ��������� �������� �����������.
                // ��� ��������, ������������� �������������� �������� �����, ���� ������ ����� �������� �����, �� ����������� ������� �����������.
                // ��� ��������, ������������� ������ �� �������������� ���� ����� �� �������������� ���� �����������,
                // �������� ����������� ������, ��� �������� ����������� �� ���� ��������.
                .RequireAuthenticatedUser()
                .Build();

        options.AddPolicy("RuleOwnerOrAdminOrModerator", policy =>
           policy.RequireAssertion(context =>
                context.User.IsInRole("Administrator") || context.User.IsInRole("Moderator") ||
                context.User.HasClaim(c => c.Type == "ArticleOwner" && c.Value == context.GetRouteValue("id"))));

        options.AddPolicy("RuleAdministratorOrModerator", policy =>
          policy.RequireAssertion(context =>
               context.User.IsInRole("Administrator") || context.User.IsInRole("Moderator")));

        options.AddPolicy("RoleAdministrator", policy => policy.RequireRole("Administrator"));
        options.AddPolicy("RoleModerator", policy => policy.RequireRole("Moderator"));

    });

    //����������� �������������� ���������, ������� ����� �����������, �������������� � ���������. 
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IUserService, UserService>();

    //��������� ����������� ��������� �� �������
    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Password settings.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 3;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings.
        options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = false;
    });

    builder.Services.ConfigureApplicationCookie(options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

        options.LoginPath = "/Identity/Account/Login";
        //���������� �������� � ���������� � ������� ������� � �������
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

    // ���������� �����������
    builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

    //���������� ������ ��� ����������� �������� ������������
    builder.Services.AddScoped<UserActivityFilter>();

    //�������� ����������� � �������� �������� ������������
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<UserActivityFilter>();
    });


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "WebBlog API",
            Description = "ASP.NET Core Web API ��� ���������� ���������� WebBlog",
            TermsOfService = new Uri("http://localhost:5214//terms"),
            Contact = new OpenApiContact
            {
                Name = "Contact",
                Url = new Uri("http://localhost:5214//contact")
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("http://localhost:5214//license")
            }
        });

        // using System.Reflection;
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}

/// <summary>
/// ���������� ������ ��� ������������� � ���� ������
/// </summary>
public partial class Program { }