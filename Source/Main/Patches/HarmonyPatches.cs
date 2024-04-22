using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DArcaneTechnology.ArmorRackPatches;
using DArcaneTechnology.CorePatches;
using DArcaneTechnology.SimpleSidearmPatches;
using HarmonyLib;
using RimWorld;
using Verse;
using Patch_CanEquip_Postfix = DArcaneTechnology.CorePatches.Patch_CanEquip_Postfix;

namespace DArcaneTechnology
{

    public class HarmonyPatches : Mod
    {



        private void PatchVanillaRimworld(Harmony harmony)
        {
            Patch_OnInit.RegisterPatches();
            Ferris.PatchHelper.RegisterPatch(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool) }, Ferris.PatchHelper.PatchTarget.PatchType.Postfix, typeof(Patch_CanEquip_Postfix), "Postfix", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool).MakeByRefType() });
            Ferris.PatchHelper.RegisterPatch(typeof(JobGiver_OptimizeApparel), "ApparelScoreGain", null, Ferris.PatchHelper.PatchTarget.PatchType.Prefix, typeof(Patch_ApparelScoreGain_Prefix), "Prefix");
            Ferris.PatchHelper.RegisterPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob", null, Ferris.PatchHelper.PatchTarget.PatchType.Prefix, typeof(Patch_OptimizeApparel_Prefix), "Prefix");
            Ferris.PatchHelper.RegisterPatch(typeof(ResearchManager), "FinishProject", null, Ferris.PatchHelper.PatchTarget.PatchType.Postfix, typeof(Patch_FinishProject_Postfix), "Postfix");
            Ferris.PatchHelper.RegisterPatch(typeof(WealthWatcher), "CalculateWealthItems", null, Ferris.PatchHelper.PatchTarget.PatchType.Transpiler, typeof(Patch_WealthWatcher_CalculateWealthItems_Transpiler), "Transpiler");
        }

        private void PatchSimpleSidearms(Harmony harmony)
        {
            if (!LoadedModManager.RunningModsListForReading.Any(x => x.Name.EqualsIgnoreCase("simple sidearms"))) return;
            D.Text("Found Simple Sidearms was loaded, Attempting to Load Patches");

            Ferris.PatchHelper.RegisterPatch("PeteTimesSix.SimpleSidearms.Utilities.StatCalculator", "CanPickupSidearmType", null, Ferris.PatchHelper.PatchTarget.PatchType.Postfix, typeof(Patch_isValidSidearm_Postfix), "Postfix", null);
        }

        private void PatchArmorRacks(Harmony harmony)
        {
            if (!LoadedModManager.RunningModsListForReading.Any(x => x.Name.EqualsIgnoreCase("Armor Racks"))) return;
            D.Text("Found Armor Racks was loaded, Attempting to Load Patches");

            Ferris.PatchHelper.RegisterPostfixPatch("ArmorRacks.Things.ArmorRack", "CanStoreApparel", null, typeof(Patch_CanStoreApparel_Postfix));
            Ferris.PatchHelper.RegisterPostfixPatch("ArmorRacks.Things.ArmorRack", "CanStoreWeapon", null, typeof(Patch_CanStoreWeapon_Postfix));

        }

        public HarmonyPatches(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("io.github.dametri.arcanetechnology");
            var executingAssembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(executingAssembly);
            PatchVanillaRimworld(harmony);
            PatchSimpleSidearms(harmony);
            PatchArmorRacks(harmony);


            Ferris.PatchHelper.ProcessRegisteredPatches(harmony);
        }

    }

}
