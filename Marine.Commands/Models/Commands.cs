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
                    if (property.DeclaringType == typeof(CommandBase) || property.DeclaringType.IsSubclassOf(typeof(CommandBase)))
                    {
                        var value = property.GetValue(this) as CommandBase;

                        if (value == null)
                        {
                            continue;
                        }

                        list.Add(value);
                    }
                }

                return list;
            }
        }

        public Force Force { get; set; } = new();

        public Steal Steal { get; set; } = new();
    }
}
