using UnityEngine;

namespace Asteroids.Entities.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
    }
}