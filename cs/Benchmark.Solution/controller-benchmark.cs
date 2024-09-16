using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;

namespace ControllerBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ControllerBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class ControllerBenchmarks
    {
        private HttpClient _client;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Benchmark]
        public async Task GetEndpoint()
        {
            var response = await _client.GetAsync("/api/sample");
            response.EnsureSuccessStatusCode();
        }

        [Benchmark]
        public async Task PostEndpoint()
        {
            var content = new StringContent("{\"data\": \"test\"}", System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/sample", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
