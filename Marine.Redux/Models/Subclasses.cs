using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Group;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Redux.Models
{
    public class Subclasses
    {
        [YamlIgnore]
        public List<Subclass> All
        {
            get
            {
                var list = new List<Subclass>();

                foreach (var property in GetType().GetProperties())
                {
                    if (property.DeclaringType == typeof(Subclass) || property.DeclaringType.IsSubclassOf(typeof(Subclass)))
                    {
                        var value = property.GetValue(this) as Subclass;

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

        public Killer Killer { get; set; } = new();

        public Pickpocket Pickpocket { get; set; } = new();

        public Thief Thief { get; set; } = new();
    }
}
