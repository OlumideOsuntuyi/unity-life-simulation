namespace Simulation
{
    public class Behaviour
    {
        protected uint hash;
        public SimObject gameObject => SimObject._objects[hash];
        public Transform transform => gameObject.transform;
    }
}