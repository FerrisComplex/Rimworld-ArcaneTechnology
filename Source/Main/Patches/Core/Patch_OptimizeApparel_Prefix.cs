using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DArcaneTechnology.CorePatches;

internal class Patch_OptimizeApparel_Prefix
{
    public static bool Prefix(Pawn pawn, ref Job __result)
    {
        if (pawn.IsQuestLodger()) return true;
        if (!DebugViewSettings.debugApparelOptimize && Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick) return true;

        foreach (var thingWithComps in pawn.equipment.AllEquipmentListForReading)
            if (Base.IsResearchLocked(thingWithComps.def, pawn))
            {
                var job = JobMaker.MakeJob(JobDefOf.DropEquipment, thingWithComps);
                __result = job;
                return false;
            }

        var wornApparel = pawn.apparel.WornApparel;
        for (var i = wornApparel.Count - 1; i >= 0; i--)
            if (Base.IsResearchLocked(wornApparel[i].def, pawn))
            {
                var job2 = JobMaker.MakeJob(JobDefOf.RemoveApparel, wornApparel[i]);
                job2.haulDroppedApparel = true;
                __result = job2;
                return false;
            }

        return true;
    }
}
