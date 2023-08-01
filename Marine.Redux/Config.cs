using Exiled.API.Interfaces;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Group;
using System.Collections.Generic;
using System.ComponentModel;

namespace Marine.Redux
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        public List<Subclass> Subclasses { get; set; } = new List<Subclass>()
        {
            new Killer(),
            new Pickpocket(),
            new Thief(),
        };
    }
}
