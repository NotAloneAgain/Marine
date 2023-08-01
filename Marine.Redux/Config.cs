﻿using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Marine.Redux
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        public Models.Subclasses Subclasses { get; set; } = new ();
    }
}
