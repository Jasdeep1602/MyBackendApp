using MyBackendApp.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel with custom ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
    options.ListenAnyIP(7195, listenOptions => listenOptions.UseHttps()); // HTTPS
});

// Add CORS policy to allow all origins, methods, and headers for development purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<ListItemService>();

// Configure OpenAPI if needed
builder.Services.AddOpenApi();

var app = builder.Build();

// Use CORS in the request pipeline
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Weather Forecast endpoint
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "Welcome to My Backend App!");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// List Items endpoints
app.MapGet("/list-items", async (ListItemService service) =>
    await service.GetAllAsync());

app.MapGet("/list-items/{id}", async (string id, ListItemService service) =>
    await service.GetAsync(id) is ListItem item
        ? Results.Ok(item)
        : Results.NotFound());

app.MapPost("/list-items", async (ListItem newItem, ListItemService service) =>
{
    await service.CreateAsync(newItem);
    return Results.Created($"/list-items/{newItem.Id}", newItem);
});

app.MapPut("/list-items/{id}", async (string id, ListItem updatedItem, ListItemService service) =>
{
    var existingItem = await service.GetAsync(id);
    if (existingItem is null) return Results.NotFound();

    updatedItem.Id = id;
    await service.UpdateAsync(id, updatedItem);
    return Results.NoContent();
});

app.MapDelete("/list-items/{id}", async (string id, ListItemService service) =>
{
    var existingItem = await service.GetAsync(id);
    if (existingItem is null) return Results.NotFound();

    await service.DeleteAsync(id);
    return Results.NoContent();
});

// Run the app
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
