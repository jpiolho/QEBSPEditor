using System.Diagnostics;

namespace QEBSPEditor.Core.Utilities
{
    public class BenchmarkScope : IDisposable
    {
        private Stopwatch _sw;
        private string _name;
        private BenchmarkScope? _parent;

        public BenchmarkScope(string name)
        {
            _name = name;
            _sw = Stopwatch.StartNew();
        }

        private BenchmarkScope(string name, BenchmarkScope parent) : this(name)
        {
            _parent = parent;
        }

        public void Dispose()
        {
            _sw.Stop();

            Console.WriteLine($"{_name}: {_sw.Elapsed}");
        }
    }
}
