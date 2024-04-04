using HarmonyLib;
using RimWorld;

namespace DArcaneTechnology.CorePatches;

[HarmonyPatch(typeof(ResearchManager))]
[HarmonyPatch("FinishProject")]
internal class Patch_FinishProject_Postfix
{
    public static void Postfix()
    {
        Base.playerTechLevel = Base.GetPlayerTech();
    }
}