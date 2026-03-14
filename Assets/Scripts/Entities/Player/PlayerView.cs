using UnityEngine;

namespace Asteroids.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerView : MonoBehaviour
    {
        public Transform Transform => transform;
    }
}