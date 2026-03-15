using UnityEngine;

namespace Asteroids.Physics
{
    public class PhysicsDebugger : MonoBehaviour
    {
        [Header("Настройки отладки физики")]
        [Tooltip("Включить отображение радиусов коллизий")]
        public bool showCollisionRadius = true;

        public static bool IsEnabled;

        private void Update()
        {
            IsEnabled = showCollisionRadius;
        }
    }
}