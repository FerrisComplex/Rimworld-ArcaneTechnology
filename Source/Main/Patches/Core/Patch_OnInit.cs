using DFerrisArcaneTech;
using Verse;

namespace DArcaneTechnology.CorePatches;

public class Patch_OnInit
{
    public static void RegisterPatches()
    {
        Ferris.PatchHelper.RegisterPostfixPatch(typeof(ScribeLoader), "FinalizeLoading", null, typeof(Patch_OnInit));
    }

    public static void Postfix()
    {
        SettingsHelper.OnMapInitialization();
    }
}
