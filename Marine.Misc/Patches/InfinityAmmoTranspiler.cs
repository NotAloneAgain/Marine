using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms.Ammo;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Marine.Misc.Patches
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
    internal static class InfinityAmmoTranspiler
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> instructions = ListPool<CodeInstruction>.Shared.Rent(instr);

            const int index = 0;

            instructions.Clear();

            System.Reflection.ConstructorInfo listConstructor = typeof(List<AmmoPickup>).GetConstructor(new Type[1] { typeof(int) });

            instructions.InsertRange(index, new CodeInstruction[7]
            {
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, listConstructor),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, typeof(List<AmmoPickup>).GetMethod(nameof(List<AmmoPickup>.Clear))),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ret)
            });

            for (var i = 0; i < instructions.Count; i++)
            {
                yield return instructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(instructions);
        }
    }
}