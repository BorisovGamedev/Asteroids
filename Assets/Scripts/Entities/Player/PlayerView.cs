using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
        
        [SerializeField] private ParticleSystem _shieldParticles;
        public ParticleSystem ShieldParticles => _shieldParticles;
        
        [SerializeField] private LineRenderer _laserLine;
        public LineRenderer LaserLine => _laserLine;

        public float DebugRadius { get; set; }

        private void OnDrawGizmos()
        {
            if (!PhysicsDebugger.IsEnabled) return;

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            
            Gizmos.DrawSphere(transform.position, DebugRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, DebugRadius);
        }
    }
}