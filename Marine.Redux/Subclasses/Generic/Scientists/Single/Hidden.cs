using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Hidden : SingleSubclass
    {
        public override string Name { get; set; } = "Скрытный";

        public override string Desc { get; set; } = "Ты очень незаметный и скрытный, иногда люди не замечают как ты к ним подходишь";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Возможность скрыться с помощью команды [.hide].",
            "Шаги, бесслышные для SCP-939 (может не работать)"
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            Message = new("Ты - Скрытный!\nТы можешь скрыться с помощью команды .hide.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override bool CanSoundFootstep { get; set; } = false;

        public override int Chance { get; set; } = 15;
    }
}
