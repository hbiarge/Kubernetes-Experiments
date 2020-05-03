using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private const string SummariesKey = "summaries";

        private static readonly string[] DefaultSummaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IMemoryCache _localCache;
        private readonly IDistributedCache _distributedCache;

        public WeatherForecastController(
            IMemoryCache localCache,
            IDistributedCache distributedCache)
        {
            _localCache = localCache;
            _distributedCache = distributedCache;
        }

        [HttpGet("local")]
        public IEnumerable<WeatherForecast> GetLocal()
        {
            // Rich api to work with...
            string[] summaries = _localCache.GetOrCreate(
                key: SummariesKey,
                factory: entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                    return DefaultSummaries;
                });

            return GenerateWeatherForecast(summaries);
        }

        [HttpGet("distributed")]
        public async Task<IEnumerable<WeatherForecast>> GetDistributed()
        {
            // We are going to work with byte[], so we should worry about serialization...
            var summariesString = await _distributedCache.GetStringAsync(SummariesKey);
            string[] summaries;

            if (string.IsNullOrEmpty(summariesString))
            {
                summaries = DefaultSummaries;

                await _distributedCache.SetStringAsync(
                    SummariesKey,
                    JsonSerializer.Serialize(summaries),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
            }
            else
            {
                summaries = JsonSerializer.Deserialize<string[]>(summariesString);
            }

            return GenerateWeatherForecast(summaries);
        }

        private static IEnumerable<WeatherForecast> GenerateWeatherForecast(
            string[] summaries)
        {
            var rng = new Random();
            var weatherForecasts = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = summaries[rng.Next(summaries.Length)]
                });

            return weatherForecasts;
        }
    }
}
