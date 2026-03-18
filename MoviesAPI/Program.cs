using Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MoviesAPI.Auth;
using MoviesAPI.Middlewares;
using MoviesAPI.Options;
using MoviesAPI.WebClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using MoviesAPI.Background;
using Serilog;
using Infrastructure.SQS.Factories;
using Infrastructure.SQS.Services;
using MoviesAPI.Extensions;
using Infrastructure.SQS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CinemaDbContext>(options =>
{
	options.UseInMemoryDatabase("CinemaDb")
			.EnableSensitiveDataLogging()
			.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});
builder.Services.InitializeMockDatabase();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<IIMDBWebApiClient, IMDBWebApiClient>();
builder.Services.RegisterOptions<IMDBWebApiClientOptions>(builder.Configuration.GetSection("IMDBWebApiClient"));

builder.Services.AddScoped<ISQSService, SQSService>();
builder.Services.AddSingleton<ISQSClientFactory, SQSClientFactory>();
builder.Services.RegisterOptions<SQSOptions>(builder.Configuration.GetSection("SQS"));

builder.Services.AddSingleton<IMDBStatusProvider>();

builder.Services.AddMediator(x => x.ServiceLifetime = ServiceLifetime.Scoped);

builder.Services.AddSingleton<ICustomAuthenticationTokenService, CustomAuthenticationTokenService>();
builder.Services.AddAuthentication(options =>
{
	options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationSchemeOptions.AuthenticationScheme, CustomAuthenticationSchemeOptions.AuthenticationScheme);
	options.RequireAuthenticatedSignIn = true;
	options.DefaultScheme = CustomAuthenticationSchemeOptions.AuthenticationScheme;
});

builder.Services.AddHostedService<IMDBStatusBackgroundTask>();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

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

app.Run();
