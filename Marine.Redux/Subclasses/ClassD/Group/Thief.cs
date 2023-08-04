using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public sealed class Thief : GroupSubclass
    {
        public override string Name { get; set; } = "Вор";

        public override int Max { get; set; } = 4;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = false,
            Message = new("Вы - Вор! Когда-то давно вы ограбили банк и попали сюда.\nВы имеете хороший стартовый набор, команду .steal и +5% к скорости.", 12, true),
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardJanitor, 100 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.SCP500, 5 },
                        { ItemType.Medkit, 100 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 30 },
                        { ItemType.ArmorLight, 15 },
                        { ItemType.Radio, 100 },
                    }, true)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 20;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;

            base.Unsubscribe();
        }

        private void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.SearchTime *= 0.75f;
        }
    }
}
