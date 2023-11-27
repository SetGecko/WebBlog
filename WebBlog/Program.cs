using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    //подключаем логирование
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    //добавить контекст БД
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
    //Добавить фиксацию и вывод исключений связанных с базой данных
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // регистрация сервисов репозиториев для взаимодействия с базой данных
    builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();


    // подключаем набор служб удостоверений . по умолчанию также будут добавлены пользоваетельские формы
    //и настроена проверка подлинности для использования cookie
    builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<BlogRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddScoped<UserManager<BlogUser>>();

    //Добавляем полититики для  администратора, модератора и пользователя
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
                // Резервная политика авторизации:
                // Применяется ко всем запросам, которые явно не указывают политику авторизации.
                // Для запросов, обслуживаемых маршрутизацией конечных точек, сюда входит любая конечная точка, не указывающая атрибут авторизации.
                // Для запросов, обслуживаемых другим ПО промежуточного слоя после ПО промежуточного слоя авторизации,
                // например статических файлов, эта политика применяется ко всем запросам.
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

    //подключение слабосвязанных сущностей, которые легко тестировать, модифицировать и обновлять. 
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IUserService, UserService>();

    //изменияем стандартные настройки на попроще
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
        //подключаем страницу с сообщением о запрете доступа к ресурсу
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

    // Подключаем автомаппинг
    builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

    //подключаем фильтр для логирования действий пользователя
    builder.Services.AddScoped<UserActivityFilter>();

    //добавить контроллеры с фильтром действий пользователя
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<UserActivityFilter>();
    });
    //подключаем swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(name: "v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApiSimpleBlog", Version = "v1" });
    });



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        //подключаем обработчик глобальных исключений и перекидываем на страницу что то пошло не так
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }



    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    //подключаем страничку на код 404
    app.UseStatusCodePagesWithReExecute("/Home/Error404", "?statusCode={0}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();



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

public partial class Program { }


