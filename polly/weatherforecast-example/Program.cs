using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

int times = 0;

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

    switch (times)
    {
        case 0:
            times++;
            throw new Exception("example exception 1");
        case 1:
            times++;
            throw new Exception("example exception 2");
        default:
            return forecast;
    }
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/polly-example", () =>
{
    HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5253"),
    };

    return client.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
})
.WithName("PollyExample")
.WithOpenApi();

app.MapGet("/polly-execution-retry", () =>
{
    HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5253"),
    };

    AsyncRetryPolicy policy = Policy.Handle<Exception>()
                                    .WaitAndRetryAsync(3, retryTime => TimeSpan.FromMilliseconds(800 * retryTime));

    return policy.ExecuteAsync(async () => await client.GetFromJsonAsync<WeatherForecast[]>("weatherforecast"));
})
.WithName("PollyExampleRetry")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
