using Serilog;

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/mobileapp-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting mobile app logger API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // IMPORTANT: Updated CORS for your live site and local testing
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("MobileAppPolicy", policy =>
        {
            // FOR PRODUCTION - Specific origins (use this when deploying)
            policy.WithOrigins(
                "https://connect.cci-digital.cloud",  // Your live WordPress site
                "http://localhost:3000",               // React dev server
                "http://localhost:5000",                // Local API
                "http://10.19.132.7:5000"               // Your local IP for testing
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Important if you need to send cookies/auth

            // FOR TESTING ONLY - Comment the above and uncomment below if you have issues
            // policy.AllowAnyOrigin()
            //       .AllowAnyMethod()
            //       .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // In production, you might want HTTPS redirection
        app.UseHttpsRedirection();
    }

    // IMPORTANT: CORS must be called before other middleware
    app.UseCors("MobileAppPolicy");
    app.UseAuthorization();
    app.MapControllers();

    // Log that the app has started successfully
    Log.Information("Mobile app logger API started successfully");
    Log.Information("Listening on: http://localhost:5000 and http://10.19.132.7:5000");
    Log.Information("CORS configured for: https://connect.cci-digital.cloud");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}