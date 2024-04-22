using System.Collections.Generic;
using DFerrisArcaneTech.Modules;
using Verse;

namespace DArcaneTechnology;

public static class GearAssigner
{
    public static HashSet<string> exemptProjects = new();


    public static Dictionary<string, string> overrideAssignment = new();


    public static Dictionary<string, string> hardAssignment = new();


    // Note: this type is marked as 'beforefieldinit'.
    static GearAssigner()
    {
    }

    public static void HardAssign(ref Dictionary<ThingDef, ResearchProjectDef> thingDic, ref Dictionary<ResearchProjectDef, List<ThingDef>> researchDic)
    {
        foreach (var text in hardAssignment.Keys)
            if (!overrideAssignment.ContainsKey(text))
            {
                var namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail(text);
                if (namedSilentFail != null)
                {
                    var namedSilentFail2 = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(hardAssignment[text]);
                    if (namedSilentFail2 != null)
                    {
                        if (namedSilentFail.GetCompProperties<CompProperties_DArcane>() == null) namedSilentFail.comps.Add(new CompProperties_DArcane(namedSilentFail2));
                        if (!thingDic.ContainsKey(namedSilentFail)) thingDic.Add(namedSilentFail, namedSilentFail2);
                        if (!researchDic.ContainsKey(namedSilentFail2))
                            researchDic.Add(namedSilentFail2, new List<ThingDef>
                            {
                                namedSilentFail
                            });
                        else if (!researchDic[namedSilentFail2].Contains(namedSilentFail)) researchDic[namedSilentFail2].Add(namedSilentFail);
                    }
                }
            }
    }


    public static void OverrideAssign(ref Dictionary<ThingDef, ResearchProjectDef> thingDic, ref Dictionary<ResearchProjectDef, List<ThingDef>> researchDic)
    {
        foreach (var text in overrideAssignment.Keys)
        {
            var namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail(text);
            if (namedSilentFail != null)
            {
                var namedSilentFail2 = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(overrideAssignment[text]);
                if (namedSilentFail2 != null)
                {
                    if (namedSilentFail.GetCompProperties<CompProperties_DArcane>() == null) namedSilentFail.comps.Add(new CompProperties_DArcane(namedSilentFail2));
                    thingDic.SetOrAdd(namedSilentFail, namedSilentFail2);
                    if (!researchDic.ContainsKey(namedSilentFail2))
                        researchDic.Add(namedSilentFail2, new List<ThingDef>
                        {
                            namedSilentFail
                        });
                    else if (!researchDic[namedSilentFail2].Contains(namedSilentFail)) researchDic[namedSilentFail2].Add(namedSilentFail);
                }
            }
        }
    }


    public static bool GetOverrideAssignment(ThingDef thing, out ResearchProjectDef rpd)
    {
        string text;
        if (overrideAssignment.TryGetValue(thing.defName, out text))
        {
            if (text == "None")
            {
                rpd = null;
                return true;
            }

            rpd = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(text);
            if (rpd != null) return true;
        }

        rpd = null;
        return false;
    }


    public static bool ProjectIsExempt(ResearchProjectDef rpd)
    {
        return TechnologyLevelSettings.ExemptClothing && exemptProjects.Contains(rpd.defName);
    }
}
