using HackerNews.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IHackerNewsClient, HackerNewsClient>();

builder.Services.AddHttpClient<HackerNewsApi>(client => client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/"));
builder.Services.AddKeyedTransient<IHackerNewsApi, HackerNewsApi>("base");
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IHackerNewsApi, CachedHackerNewsApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/best", (IHackerNewsClient hackerNewsClient, int count = 10) => hackerNewsClient.GetBestAsync(count));

app.Run();
