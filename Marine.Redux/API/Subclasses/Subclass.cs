using Marine.Redux.API.Enums;
using Marine.Redux.API.Interfaces;
using MEC;
using PlayerRoles;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using Exiled.Events.Commands.Show;
using UnityEngine;
using Exiled.Events.EventArgs.Player;

namespace Marine.Redux.API.Subclasses
{
    public abstract class Subclass : IHasName
    {
        private static List<Subclass> _list;

        static Subclass()
        {
            _list = new(100);
        }

        public Subclass()
        {
            _list.Add(this);
        }

        public Subclass(string name, RoleTypeId role, SpawnInfo spawnInfo) : this()
        {
            Name = name;
            Role = role;
            SpawnInfo = spawnInfo;
        }

        public static IReadOnlyCollection<Subclass> ReadOnlyCollection => _list.AsReadOnly();

        public abstract SubclassType Type { get; }

        public virtual string Name { get; set; } = "(null)";

        [YamlMember(Alias = "start_role")]
        public abstract RoleTypeId Role { get; set; }

        [YamlMember(Alias = "game_role")]
        public virtual RoleTypeId GameRole { get; set; } = RoleTypeId.None;

        [YamlMember(Alias = "spawn_info")]
        public virtual SpawnInfo SpawnInfo { get; set; } = new SpawnInfo();

        public static bool HasAny(in Player player)
        {
            foreach (var subclass in _list)
            {
                if (subclass.Has(player))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Subclass> operator +(in Subclass first, in Subclass second)
        {
            return new(2) { first, second };
        }

        public static List<Subclass> operator -(in List<Subclass> subclasses, in Subclass subclass)
        {
            if (!subclasses.Contains(subclass))
            {
                return subclasses;
            }

            subclasses.Remove(subclass);

            return subclasses;
        }

        public static bool operator ==(in Subclass first, in Subclass second)
        {
            return first.Name == second.Name && first.Role == second.Role && first.SpawnInfo.Inventory.GetHashCode() == second.SpawnInfo.Inventory.GetHashCode() && first.Is(second);
        }

        public static bool operator !=(in Subclass first, in Subclass second)
        {
            return !(first == second);
        }

        public static bool operator true(in Subclass subclass)
        {
            return subclass != null;
        }

        public static bool operator !(in Subclass subclass)
        {
            return subclass == null;
        }

        public static bool operator false(in Subclass subclass)
        {
            return !subclass;
        }

        public bool Is<TSubclass>(TSubclass other) where TSubclass : Subclass
        {
            if (other == null)
            {
                return false;
            }

            return As<TSubclass>() == other;
        }

        public TSubclass As<TSubclass>() where TSubclass : Subclass
        {
            return this as TSubclass;
        }

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
            SpawnInfo.Message.Send(player);

            Timing.CallDelayed(0.00001f, delegate ()
            {
                player.AddAhp(SpawnInfo.Shield.Amount, SpawnInfo.Shield.Limit, SpawnInfo.Shield.Decay, SpawnInfo.Shield.Efficacy, SpawnInfo.Shield.sus, SpawnInfo.Shield.Persistent);

                player.MaxHealth = SpawnInfo.Health;
                player.Health = SpawnInfo.Health;

                if (SpawnInfo.ShowInfo)
                {
                    CreateInfo(player);
                }

                if (SpawnInfo.Size != Vector3.zero && SpawnInfo.Size != Vector3.one)
                {
                    player.Scale = SpawnInfo.Size;
                }

                if (GameRole != RoleTypeId.None)
                {
                    player.RoleManager.ServerSetRole(GameRole, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                }
            });
        }

        public virtual void Revoke(Player player, in RevokeReason reason)
        {
            player.AddAhp(0);

            player.MaxHealth = 100;
            player.Health = 100;

            DestroyInfo(player);

            if (SpawnInfo.Size != Vector3.zero && SpawnInfo.Size != Vector3.one)
            {
                player.Scale = Vector3.one;
            }
        }

        public abstract bool Has(in Player player);

        public sealed override string ToString() => $"{Name} ({Role}) [HP: {SpawnInfo.Health}] [AHP: {SpawnInfo.Shield.Limit}]";

        public sealed override bool Equals(object obj) => obj is Subclass subclass && this == subclass;

        public sealed override int GetHashCode()
        {
            var hashCode = 1852696779;

            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Role.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<SpawnInfo>.Default.GetHashCode(SpawnInfo);

            return hashCode;
        }

        protected void OnDestroying(DestroyingEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            Revoke(ev.Player, RevokeReason.Leave);
        }

        protected void OnDied(DiedEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            Revoke(ev.Player, RevokeReason.Died);
        }

        protected void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            DestroyInfo(ev.Player);
        }

        protected void CreateInfo(Player ply)
        {
            ply.CustomInfo = $"{ply.CustomName}\n{Name}";
            ply.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);
        }

        protected void DestroyInfo(Player ply)
        {
            ply.CustomInfo = string.Empty;
            ply.InfoArea |= PlayerInfoArea.Role | PlayerInfoArea.Nickname;
        }
    }
}
