using ArmorRacks.Things;
using HarmonyLib;
using RimWorld;

namespace DArcaneTechnology.ArmorRackPatches;

[HarmonyPatch(typeof(ArmorRack))]
[HarmonyPatch("CanStoreApparel")]
internal class Patch_CanStoreApparel_Postfix
{
    private static void Postfix(Apparel apparel, ref bool __result)
    {
        if (__result) __result = __result && !Base.IsResearchLocked(apparel.def);
    }
}
