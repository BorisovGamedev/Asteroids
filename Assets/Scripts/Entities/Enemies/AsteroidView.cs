using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AsteroidView : MonoBehaviour
    {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;
    }
}