using System.Reflection;
using DArcaneTechnology.CorePatches;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DArcaneTechnology;

public class HarmonyPatches : Mod
{
    public HarmonyPatches(ModContentPack content) : base(content)
    {
        var harmony = new Harmony("io.github.dametri.arcanetechnology");
        var executingAssembly = Assembly.GetExecutingAssembly();
        harmony.PatchAll(executingAssembly);
        D.Text("Attempting CanEquip Patch");
        var methodInfo = AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new[]
        {
            typeof(Thing),
            typeof(Pawn),
            typeof(string).MakeByRefType(),
            typeof(bool)
        });
        var methodInfo2 = AccessTools.Method(typeof(Patch_CanEquip_Postfix), "Postfix", new[]
        {
            typeof(Thing),
            typeof(Pawn),
            typeof(string).MakeByRefType(),
            typeof(bool).MakeByRefType()
        });
        if (methodInfo != null && methodInfo2 != null)
        {
            D.Text("Patch Success!");
            harmony.Patch(methodInfo, null, new HarmonyMethod(methodInfo2));
        }
        else
        {
            D.Warning("Failed to find " + (methodInfo == null && methodInfo2 == null ? "Both" : methodInfo == null ? "Target" : "Patch") + " Method to replace, skipping this patch");
        }
    }
}