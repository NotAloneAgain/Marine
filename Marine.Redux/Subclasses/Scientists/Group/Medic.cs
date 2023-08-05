using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.Scientists.Group
{
    public class Medic : GroupSubclass
    {
        private const string ConsoleMessage = "\n\t+ У тебя есть:" +
                "\n\t\t- Возможность лечить других держа аптечку/таблетки/SCP-500 в руках [.heal]." +
                "\n\t\t- Двойной эффект при лечении (аптечка у тебя восстановит 130 здоровья, обезболивающее восстановит 100 здоровья но с той же скоростью).";

        public Medic() : base() { }

        public override string Name { get; set; } = "Медик";

        public override int Max { get; set; } = 2;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Вы - Медик!\nВы имеете хорошие медицинские познания и можете лечить других (проверь консоль).", 12, true),
            Health = 125,
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardScientist, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Medkit, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 15;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }

        private void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type is not ItemType.Medkit and not ItemType.Painkillers || !Has(ev.Player))
            {
                return;
            }

            if (ev.Item.Type == ItemType.Medkit)
            {
                ev.Player.Heal(65);
            }
            else if (ev.Item.Type == ItemType.Painkillers)
            {
                var animation = (ev.Item.Base as Painkillers)._healProgress;
                var handler = UsableItemsController.GetHandler(ev.Player.ReferenceHub);

                handler.ActiveRegenerations.Remove(handler.ActiveRegenerations.Last());

                handler.ActiveRegenerations.Add(new RegenerationProcess(animation, 0.06666667f, 100));
            }
        }
    }
}
