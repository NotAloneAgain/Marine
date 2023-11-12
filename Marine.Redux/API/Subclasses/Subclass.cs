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
        private const string RemarkText = "<size=75%>Для более подробной информации обратись в консоль.</size>";

        private static readonly List<Subclass> _list;

        static Subclass() => _list = new(100);

        public Subclass()
        {
            Id = _list.Count + 1;

            _list.Add(this);
        }

        [YamlIgnore]
        public static IReadOnlyCollection<Subclass> ReadOnlyCollection => _list.AsReadOnly();

        public abstract SubclassType Type { get; }

        public abstract string Name { get; set; }

        public abstract string Desc { get; set; }

        public virtual List<string> Abilities { get; set; } = new List<string>();

        [YamlIgnore]
        public int Id { get; }

        [YamlIgnore]
        public virtual bool ConsoleRemark { get; } = false;

        [YamlMember(Alias = "keep_after_escape")]
        public virtual bool KeepAfterEscape { get; set; } = true;

        [YamlMember(Alias = "can_trigger_tesla")]
        public virtual bool CanTriggerTesla { get; set; } = true;

        [YamlMember(Alias = "can_sound_footstep")]
        public virtual bool CanSoundFootstep { get; set; } = true;

        [YamlMember(Alias = "damage_multiplayer")]
        public virtual float DamageMultiplayer { get; set; } = 1;

        [YamlMember(Alias = "hurt_multiplayer")]
        public virtual float HurtMultiplayer { get; set; } = 1;

        [YamlMember(Alias = "chance")]
        public abstract int Chance { get; set; }

        [YamlMember(Alias = "start_role")]
        public abstract RoleTypeId Role { get; set; }

        [YamlMember(Alias = "game_role")]
        public virtual RoleTypeId GameRole { get; set; } = RoleTypeId.None;

        [YamlMember(Alias = "spawn_info")]
        public virtual SpawnInfo SpawnInfo { get; set; } = new SpawnInfo();

        public static bool HasAny(Player player)
        {
            return ReadOnlyCollection.Any(sub => sub.Has(player));
        }

        public static bool Has<TSubclass>(in Player player) where TSubclass : Subclass
        {
            if (player == null || player.IsHost)
            {
                return false;
            }

            foreach (Subclass subclass in _list)
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
            Subclass subclass = _list.Find(sub => sub.Is<TSubclass>());

            if (subclass == null)
            {
                return false;
            }

            foreach (Player player in Player.List.Where(ply => ply.Role == (subclass.GameRole == RoleTypeId.None ? subclass.Role : subclass.GameRole)))
            {
                if (subclass.Has(player))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Is<TSubclass>() where TSubclass : Subclass
        {
            return As<TSubclass>() != null;
        }

        public TSubclass As<TSubclass>() where TSubclass : Subclass
        {
            return this as TSubclass;
        }

        public virtual void Subscribe() { }

        public virtual void Unsubscribe() { }

        public virtual void OnEscaping(Player player)
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

        public virtual void Assign(Player player)
        {
            Log.Info($"Assign {Name} ({GetType().Name}) to {player.Nickname} ({player.UserId})");

            Timing.CallDelayed(0.00005f, delegate ()
            {
                SpawnInfo.Message.FormateWithSend($"Ты - {Name}!\n{Desc}.{(ConsoleRemark ? "\n" + RemarkText : string.Empty)}", player);

                player.SendConsoleMessage($"\nНазвание: {Name}.\nОписание: {Desc}.\nШанс получения: {Chance}%.{GetAbilitiesText()}", "yellow");

                if (GameRole != RoleTypeId.None)
                {
                    player.Role.Set(GameRole, SpawnReason.None, RoleSpawnFlags.None);
                }

                player.AddAhp(SpawnInfo.Shield.Amount, SpawnInfo.Shield.Limit, SpawnInfo.Shield.Decay, SpawnInfo.Shield.Efficacy, SpawnInfo.Shield.Sustain, SpawnInfo.Shield.Persistent);

                if (SpawnInfo.Health != 0)
                {
                    player.MaxHealth = SpawnInfo.Health;
                    player.Health = SpawnInfo.Health;
                }

                if (SpawnInfo.ShowInfo)
                {
                    CreateInfo(player);
                }

                if (SpawnInfo.Size != Vector3.one)
                {
                    player.Scale = SpawnInfo.Size;
                }

                if (SpawnInfo.Inventory != null && SpawnInfo.Inventory.Slots.Any())
                {
                    player.ClearInventory();

                    SpawnInfo.Inventory.Randomize();

                    foreach (ItemType item in SpawnInfo.Inventory.Items)
                    {
                        player.AddItem(item);
                    }
                }

                OnAssigned(player);
            });
        }

        public virtual void Revoke(Player player, in RevokeReason reason)
        {
            DestroyInfo(player);

            player.Scale = Vector3.one;

            OnRevoked(player, reason);
        }

        public virtual bool Has(in Player player) => player != null;

        public virtual bool Can(in Player player) => player != null && Random.Range(0, 101) >= 100 - Chance;

        public sealed override string ToString()
        {
            return $"{Name} ({Role}) [HP: {SpawnInfo.Health}] [AHP: {SpawnInfo.Shield.Limit}]";
        }

        protected internal virtual void OnDamage(HurtingEventArgs ev) { }

        protected internal virtual void OnHurt(HurtingEventArgs ev) { }

        protected virtual void OnAssigned(Player player) { }

        protected virtual void OnRevoked(Player player, in RevokeReason reason) { }

        protected void CreateInfo(Player ply)
        {
            ply.CustomInfo = $"{ply.CustomName}{(string.IsNullOrEmpty(ply.CustomInfo) ? string.Empty : $"\n{ply.CustomInfo}")}\n{GetRoleInfo(ply)}";
            ply.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);
        }

        protected void DestroyInfo(Player ply)
        {
            ply.CustomInfo = ply.CustomInfo.Replace(ply.CustomName, string.Empty).Replace("\n", string.Empty).Replace(GetRoleInfo(ply), string.Empty);
            ply.InfoArea |= PlayerInfoArea.Role | PlayerInfoArea.Nickname;
        }

        private protected string GetAbilitiesText()
        {
            if (Abilities.Count == 0)
            {
                return string.Empty;
            }

            string result = "\nОсобенности: \n";

            for (int i = 0; i < Abilities.Count; i++)
            {
                result += $"{i + 1}.\t {Abilities[i]}{(i + 1 == Abilities.Count ? string.Empty : "\n")}";
            }

            return result;
        }

        private protected string GetRoleInfo(Player ply) => ply.IsScp ? $"{ply.Role.Type.Translate()} - {Name}" : Name;
    }
}
