using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();


app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers()    ;
app.Run();

 