using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DArcaneTechnology.CorePatches;

internal class Patch_ApparelScoreGain_Prefix
{
    public static bool Prefix(Pawn pawn, Apparel ap, List<float> wornScoresCache, ref float __result)
    {
        if (Base.IsResearchLocked(ap.def, pawn))
        {
            __result = -5000f;
            return false;
        }

        return true;
    }
}
