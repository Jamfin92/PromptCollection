using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

// Reference to old project
using OldProject.Services;

// Reference to new project
using NewProject.Services;

namespace UpgradeBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<UpgradeBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class UpgradeBenchmarks
    {
        private OldProject.Services.SampleService _oldService;
        private NewProject.Services.SampleService _newService;

        [GlobalSetup]
        public void Setup()
        {
            _oldService = new OldProject.Services.SampleService();
            _newService = new NewProject.Services.SampleService();
        }

        [Benchmark(Baseline = true)]
        public async Task OldVersionMethod()
        {
            await _oldService.PerformOperation();
        }

        [Benchmark]
        public async Task NewVersionMethod()
        {
            await _newService.PerformOperation();
        }
    }
}
