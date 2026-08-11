namespace Simulation
{
    public struct Permission
    {
        public bool canWalk;
        public bool canSwim;
        public bool canFly;
        public bool canGrow;
    }

    public struct DataEntry
    {
        public Kingdom kingdom;
        public Habitat habitat;
    }
    public enum Kingdom { None, Plant, Animal };
    public enum Habitat { Amphibious, Water, Land };
}