using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Group;
using Marine.Redux.Subclasses.ClassD.Single;
using Marine.Redux.Subclasses.Scientists.Group;
using Marine.Redux.Subclasses.Scientists.Single;
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
                    Scp343,
                    Gang,
                    Letting,
                    Engineer,
                    Head,
                    Medic,
                    Manager
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

        public Gang Gang { get; set; } = new();

        public Letting Letting { get; set; } = new();

        public Medic Medic { get; set; } = new();

        public Engineer Engineer { get; set; } = new();

        public Head Head { get; set; } = new();

        public Manager Manager { get; set; } = new();

        public Pickpocket Pickpocket { get; set; } = new();
    }
}
