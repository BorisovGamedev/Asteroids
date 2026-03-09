using Cysharp.Threading.Tasks;

namespace Asteroids.Configs
{
    public interface IConfigProvider
    {
        PlayerConfig Player { get; }
        EnemiesConfig Enemies { get; }
        WorldConfig World { get; }

        UniTask LoadAllConfigsAsync();
    }
}