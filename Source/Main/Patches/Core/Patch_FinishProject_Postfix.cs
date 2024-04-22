using HarmonyLib;
using RimWorld;

namespace DArcaneTechnology.CorePatches;

internal class Patch_FinishProject_Postfix
{
    public static void Postfix()
    {
        Base.PlayerTechLevel = Base.GetPlayerTech();
    }
}
