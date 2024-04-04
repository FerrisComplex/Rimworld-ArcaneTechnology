using RimWorld;
using Verse;

namespace DArcaneTechnology;

internal class Patch_CanWear_Postfix
{
    private static void Postfix(Pawn pawn, Apparel apparel, ref bool __result)
    {
        __result = __result && !Base.IsResearchLocked(apparel.def, pawn);
    }
}