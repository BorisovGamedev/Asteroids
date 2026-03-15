using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UfoView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
        
        public float DebugRadius { get; set; }

        private void OnDrawGizmos()
        {
            if (!PhysicsDebugger.IsEnabled) return;

            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            
            Gizmos.DrawSphere(transform.position, DebugRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DebugRadius);
        }
    }
}