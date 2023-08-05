using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.ClassD.Group
{
    public sealed class Pickpocket : GroupSubclass
    {
        public Pickpocket() : base() { }

        public override string Name { get; set; } = "Карманник";

        public override int Max { get; set; } = 100;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = false,
            Message = new("Ты - карманник!\nТы появляешься с тем, что смог стащить и можешь воровать командой .steal.", 10, true),
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.ParticleDisruptor, 1 },
                        { ItemType.AntiSCP207, 1 },
                        { ItemType.Flashlight, 30 },
                        { ItemType.Coin, 25 },
                        { ItemType.Medkit, 25 },
                        { ItemType.Painkillers, 20 },
                        { ItemType.ArmorLight, 15 },
                        { ItemType.SCP500, 5 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 25 },
                        { ItemType.Coin, 20 },
                        { ItemType.Medkit, 20 },
                        { ItemType.Painkillers, 15 },
                        { ItemType.ArmorLight, 10 },
                        { ItemType.SCP500, 3 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 20 },
                        { ItemType.Coin, 15 },
                        { ItemType.Medkit, 15 },
                        { ItemType.Painkillers, 10 },
                        { ItemType.ArmorLight, 5 },
                        { ItemType.SCP500, 2 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 15 },
                        { ItemType.Coin, 10 },
                        { ItemType.Medkit, 10 },
                        { ItemType.Painkillers, 5 },
                        { ItemType.ArmorLight, 3 },
                        { ItemType.SCP500, 1 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 10 },
                        { ItemType.Coin, 5 },
                        { ItemType.Medkit, 5 },
                        { ItemType.Painkillers, 3 },
                        { ItemType.ArmorLight, 1 },
                        { ItemType.SCP500, 1 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 10 },
                        { ItemType.Coin, 5 },
                        { ItemType.Medkit, 5 },
                        { ItemType.Painkillers, 3 },
                        { ItemType.ArmorLight, 1 },
                        { ItemType.SCP500, 1 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 5 },
                        { ItemType.Coin, 3 },
                        { ItemType.Medkit, 3 },
                        { ItemType.Painkillers, 1 },
                        { ItemType.ArmorLight, 1 },
                        { ItemType.SCP500, 1 },
                    }, true),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Flashlight, 3 },
                        { ItemType.Coin, 1 },
                        { ItemType.Medkit, 1 },
                        { ItemType.Painkillers, 1 },
                        { ItemType.ArmorLight, 1 },
                        { ItemType.SCP500, 1 },
                    }, true),
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 40;

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

        protected override void OnAssigned(Player player)
        {
            if (player.Items.Any(item => item.Type != ItemType.Coin))
            {
                return;
            }

            player.Broadcast(10, "<b>Воу-воу, полегче! Тебе выпало 8 монеток, поэтому ты получаешь мегабафф! +20% к скорости и 200 здоровья!");

            player.MaxHealth = 200;
            player.Health = 200;

            player.GetEffect(EffectType.MovementBoost).ServerSetState(20);
        }

        private void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.SearchTime *= 0.9f;
        }
    }
}
