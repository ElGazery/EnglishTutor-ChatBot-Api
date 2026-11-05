using EnglishTutor.Api.Extensions;
using EnglishTutor.Api.Middleware;
using EnglishTutor.Application;
using EnglishTutor.Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "English Tutor API",
        Version = "v1",
        Description = "ASP.NET Core Web API for AI-powered English Tutoring ChatBot"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT Bearer token"
    });

    var scheme = new OpenApiSecuritySchemeReference("Bearer");
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 44355;
    });
}

builder.Services.AddCors(options =>
    options.AddPolicy("CorsPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()));

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "English Tutor API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
