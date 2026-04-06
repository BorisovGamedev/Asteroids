using UnityEngine;

namespace Asteroids.Physics
{
    public static class PhysicsMath
    {
        public static bool RayIntersectsCircle(Vector2 rayOrigin, Vector2 rayDir, float rayLength, Vector2 circleCenter, float circleRadius)
        {
            Vector2 toCircle = circleCenter - rayOrigin;
            
            float projection = Vector2.Dot(toCircle, rayDir);

            if (projection < 0 || projection > rayLength + circleRadius)
                return false;

            Vector2 closestPoint = rayOrigin + rayDir * projection;
            
            float sqrDistance = (circleCenter - closestPoint).sqrMagnitude;

            return sqrDistance <= (circleRadius * circleRadius);
        }
        
        public static float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            
            if (angle < 0f)
            {
                angle += 360f;
            }
            
            return angle;
        }
    }
}