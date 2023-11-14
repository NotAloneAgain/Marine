using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using PlayerRoles;

namespace Marine.Redux.Subclasses.Generic.Scp939
{
    public class Zmei : SingleSubclass
    {
        public override string Name { get; set; } = "Змей-Горыныч";

        public override string Desc { get; set; } = "Ты огнедышущий ящер, прозванный древними русами \"Змей-Горыныч\"";

        public override int Chance { get; set; } = 2;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp939;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Health = 3666,
        };

        protected override void OnAssigned(Player player)
        {
            base.OnAssigned(player);

            player.ReferenceHub.playerEffectsController.EnableEffect<Spicy>();
        }
    }
}
