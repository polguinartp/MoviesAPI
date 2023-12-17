using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MoviesAPI.ActionFilters;
using MoviesAPI.Auth;
using MoviesAPI.Exceptions;
using MoviesAPI.Middlewares;
using MoviesAPI.Options;
using MoviesAPI.Services;
using MoviesAPI.WebClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using MoviesAPI.Background;
using Serilog;
using MoviesAPI.Database;
using Infrastructure.Options;
using Infrastructure.SQS.Factories;
using Infrastructure.SQS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CinemaContext>(options =>
{
    options.UseInMemoryDatabase("CinemaDb")
        .EnableSensitiveDataLogging()
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<IIMDBWebApiClient, IMDBWebApiClient>();
builder.Services.AddTransient<IShowtimeService, ShowtimeService>();
builder.Services.AddTransient<IRepository<ShowtimeEntity>, BaseRepository<ShowtimeEntity>>();
builder.Services.AddTransient<IRepository<AuditoriumEntity>, BaseRepository<AuditoriumEntity>>();
builder.Services.AddTransient<IRepository<MovieEntity>, BaseRepository<MovieEntity>>();

builder.Services.AddSingleton<ISQSFactory, SQSFactory>();
builder.Services.AddScoped<ISQSService, SQSService>();
builder.Services.AddScoped<IQueueService, QueueService>();

builder.Services.AddScoped<ShowtimeActionFilter>();

builder.Services.AddSingleton<ICustomAuthenticationTokenService, CustomAuthenticationTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationSchemeOptions.AuthenticationScheme, CustomAuthenticationSchemeOptions.AuthenticationScheme);
    options.RequireAuthenticatedSignIn = true;
    options.DefaultScheme = CustomAuthenticationSchemeOptions.AuthenticationScheme;
});

builder.Services.AddOptions<WebApiClientOptions>().Bind(builder.Configuration.GetSection("WebApiClient"));
builder.Services.AddOptions<SQSOptions>().Bind(builder.Configuration.GetSection("SQS"));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<IMDBStatusBackgroundTask>();
builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggerMiddleware>();
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

SampleData.Initialize(app);

app.Run();
