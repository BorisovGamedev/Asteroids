namespace Asteroids.Entities.Enemies.Ufo
{
    public interface IUfoState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}