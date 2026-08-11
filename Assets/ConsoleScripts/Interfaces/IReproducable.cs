using Simulation;

namespace Simulation
{
    public interface IReproducable
    {
        public void Reproduce(IReproducable reproducable);
        public bool CanReproduce(IReproducable reproducable);
        public float Affinity(IReproducable reproducable);
    }
}
