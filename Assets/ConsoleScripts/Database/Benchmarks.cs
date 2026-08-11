namespace Simulation
{
    [System.Serializable]
    public class Benchmarks
    {
        public static Benchmarks Instance;
        public GenePotential benchmark;
        public static GenePotential Benchmark => Instance.benchmark;
        public static GenePotential InverseBenchmark;
        public Benchmarks(GenePotential potential)
        {
            Instance = this;
            InverseBenchmark = new();
            this.benchmark = potential;
            var props = GenePotential.Properties();
            foreach(var p in props)
            {
                float value = potential.Get(p);
                if(value == 0)
                {
                    continue;
                }
                GenePotential.Set(p, 1 / value, ref InverseBenchmark);
            }
        }
    }
}