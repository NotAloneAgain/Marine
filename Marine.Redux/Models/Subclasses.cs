using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Group;
using Marine.Redux.Subclasses.ClassD.Single;
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
                var list = new List<Subclass>()
                {
                    Dwarf,
                    Giant,
                    Killer,
                    Thief,
                    Pickpocket,
                    Scp073,
                    Scp181,
                    Scp343
                };

                return list;
            }
        }

        public Dwarf Dwarf { get; set; } = new();

        public GigaChad Giant { get; set; } = new();

        public Killer Killer { get; set; } = new();

        public Thief Thief { get; set; } = new();

        public Scp073 Scp073 { get; set; } = new();

        public Scp181 Scp181 { get; set; } = new();

        public Scp343 Scp343 { get; set; } = new();

        public Pickpocket Pickpocket { get; set; } = new();
    }
}
