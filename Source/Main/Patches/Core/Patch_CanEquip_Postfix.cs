using Verse;

namespace DArcaneTechnology.CorePatches;

internal class Patch_CanEquip_Postfix
{
    private static void Postfix(Thing thing, Pawn pawn, ref string cantReason, ref bool __result)
    {
        if (__result && Base.IsResearchLocked(thing.def, pawn))
        {
            __result = false;
            cantReason = "DUnknownTechnology".Translate();
        }
    }
}