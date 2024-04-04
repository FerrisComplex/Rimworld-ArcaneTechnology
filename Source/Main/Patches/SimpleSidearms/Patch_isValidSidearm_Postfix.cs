using System;
using SimpleSidearms.rimworld;
using Verse;

namespace DArcaneTechnology.SimpleSidearmPatches;

internal class Patch_isValidSidearm_Postfix
{
    private static void Postfix(object sidearmType, object pawn, ref bool __result, ref string errString)
    {
        if (__result)
            try
            {
                if (pawn != null && Base.IsResearchLocked(((ThingDefStuffDefPair)sidearmType).thing, (Pawn)pawn))
                {
                    __result = false;
                    errString = "DUnknownTechnology".Translate();
                }
            }
            catch (Exception ex)
            {
                var str = "Error in Arcane Technology simple sidearms postfix, exception is ";
                var ex2 = ex;
                Log.ErrorOnce(str + (ex2 != null ? ex2.ToString() : null), 6969420);
            }
    }
}