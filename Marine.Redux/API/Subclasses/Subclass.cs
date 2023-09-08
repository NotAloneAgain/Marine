using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Subclasses
{
    public abstract class Subclass : IHasName, IHasHandlers
    {
        private static List<Subclass> _list;

        static Subclass()
        {
            _list = new(100);
        }

        public Subclass()
        {
            Id = _list.Count + 1;

            _list.Add(this);
        }

        [YamlIgnore]
        public static IReadOnlyCollection<Subclass> ReadOnlyCollection => _list.AsReadOnly();

        public abstract SubclassType Type { get; }

        public abstract string Name { get; set; }

        [YamlIgnore]
        public int Id { get; }

        [YamlMember(Alias = "keep_after_escape")]
        public virtual bool KeepAfterEscape { get; set; } = true;

        [YamlMember(Alias = "chance")]
        public abstract int Chance { get; set; }

        [YamlMember(Alias = "start_role")]
        public abstract RoleTypeId Role { get; set; }

        [YamlMember(Alias = "game_role")]
        public virtual RoleTypeId GameRole { get; set; } = RoleTypeId.None;

        [YamlMember(Alias = "spawn_info")]
        public virtual SpawnInfo SpawnInfo { get; set; } = new SpawnInfo();

        public static bool HasAny(Player player) => ReadOnlyCollection.Any(sub => sub.Has(player));

        public static bool Has<TSubclass>(in Player player) where TSubclass : Subclass
        {
            if (player == null || player.IsHost)
            {
                return false;
            }

            foreach (var subclass in _list)
            {
                if (subclass.Is<TSubclass>())
                {
                    return subclass.Has(player);
                }
            }

            return false;
        }

        public static bool AnyHas<TSubclass>() where TSubclass : Subclass
        {
            var subclass = _list.Find(sub => sub.Is<TSubclass>());

            if (subclass == null)
            {
                return false;
            }

            foreach (var player in Player.List.Where(ply => ply.Role == (subclass.GameRole == RoleTypeId.None ? subclass.Role : subclass.GameRole)))
            {
                if (subclass.Has(player))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Is<TSubclass>() where TSubclass : Subclass => As<TSubclass>() != null;

        public TSubclass As<TSubclass>() where TSubclass : Subclass => this as TSubclass;

        public virtual void Subscribe()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public virtual void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        }

        public virtual void Assign(Player player)
        {
            Timing.CallDelayed(0.00005f, delegate ()
            {
                SpawnInfo.Message.Send(player);

                player.AddAhp(SpawnInfo.Shield.Amount, SpawnInfo.Shield.Limit, SpawnInfo.Shield.Decay, SpawnInfo.Shield.Efficacy, SpawnInfo.Shield.Sustain, SpawnInfo.Shield.Persistent);

                player.MaxHealth = SpawnInfo.Health;
                player.Health = SpawnInfo.Health;

                if (SpawnInfo.ShowInfo)
                {
                    CreateInfo(player);
                }

                if (SpawnInfo.Size != Vector3.one)
                {
                    player.Scale = SpawnInfo.Size;
                }

                if (GameRole != RoleTypeId.None)
                {
                    player.Role.Set(GameRole, SpawnReason.ForceClass, RoleSpawnFlags.None);
                }

                if (SpawnInfo.Inventory != null && SpawnInfo.Inventory.Slots.Any())
                {
                    player.ClearInventory();

                    SpawnInfo.Inventory.Randomize();

                    foreach (var item in SpawnInfo.Inventory.Items)
                    {
                        player.AddItem(item);
                    }
                }

                OnAssigned(player);
            });
        }

        public virtual void Revoke(Player player, in RevokeReason reason)
        {
            player.AddAhp(0);

            player.MaxHealth = 100;
            player.Health = 100;

            DestroyInfo(player);

            player.Scale = Vector3.one;

            OnRevoked(player, reason);
        }

        public abstract bool Has(in Player player);

        public virtual bool Can(in Player player)
        {
            if (player == null)
            {
                return false;
            }

            return Random.Range(0, 101) >= 100 - Chance;
        }

        public sealed override string ToString() => $"{Name} ({Role}) [HP: {SpawnInfo.Health}] [AHP: {SpawnInfo.Shield.Limit}]";

        protected virtual void OnAssigned(Player player) { }

        protected virtual void OnRevoked(Player player, in RevokeReason reason) { }

        protected virtual void OnDestroying(DestroyingEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            Revoke(ev.Player, RevokeReason.Leave);
        }

        protected virtual void OnDied(DiedEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            Revoke(ev.Player, RevokeReason.Died);
        }

        protected virtual void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.SpawnFlags == RoleSpawnFlags.None)
            {
                return;
            }

            if (ev.NewRole == Role && Can(ev.Player))
            {
                Assign(ev.Player);

                return;
            }

            if (!Has(ev.Player))
            {
                return;
            }

            if (ev.Reason == SpawnReason.Escaped)
            {
                OnEscaping(ev.Player);

                return;
            }

            Revoke(ev.Player, ev.Reason switch
            {
                SpawnReason.Died => RevokeReason.Died,
                SpawnReason.Destroyed => RevokeReason.Leave,
                SpawnReason.ForceClass => RevokeReason.Admin,
                _ => RevokeReason.None
            });
        }

        protected virtual void OnEscaping(Player player)
        {
            if (KeepAfterEscape)
            {
                Timing.CallDelayed(0.00005f, delegate ()
                {
                    player.AddAhp(SpawnInfo.Shield.Amount, SpawnInfo.Shield.Limit, SpawnInfo.Shield.Decay, SpawnInfo.Shield.Efficacy, SpawnInfo.Shield.Sustain, SpawnInfo.Shield.Persistent);

                    player.MaxHealth = SpawnInfo.Health;
                    player.Health = SpawnInfo.Health;

                    if (SpawnInfo.ShowInfo)
                    {
                        DestroyInfo(player);
                    }
                });
            }
        }

        protected void CreateInfo(Player ply)
        {
            ply.CustomInfo = $"{ply.CustomName}{(string.IsNullOrEmpty(ply.CustomInfo) ? string.Empty : $"\n{ply.CustomInfo}")}\n{Name}";
            ply.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);
        }

        protected void DestroyInfo(Player ply)
        {
            ply.CustomInfo = ply.CustomInfo.Replace(ply.CustomName, string.Empty).Replace("\n", string.Empty).Replace(Name, string.Empty);
            ply.InfoArea |= PlayerInfoArea.Role | PlayerInfoArea.Nickname;
        }
    }
}
