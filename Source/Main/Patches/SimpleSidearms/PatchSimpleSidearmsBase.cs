using System;
using HarmonyLib;
using Verse;

namespace DArcaneTechnology.SimpleSidearmPatches;

[StaticConstructorOnStartup]
public static class PatchSimpleSidearmsBase
{
    public static Type aou;

    static PatchSimpleSidearmsBase()
    {
        try
        {
            var harmony = new Harmony("io.github.dametri.arcanetechnology");
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "simple sidearms"))
            {
                D.Text("Attempting SimpleSideArms Patch!");
                var type = GetType("PeteTimesSix.SimpleSidearms.Utilities.StatCalculator");
                if (type == null)
                {
                    type = AccessTools.TypeByName("PeteTimesSix.SimpleSidearms.Utilities.StatCalculator");
                    if (type == null)
                    {
                        D.Warning("Failed to find StatCalculator via AccessTools aborting!");
                        return;
                    }
                }

                aou = type;
                var method = AccessTools.Method(typeof(Patch_isValidSidearm_Postfix), "Postfix");
                var methodInfo = AccessTools.Method(type, "CanPickupSidearmType");
                if (methodInfo == null)
                    methodInfo = AccessTools.DeclaredMethod(type, "CanPickupSidearmType");

                if (methodInfo == null)
                {
                    D.Warning("Failed to find SimpleSidearms Method in StatCalculator!");
                    return;
                }

                D.Text("Enabled SimpleSidearms Patch");
                harmony.Patch(methodInfo, null, new HarmonyMethod(method));
            }
        }
        catch (Exception ex)
        {
            D.Error("Failed to enable SimpleSidearms Patch", ex);
        }
    }


    public static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName, false);
        if (type != null) return type;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (var i = 0; i < assemblies.Length; i++)
        {
            type = assemblies[i].GetType(typeName, false);
            if (type != null) return type;
        }

        return null;
    }
}