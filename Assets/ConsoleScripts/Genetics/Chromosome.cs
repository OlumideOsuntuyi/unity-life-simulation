namespace Simulation
{
    [System.Serializable]
    public struct Chromosome
    {
        public Type type;
        public Chromosome(Type type)
        {
            this.type = type;   
        }
        public static Chromosome Combne(Chromosome a, Chromosome b)
        {
            return new Chromosome(Math.RandomInt(1, 2) == 1 ? a.type : b.type);
        }
        public static Chromosome Rand()
        {
            return new Chromosome(Math.RandomInt(1, 2) == 1 ? Type.XX : Type.XY);
        }
        public enum Type {XX, XY};
    }
}