using Exiled.API.Interfaces;
using Marine.Misc.Models;
using System.ComponentModel;

namespace Marine.Misc
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        [Description("Config of RealisticEffects")]
        public RealisticEffectsConfig RealisticEffects { get; set; } = new();

        [Description("Config of BetterFirearms")]
        public BetterFirearmsConfig BetterFirearms { get; set; } = new();

        [Description("Config of RemoteKeycard")]
        public RemoteKeycardConfig RemoteKeycard { get; set; } = new();

        [Description("Config of BetterRoles")]
        public BetterRolesConfig BetterRoles { get; set; } = new();

        [Description("Config of ZombieInfection")]
        public DefaultConfig ZombieInfection { get; set; } = new();

        [Description("Config of InfinityAmmo")]
        public DefaultConfig InfinityAmmo { get; set; } = new();
    }
}
