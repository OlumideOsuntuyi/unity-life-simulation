namespace Simulation
{
    public interface IComponent
    {
        void Start();
        void Update();
        void OnDestroy();
        void SetGameObject(uint hash);
    }
}