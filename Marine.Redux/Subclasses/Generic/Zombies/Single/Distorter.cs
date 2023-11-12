using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.MicroHID;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MicroHIDPickup = Exiled.API.Features.Pickups.MicroHIDPickup;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Distorter : SingleSubclass
    {
        private int _period = 1;

        public override string Name { get; set; } = "Искажатель";

        public override string Desc { get; set; } = "Ты искажаешь электрические приборы, вызываешь галлюцинации и психоз";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Первый период - невидимость.",
            "Второй период - призрачность. ",
            "Третий период - проявление.",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new Message(string.Empty, 12, true),
            Health = 750,
            Size = Vector3.one * 0.9f,
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 12;

        public int Damage => _period switch
        {
            1 => 10,
            2 => 25,
            3 => 50,
            _ => 40
        };

        protected override void OnAssigned(Player player)
        {
            Timing.RunCoroutine(_LightsBlicks());
            Timing.RunCoroutine(_AddEffects());
        }

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.Amount = Damage;
        }

        private IEnumerator<float> _LightsBlicks()
        {
            Log.Info("Awake");

            while (Player?.IsAlive ?? false)
            {
                yield return Timing.WaitForSeconds(1);

                Log.Info("Start");

                if (Player.CurrentRoom != null && !Player.CurrentRoom.AreLightsOff)
                {
                    Log.Info("Lights");

                    Player.CurrentRoom.TurnOffLights(_period == 3 ? 10 : 0.1f);
                }

                Log.Info("Find pickups");

                foreach (var pickup in Pickup.List)
                {
                    if (pickup == null || pickup.Type is not ItemType.MicroHID and not ItemType.Radio || pickup.Room == null || pickup.Room != Player.CurrentRoom)
                    {
                        continue;
                    }

                    Log.Info("Try radio");

                    if (pickup.Is<RadioPickup>(out var radio) && radio != null)
                    {
                        radio.BatteryLevel -= 2;
                    }

                    Log.Info("Try MICRO-HID");

                    if (pickup.Is<MicroHIDPickup>(out var hid) && hid != null)
                    {
                        hid.Energy -= 0.02f;
                    }
                }

                Log.Info("Find players");

                foreach (var player in Player.List)
                {
                    Log.Info("Iteration of player");

                    if (player == null || player.IsHost || player.IsNPC || player.IsScp || player.IsTutorial || player.CurrentRoom == null || player.CurrentRoom != Player.CurrentRoom)
                    {
                        continue;
                    }

                    Log.Info("Try stamina");

                    player.Stamina -= 0.01f;
                    Player.HumeShield += 1;

                    Log.Info("Try radio");

                    if (player.HasItem(ItemType.Radio))
                    {
                        var radio = player.Items.Select(x => x.As<Radio>()).FirstOrDefault(item => item != null);

                        Log.Info("Try radio action");

                        if (radio != null)
                        {
                            radio.BatteryLevel -= 2;
                        }
                    }

                    Log.Info("Try MICRO-HID");

                    if (player.CurrentItem.Is<MicroHid>(out var hid) && hid != null)
                    {
                        Log.Info("Try MICRO-HID action");

                        hid.Energy -= 0.02f;
                        hid.Base.ServerSendStatus(HidStatusMessageType.EnergySync, hid.Base.EnergyToByte);
                    }
                }

                Log.Info("End of iteration");
            }
        }

        private IEnumerator<float> _AddEffects()
        {
            while (Player?.IsAlive ?? false)
            {
                Player.DisableAllEffects();

                switch (_period)
                {
                    case 1:
                        {
                            Player.EnableEffect(EffectType.Invisible);
                            Player.EnableEffect(EffectType.MovementBoost, 15, 0);

                            _period = 2;

                            break;
                        }
                    case 2:
                        {
                            Player.EnableEffect(EffectType.Ghostly);
                            Player.EnableEffect(EffectType.MovementBoost, 10, 0);

                            _period = 3;

                            break;
                        }
                    case 3:
                        {
                            Player.EnableEffect(EffectType.MovementBoost, 20, 0);

                            _period = 1;

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                yield return Timing.WaitForSeconds(20);
            }
        }
    }
}
