using UnityEngine;

namespace Asteroids.InputService
{
    public class DesktopInputService : IInputService
    {
        public bool IsVectorControl => false;
        public Vector2 DirectionVector => Vector2.zero;

        public float ForwardThrust
        {
            get
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return 1f;
                return 0f;
            }
        }

        public float Rotation
        {
            get
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return -1f;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return 1f;
                return 0f;
            }
        }

        public bool IsFiring => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        public bool IsFiringLaser => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1);
    }
}