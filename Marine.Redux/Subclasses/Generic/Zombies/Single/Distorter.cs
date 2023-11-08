using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Subclasses.Generic.Zombies.Single
{
    public class Distorter : SingleSubclass
    {
        private Invisible _invisible;
        private Ghostly _ghostly;
        private int _period = 1;

        public override string Name { get; set; } = "Искажатель";

        public override string Desc { get; set; } = "Ты искажаешь электрические приборы, вызываешь галлюцинации и психоз.";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Первый период - невидимость.",
            "Второй период - призрачность.",
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

        public override int Chance { get; set; } = 10;

        protected override void OnAssigned(Player player)
        {
            Timing.RunCoroutine(_LightsBlicks());
            Timing.RunCoroutine(_AddEffects());

            Player.GetEffect(EffectType.MovementBoost)?.ServerSetState(5, 0, false);
        }

        private IEnumerator<float> _LightsBlicks()
        {
            while (Player != null)
            {
                Player.CurrentRoom.TurnOffLights(0.86f);

                foreach (var pickup in Pickup.List)
                {
                    if (pickup.Type is not ItemType.MicroHID and not ItemType.Radio || pickup.Room != Player.CurrentRoom)
                    {
                        continue;
                    }

                    if (pickup.Is<RadioPickup>(out var radio))
                    {
                        radio.BatteryLevel -= 2;
                    }

                    if (pickup.Is<MicroHIDPickup>(out var hid))
                    {
                        hid.Energy -= 0.02f;
                    }
                }

                foreach (var player in Player.List)
                {
                    if (player.CurrentRoom != Player.CurrentRoom || !player.HasItem(ItemType.Radio) && player.CurrentItem.Type != ItemType.MicroHID)
                    {
                        continue;
                    }

                    if (player.HasItem(ItemType.Radio))
                    {
                        var radio = player.Items.FirstOrDefault(x => x.Type == ItemType.Radio).As<Radio>();

                        radio.BatteryLevel -= 2;
                    }

                    if (player.CurrentItem.Type == ItemType.MicroHID)
                    {
                        var hid = player.CurrentItem.As<MicroHid>();

                        hid.Energy -= 0.02f;
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        private IEnumerator<float> _AddEffects()
        {
            while (Player != null)
            {
                switch (_period)
                {
                    case 1:
                        {
                            _invisible.ServerSetState(255, 0);

                            _period = 2;

                            break;
                        }
                    case 2:
                        {
                            _invisible.ServerSetState(0, 0);
                            _ghostly.ServerSetState(255, 0);

                            _period = 3;

                            break;
                        }
                    case 3:
                        {
                            _invisible.ServerSetState(0, 0);
                            _invisible.ServerSetState(0, 0);

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
