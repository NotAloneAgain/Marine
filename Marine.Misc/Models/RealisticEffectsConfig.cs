namespace Marine.Misc.Models
{
    public sealed class RealisticEffectsConfig : DefaultConfig
    {
        public int Bleeding939 { get; set; } = 6;

        public int BleedingShot { get; set; } = 6;

        public int ZombiePoison { get; set; } = 10;

        public bool AddDuration { get; set; } = true;
    }
}
