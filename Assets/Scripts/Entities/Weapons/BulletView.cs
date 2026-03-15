using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
        
        public float DebugRadius { get; set; }

        private void OnDrawGizmos()
        {
            if (!PhysicsDebugger.IsEnabled) return;

            Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
            
            Gizmos.DrawSphere(transform.position, DebugRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DebugRadius);
        }
    }
}