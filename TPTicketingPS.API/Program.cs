using TPTicketingPS.Application.Events;
using TPTicketingPS.Application;
using TPTicketingPS.Infrastructure;
using TPTicketingPS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Web API configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ticketing API",
        Version = "v1",
        Description = "API REST para gestión de eventos, reservas y pagos."
    });
});


// Capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

// Seed
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<TPTicketingPS.Infrastructure.Persistence.DbInitializer>();
    await initializer.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(/*options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0*/);
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketing API v1");
    });
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
