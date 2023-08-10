using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp343 : SingleSubclass
    {
        private const string ConsoleMessage = "\n\t+ У тебя есть:" +
                "\n\t\t- телепортация [.tp]." +
                "\n\t\t- Открытия любых дверей кроме оружеек." +
                "\n\t\t- Выдача любых предметов кроме опасных [.item]." +
                "\n\t\t- Преобразование опасных предметов в аптечки путем подбирания." +
                "\n\t\t- Возможность сменить модельку (не роль) подбрасыванием монетки.";

        private RoleTypeId _model = RoleTypeId.Tutorial;

        public Scp343() : base() { }

        public override string Name { get; set; } = "SCP-343";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Бог.\nО своих способностях ты можешь прочитать в консоли.", 15, true, "#009A63"),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.Coin, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override RoleTypeId GameRole { get; set; } = RoleTypeId.Tutorial;

        public override int Chance { get; set; } = 3;

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp073>() && !AnyHas<Scp181>() && Player.List.Count() >= 8;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickupingUpItem;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickupingUpItem;
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Dying -= OnDying;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            player.IsGodModeEnabled = true;

            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }

        protected override void OnRevoked(Player player, in RevokeReason reason)
        {
            player.IsGodModeEnabled = false;
        }

        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            _model++;

            _model = _model switch
            {
                RoleTypeId.Overwatch or RoleTypeId.Filmmaker => RoleTypeId.Scp173,
                RoleTypeId.CustomRole => RoleTypeId.ChaosRifleman,
                RoleTypeId.Scp079 => RoleTypeId.ChaosConscript,
                RoleTypeId.Scp0492 => RoleTypeId.NtfSergeant,
                _ => _model
            };

            ev.Player.ShowHint($"<line-height=95%><size=90%><voffset=-20em><color=#{_model.GetColor()}>Ваша моделька: {_model.Translate()}</color></size></voffset>", 5);

            if (_model == RoleTypeId.Tutorial)
            {
                ev.Player.Role.Set(GameRole, SpawnReason.ForceClass, RoleSpawnFlags.None);

                return;
            }

            ev.Player.ChangeAppearance(_model, Player.List.Where(ply => ply.IsAlive), true);
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Attacker))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Has(ev.Player) || ev.Door.IsMoving || ev.Door.IsLocked)
            {
                return;
            }

            ev.IsAllowed = ev.Door.Type switch
            {
                DoorType.CheckpointArmoryA or DoorType.CheckpointArmoryB
                or DoorType.HczArmory or DoorType.LczArmory
                or DoorType.Scp049Armory or DoorType.Scp173Armory
                or DoorType.HID => false,
                _ => true,
            };
        }

        private void OnPickupingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player != Player || !ev.IsAllowed)
            {
                return;
            }

            var id = ev.Pickup.Info.ItemId;

            if (id.GetCategory() is ItemCategory.Firearm or ItemCategory.Grenade or ItemCategory.MicroHID or ItemCategory.Ammo || id is ItemType.Jailbird or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP018)
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
                ev.Player.AddItem(ItemType.Medkit);
            }
        }
    }
}
