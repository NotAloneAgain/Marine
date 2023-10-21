using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Group;
using Marine.Redux.Subclasses.ClassD.Single;
using Marine.Redux.Subclasses.Events.Halloween;
using Marine.Redux.Subclasses.Guards.Group;
using Marine.Redux.Subclasses.Guards.Single;
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
                    Janitor,
                    Pickpocket,
                    Scp073,
                    Scp181,
                    Scp343,
                    Gang,
                    Letting,
                    Engineer,
                    Head,
                    Medic,
                    Infected,
                    Hidden,
                    Programmer,
                    Manager,
                    Imposter,
                    Assault,
                    Bomber,
                    Informator,
                    Junior,
                    Senior,
                    Sniper,
                    Vampire,
                    TwoFaced
                };

                return list;
            }
        }

        public Dwarf Dwarf { get; set; } = new();

        public GigaChad Giant { get; set; } = new();

        public Killer Killer { get; set; } = new();

        public Thief Thief { get; set; } = new();

        public Janitor Janitor { get; set; } = new();

        public Scp073 Scp073 { get; set; } = new();

        public Scp181 Scp181 { get; set; } = new();

        public Scp343 Scp343 { get; set; } = new();

        public Gang Gang { get; set; } = new();

        public Letting Letting { get; set; } = new();

        public Medic Medic { get; set; } = new();

        public Engineer Engineer { get; set; } = new();

        public Head Head { get; set; } = new();

        public Manager Manager { get; set; } = new();

        public Hidden Hidden { get; set; } = new();

        public Infected Infected { get; set; } = new();

        public Programmer Programmer { get; set; } = new();

        public Imposter Imposter { get; set; } = new();

        public Assault Assault { get; set; } = new();

        public Bomber Bomber { get; set; } = new();

        public Informator Informator { get; set; } = new();

        public Junior Junior { get; set; } = new();

        public Senior Senior { get; set; } = new();

        public Sniper Sniper { get; set; } = new();

        public Pickpocket Pickpocket { get; set; } = new();

        public Vampire Vampire { get; set; } = new();

        public TwoFaced TwoFaced { get; set; } = new();
    }
}
