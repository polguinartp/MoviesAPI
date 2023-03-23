using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI.ActionFilters;
using MoviesAPI.Auth;
using MoviesAPI.Database;
using MoviesAPI.Exceptions;
using MoviesAPI.Middlewares;
using MoviesAPI.Options;
using MoviesAPI.Services;
using MoviesAPI.WebClients;

namespace MoviesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddAutoMapper(typeof(Startup));

            services.AddHttpClient<IIMDBWebApiClient, IMDBWebApiClient>();
            services.AddTransient<IShowtimeService, ShowtimeService>();
            services.AddTransient<IRepository<ShowtimeEntity>, BaseRepository<ShowtimeEntity>>();
            services.AddTransient<IRepository<AuditoriumEntity>, BaseRepository<AuditoriumEntity>>();
            services.AddTransient<IRepository<MovieEntity>, BaseRepository<MovieEntity>>();

            services.AddScoped<ShowtimeActionFilter>();

            services.AddSingleton<ICustomAuthenticationTokenService, CustomAuthenticationTokenService>();

            services.AddAuthentication(options =>
            {
                options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationSchemeOptions.AuthenticationScheme, CustomAuthenticationSchemeOptions.AuthenticationScheme);
                options.RequireAuthenticatedSignIn = true;
                options.DefaultScheme = CustomAuthenticationSchemeOptions.AuthenticationScheme;
            });

            services.AddOptions<WebApiClientOptions>().Bind(Configuration.GetSection("WebApiClient"));

            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RequestLoggerMiddleware>();
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SampleData.Initialize(app);
        }
    }
}
