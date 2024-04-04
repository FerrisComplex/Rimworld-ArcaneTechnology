using Verse.AI;

namespace DArcaneTechnology;

internal class Patch_BaseRackJob_Prefix
{
    private static bool Prefix(JobDriver __instance, ref bool __result)
    {
        var thing = __instance.job.GetTarget(TargetIndex.A).Thing;
        if (thing != null && Base.IsResearchLocked(thing.def))
        {
            __result = false;
            return false;
        }

        return true;
    }
}