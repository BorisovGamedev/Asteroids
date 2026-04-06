namespace Asteroids.Configs
{
    public class PlayerConfig
    {
        public int MaxHealth { get; set; }
        public float Acceleration { get; set; }
        public float MaxSpeed { get; set; }
        public float Drag { get; set; }
        public float RotationSpeed { get; set; }
        public int MaxLaserCharges { get; set; }
        public float LaserCooldownSeconds { get; set; }
        public float BulletSpeed { get; set; }
        public float FireRateSeconds { get; set; }
        public float PlayerRadius { get; set; }
        public float BulletRadius { get; set; }
        public float BulletLifeTimeSeconds { get; set; }
        public float BulletSpawnOffset { get; set; }
        public float InvulnerabilitySeconds { get; set; }
        public float KnockbackForce { get; set; }
    }
}