using Marine.Commands.API.Abstract;
using Marine.Commands.Commands;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Commands.Models
{
    public class Commands
    {
        [YamlIgnore]
        public List<CommandBase> All
        {
            get
            {
                var list = new List<CommandBase>();

                foreach (var property in GetType().GetProperties())
                {
                    if (property.DeclaringType.IsSubclassOf(typeof(CommandBase)) || property.DeclaringType.DeclaringType.IsSubclassOf(typeof(CommandBase)))
                    {
                        list.Add(property.GetValue(this) as CommandBase);
                    }
                }

                return list;
            }
        }

        public Force Force { get; set; } = new();

        public Steal Steal { get; set; } = new();
    }
}
