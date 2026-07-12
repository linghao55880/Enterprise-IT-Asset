using ITAssetManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register MockDataService as Singleton
builder.Services.AddSingleton<MockDataService>();

// Configure CORS for frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseAuthorization();

// Health check endpoint for Railway
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// API controllers
app.MapControllers();

// SPA fallback: all unmatched routes go to index.html
app.MapFallbackToFile("index.html");

app.Run();
