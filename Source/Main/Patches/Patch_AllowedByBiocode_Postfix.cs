using Verse;

namespace DArcaneTechnology;

internal class Patch_AllowedByBiocode_Postfix
{
    private static void Postfix(Thing thing, Pawn pawn, ref bool __result)
    {
        __result = __result && !Base.IsResearchLocked(thing.def, pawn);
    }
}