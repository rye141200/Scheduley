using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Scheduley.Core.Contracts;

namespace Scheduley.Benchmarks.AuthBenchmarks;

// Enable memory diagnostics to see memory allocation
[MemoryDiagnoser]
// Fix the SimpleJob attribute by using the proper constructor syntax
[SimpleJob(RuntimeMoniker.Net80)]
// Configure columns to display detailed memory statistics
[Config(typeof(DetailedMemoryConfig))]
public class AuthBenchmark
{
    private WebApplicationFactory<API.Program> _factory;
    private HttpClient _client;
    private IServiceScope _scope;
    private IUserCacheService _cacheService;

    // Your pre-issued token
    private const string AuthToken =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFobWFkLm1oZnoxNDEyQGdtYWlsLmNvbSIsInVuaXF1ZV9uYW1lIjoiQWhtYWQgTWFoZm91eiIsInJvbGUiOiJCYXNpYyIsIkNyZWF0ZWRBdCI6IjQvMTQvMjAyNSIsIkxhc3RMb2dpbiI6Ijc6MDY6MDIgUE0iLCJuYmYiOjE3NDQ2NTc1NjIsImV4cCI6MTc3NjE5MzU2MiwiaWF0IjoxNzQ0NjU3NTYyfQ.3M8mLeRIIXHsUikzwWRPOuh4VTjdlQNzuRBGJHv6S9M";

    private const string TestEmail = "ahmad.mhfz1412@gmail.com";

    [GlobalSetup]
    public void Setup()
    {
        // Create a test server with the application
        _factory = new WebApplicationFactory<API.Program>();

        // Create a scope to resolve scoped services
        _scope = _factory.Services.CreateScope();

        // Get cache service from the scoped service provider
        _cacheService = _scope.ServiceProvider.GetRequiredService<IUserCacheService>();

        // Clear cache before tests
        _cacheService.ClearCacheAsync().GetAwaiter().GetResult();

        _client = _factory.CreateClient();

        Console.WriteLine($"Client base address: {_client.BaseAddress}");

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            AuthToken
        );

        Console.WriteLine($"Client base address: {_client.BaseAddress}");
    }

    [Benchmark(Description = "First request - cold cache")]
    public async Task ProtectedDummyEndpoint_ColdCache()
    {
        // Clear cache first to ensure a cache miss
        await _cacheService.ClearCacheAsync();

        // Execute the benchmark against the protected endpoint
        var response = await _client.GetAsync("/api/account");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    }

    [Benchmark(Description = "Second request - warm cache")]
    public async Task ProtectedDummyEndpoint_WarmCache()
    {
        // This request should hit the cache
        var response = await _client.GetAsync("/api/account");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
    }

    [Benchmark(Description = "Multiple requests")]
    public async Task ProtectedDummyEndpoint_MultipleIterations()
    {
        // Clear cache first
        await _cacheService.ClearCacheAsync();

        // Simulate multiple users/requests
        for (int i = 0; i < 10; i++)
        {
            var response = await _client.GetAsync("/api/account");
            response.EnsureSuccessStatusCode();
        }
    }

    [Benchmark]
    public async Task ListUserFiles()
    {
        (await _client.GetAsync("/api/storage/files")).EnsureSuccessStatusCode();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _cacheService?.ClearCacheAsync().GetAwaiter().GetResult();
        _scope?.Dispose();
        _client?.Dispose();
        _factory?.Dispose();
    }
}

// Custom config to show detailed memory stats
public class DetailedMemoryConfig : ManualConfig
{
    public DetailedMemoryConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        // Display Gen 0, Gen 1, and Gen 2 GC collections
        WithOption(ConfigOptions.JoinSummary, true);
        // Add this line to ensure summary is shown even with errors
        WithOption(ConfigOptions.StopOnFirstError, false);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<AuthBenchmarks.AuthBenchmark>();
    }
}
