using System.Threading;
using Asteroids.Physics;
using Cysharp.Threading.Tasks;
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
        public float DebugRadius { get; set; }

        private CancellationTokenSource _laserCts;

        private void Start()
        {
            _laserLine.enabled = false;
        }

        public async UniTask ShowLaserVisualAsync(Vector2 origin, Vector2 direction, float length, int durationMs)
        {
            _laserCts?.Cancel();
            _laserCts?.Dispose();
            _laserCts = new CancellationTokenSource();

            _laserLine.SetPosition(0, origin);
            _laserLine.SetPosition(1, origin + (direction * length));
            _laserLine.enabled = true;

            bool isCancelled = await UniTask.Delay(durationMs, cancellationToken: _laserCts.Token).SuppressCancellationThrow();

            if (!isCancelled)
            {
                _laserLine.enabled = false;
            }
        }

        private void OnDestroy()
        {
            _laserCts?.Cancel();
            _laserCts?.Dispose();
        }
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