using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Marine.LevelSystem
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        public string LevelUp { get; set; } = "<voffset=-20em><size=95%><color=#068DA9><b>Поздравляем!</b>\nТы повысил свой уровень с {0} до {1}!</color></size></voffset>";

        public string LevelDown { get; set; } = "<voffset=-20em><size=95%><color=#7E1717><b>Плак-плак!</b>\nТвой уровень понизился с {0} до {1}!</color></size></voffset>";

        public string ExperienceGained { get; set; } = "<voffset=-20em><size=95%><color=#E55807><b>Ты получил {0} опыта за {1}!</b></color></size></voffset>";
    }
}
