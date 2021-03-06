﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieWebApp.Data;
using MovieWebApp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieWebApp.Controllers
{


    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IConfiguration _configuration;


        public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                UpdateMoviesController updater = new UpdateMoviesController(context, _configuration);
                List<Movie> movies = new List<Movie>();
                movies = updater.GetTMDbMovieFromApi(movies,"Popular");
                movies = updater.GetTMDbMovieFromApi(movies,"TopRated");
                movies = updater.GetTMDbMovieFromApi(movies,"Upcoming");
                await updater.MovieTableUpdate(movies);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
