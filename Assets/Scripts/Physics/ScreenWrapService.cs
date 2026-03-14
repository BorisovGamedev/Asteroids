using Asteroids.Configs;
using UnityEngine;

namespace Asteroids.Physics
{
    public class ScreenWrapService
    {
        private readonly float _worldWidth;
        private readonly float _worldHeight;

        public ScreenWrapService(IConfigProvider configProvider)
        {
            _worldWidth = configProvider.World.WorldWidth;
            _worldHeight = configProvider.World.WorldHeight;
        }

        public void Wrap(CustomPhysicsBody body)
        {
            Vector2 position = body.Position;
            
            float halfWidth = _worldWidth / 2f;
            float halfHeight = _worldHeight / 2f;

            if (position.x > halfWidth)
            {
                position.x = -halfWidth;
            }
            else if (position.x < -halfWidth)
            {
                position.x = halfWidth;
            }

            if (position.y > halfHeight)
            {
                position.y = -halfHeight;
            }
            else if (position.y < -halfHeight)
            {
                position.y = halfHeight;
            }
            
            body.Position = position;
        }
    }
}