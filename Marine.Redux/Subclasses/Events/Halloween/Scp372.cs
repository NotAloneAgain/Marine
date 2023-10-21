/*using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Marine.Misc.API;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.Subclasses.Events.Halloween
{
    public class Scp372 : SingleSubclass
    {
        public override string Name { get; set; } = "SCP-372";

        public override string Desc { get; set; } = "Ты наносишь серъезный психологический и моральный урон своим проявлением";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Особая роль, доступная только в октябре в честь Хеллоуина.",
            "Вы можете проявиться и наблюдатели будут получать урон:",
            "Команда для проявления [.scp372].",
            "Если не работает .suicide в помощь"
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Health = 150,
            Message = new("###", 15, true, "#480607"),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override RoleTypeId GameRole { get; set; } = RoleTypeId.Scp0492;

        public override int Chance { get; set; } = 10;

        public override bool Can(in Player player) => false;

        public void Run() => Timing.RunCoroutine(_Damage());

        protected override void OnAssigned(Player player)
        {
            Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible);
        }

        protected override void OnDamage(HurtingEventArgs ev)
        {
            ev.IsAllowed = !Player.HasEffect<Invisible>();
        }

        private IEnumerator<float> _Damage()
        {
            DateTime start = DateTime.Now;

            Player.DisableEffect(Exiled.API.Enums.EffectType.Invisible);

            while (Player != null && (DateTime.Now - start).TotalSeconds <= 40)
            {
                var room = Player.CurrentRoom;

                if (room == null)
                {
                    yield return Timing.WaitForSeconds(1);

                    continue;
                }

                if (room.Players.Count(p => p.IsHuman && p.IsAlive && !Has<Scp343>(p)) > 1)
                {
                    break;
                }

                foreach (var ply in room.Players)
                {
                    float radius = Player.Role.As<FpcRole>().FirstPersonController.FpcModule.CharController.radius;

                    VisionInformation vision = VisionInformation.GetVisionInformation(ply.ReferenceHub, ply.ReferenceHub.PlayerCameraReference, Player.CameraTransform.position, radius, 30, true, true, 0, true);

                    if (!vision.IsLooking || ply.IsScp || ply.IsTutorial || ply.IsGodModeEnabled)
                    {
                        continue;
                    }

                    ply.Hurt(1, Exiled.API.Enums.DamageType.CardiacArrest);

                    room.TurnOffLights(5);
                }

                yield return Timing.WaitForSeconds(0.1f);
            }

            Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible);
        }
    }
}*/