using Infrastructure.Auth;
using Infrastructure.Database;
using Infrastructure.SQS;
using Infrastructure.SQS.Factories;
using Infrastructure.SQS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Background;
using MoviesAPI.Extensions;
using MoviesAPI.Handlers;
using MoviesAPI.Middlewares;
using MoviesAPI.Options;
using MoviesAPI.WebClients;
using Serilog;
using System;
using System.Text;

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

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthenticationKey"]))
	};
});
builder.Services.AddAuthorization();

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
