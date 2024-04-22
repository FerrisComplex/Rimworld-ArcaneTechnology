using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DFerrisArcaneTech.Modules;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DArcaneTechnology.CorePatches;

public static class Patch_WealthWatcher_CalculateWealthItems_Transpiler
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var left = AccessTools.FirstMethod(typeof(ThingOwnerUtility), m => m.Name == "GetAllThingsRecursively" && m.GetParameters().Length >= 6);
        var methodToInject = AccessTools.Method(typeof(Patch_WealthWatcher_CalculateWealthItems_Transpiler), "ExtraItemsFilter");
        var tmpThingsField = typeof(WealthWatcher).GetField("tmpThings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (left == null)
            D.Debug("Can't find method ThingOwnerUtility::GetAllThingsRecursively. Please report it to mod developer");
        else if (methodToInject == null)
            D.Debug("Can't find method Patch_WealthWatcher_CalculateWealthItems_Transpiler::ExtraItemsFilter. Please report it to mod developer");
        else if (tmpThingsField == null)
            D.Debug("Can't find data structure WealthWatcher.tmpThings. Please report it to mod developer");
        else
            foreach (var code in instructions)
            {
                yield return code;
                if (code.opcode == OpCodes.Call && ((MethodInfo)code.operand).Name == "GetAllThingsRecursively")
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, tmpThingsField);
                    yield return new CodeInstruction(OpCodes.Call, methodToInject);
                }
            }
    }


    public static void ExtraItemsFilter(ref List<Thing> tmpThings)
    {
        if (TechnologyLevelSettings.ExemptFromWealthCalculation)
        {
            var list = new List<Thing>();
            foreach (var thing in tmpThings)
                if (!Base.IsResearchLocked(thing.def))
                    list.Add(thing);
            tmpThings = new List<Thing>(list);
        }
    }
}
