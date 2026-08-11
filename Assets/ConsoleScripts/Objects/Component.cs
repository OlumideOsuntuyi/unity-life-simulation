namespace Simulation
{
    public class Component : Behaviour , IComponent
    {
        public Component()
        {
            
        }
        public virtual void Start()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void OnDestroy()
        {

        }
        void IComponent.SetGameObject(uint hash)
        {
            this.hash = hash;
        }
    }
}