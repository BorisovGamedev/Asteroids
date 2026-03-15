using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AsteroidView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
        
        public float DebugRadius { get; set; }

        private void OnDrawGizmos()
        {
            if (!PhysicsDebugger.IsEnabled) return;

            Gizmos.color = new Color(0f, 0.75f, 0.75f, 0.4f);
            
            Gizmos.DrawSphere(transform.position, DebugRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, DebugRadius);
        }
    }
}