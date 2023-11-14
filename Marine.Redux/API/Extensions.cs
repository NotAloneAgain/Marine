using Exiled.API.Enums;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux.API
{
    public static class Extensions
    {
        private static readonly IReadOnlyDictionary<RoleTypeId, string> _rolesTranslation;
        private static readonly IReadOnlyDictionary<EffectType, EffectCategory> _effectToClassification;

        static Extensions()
        {
            _rolesTranslation = new Dictionary<RoleTypeId, string>()
            {
                { RoleTypeId.None, "Пустая роль" },
                { RoleTypeId.Scp173, "SCP-173" },
                { RoleTypeId.ClassD, "Персонал Класса D" },
                { RoleTypeId.Scp106, "SCP-106" },
                { RoleTypeId.NtfSpecialist, "Девятихвостая Лиса — Специалист" },
                { RoleTypeId.Scp049, "SCP-049" },
                { RoleTypeId.Scientist, "Научный Сотрудник" },
                { RoleTypeId.Scp079, "SCP-079" },
                { RoleTypeId.ChaosConscript, "Повстанец Хаоса — Новобранец" },
                { RoleTypeId.Scp096, "SCP-096" },
                { RoleTypeId.Scp0492, "SCP-049-2" },
                { RoleTypeId.NtfSergeant, "Девятихвостая Лиса — Сержант" },
                { RoleTypeId.NtfCaptain, "Девятихвостая Лиса — Капитан" },
                { RoleTypeId.NtfPrivate, "Девятихвостая Лиса — Рядовой" },
                { RoleTypeId.Tutorial, "Обучение" },
                { RoleTypeId.FacilityGuard, "Охранник Комплекса" },
                { RoleTypeId.Scp939, "SCP-939" },
                { RoleTypeId.CustomRole, "Кастомная Роль" },
                { RoleTypeId.ChaosRifleman, "Повстанец Хаоса — Стрелок" },
                { RoleTypeId.ChaosMarauder, "Повстанец Хаоса — Мародёр" },
                { RoleTypeId.ChaosRepressor, "Повстанец Хаоса — Усмиритель" },
                { RoleTypeId.Overwatch, "Надзиратель" },
                { RoleTypeId.Filmmaker, "Контентмейкер" },
                { RoleTypeId.Scp3114, "SCP-3114" },
            };

            _effectToClassification = new Dictionary<EffectType, EffectCategory>()
            {
                { EffectType.Invigorated, EffectCategory.Positive },
                { EffectType.Invisible, EffectCategory.Positive },
                { EffectType.RainbowTaste, EffectCategory.Positive },
                { EffectType.BodyshotReduction, EffectCategory.Positive },
                { EffectType.DamageReduction, EffectCategory.Positive },
                { EffectType.Vitality, EffectCategory.Positive },
                { EffectType.SilentWalk, EffectCategory.Positive },
                //{ EffectType.SugarRush, EffectCategory.Positive },
                { EffectType.Scp1853, EffectCategory.Positive },
                { EffectType.AntiScp207, EffectCategory.Positive },
                { EffectType.SpawnProtected, EffectCategory.Positive },
                //{ EffectType.Prismatic, EffectCategory.Positive },
                //{ EffectType.SugarHigh, EffectCategory.Positive },
                //{ EffectType.OrangeCandy, EffectCategory.Positive },
                { EffectType.Ghostly, EffectCategory.Positive },
                { EffectType.Scp207, EffectCategory.Harmful },
                //{ EffectType.Metal, EffectCategory.Harmful },
            };
        }

        public static string Translate(this RoleTypeId role)
        {
            return _rolesTranslation?[role] ?? role.ToString();
        }

        public static EffectCategory GetCategory(this EffectType type)
        {
            if (_effectToClassification.TryGetValue(type, out var category))
            {
                return category;
            }

            return EffectCategory.Negative;
        }

        public static EffectType GetRandomByCategory(this EffectCategory category)
        {
            List<EffectType> effects = Enum.GetValues(typeof(EffectType)).ToArray<EffectType>().Where(x => x.GetCategory() == category).ToList();

            return effects[UnityEngine.Random.Range(0, effects.Count)];
        }
    }
}
