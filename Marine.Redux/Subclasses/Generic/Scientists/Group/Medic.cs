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
        public override string Name { get; set; } = "Медик";

        public override string Desc { get; set; } = "Ты имеешь хорошие медицинские познания и можете лечить других";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Лечение других держа аптечку/таблетки/SCP-500 в руках [.heal].",
            "Повышенное количество здоровья.",
            "Двойной эффект от лечения."
        };

        public override bool ConsoleRemark { get; } = true;

        public override int Max { get; set; } = 2;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Медик!\nТы имеете хорошие медицинские познания и можете лечить других (проверь консоль).", 12, true),
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

        private void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type is not ItemType.Medkit and not ItemType.Painkillers || !Has(ev.Player))
            {
                return;
            }

            if (ev.Item.Type == ItemType.Medkit)
            {
                ev.Player.Heal(130);
            }
            else if (ev.Item.Type == ItemType.Painkillers)
            {
                UnityEngine.AnimationCurve animation = (ev.Item.Base as Painkillers)._healProgress;
                PlayerHandler handler = UsableItemsController.GetHandler(ev.Player.ReferenceHub);

                _ = handler.ActiveRegenerations.Remove(handler.ActiveRegenerations.Last());

                handler.ActiveRegenerations.Add(new RegenerationProcess(animation, 0.06666667f, 100));
            }
        }
    }
}
