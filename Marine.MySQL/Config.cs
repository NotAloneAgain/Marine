using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Marine.MySQL
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        [Description("Connection string for discord DB.")]
        public string ConnectionDiscord { get; set; } = "server=localhost;user=servers;password=1s9QFls14Fd2@;database=discord;";

        [Description("Connection string for Secret Laboratory DB.")]
        public string ConnectionScp { get; set; } = "server=localhost;user=servers;password=1s9QFls14Fd2@;database=testlvl;";
    }
}
