using System.Collections.Generic;
using System.Linq;
using DFerrisArcaneTech.Modules;
using RimWorld;
using Verse;

namespace DArcaneTechnology;

[StaticConstructorOnStartup]
public static class Base
{
    public static Dictionary<ThingDef, ResearchProjectDef> thingDic;


    public static Dictionary<ResearchProjectDef, List<ThingDef>> researchDic;


    public static Dictionary<TechLevel, List<ResearchProjectDef>> strataDic = new();


    private static TechLevel cachedTechLevel = TechLevel.Undefined;


    static Base()
    {
    }


    public static TechLevel PlayerTechLevel
    {
        get
        {
            if (cachedTechLevel == TechLevel.Undefined) cachedTechLevel = GetPlayerTech();
            return cachedTechLevel;
        }
        set
        {
            if (cachedTechLevel == TechLevel.Undefined)
            {
            }

            cachedTechLevel = value;
        }
    }


    public static void Initialize()
    {
        foreach (var researchProjectDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
        {
            if (!strataDic.ContainsKey(researchProjectDef.techLevel)) strataDic.Add(researchProjectDef.techLevel, new List<ResearchProjectDef>());
            if (!strataDic[researchProjectDef.techLevel].Contains(researchProjectDef)) strataDic[researchProjectDef.techLevel].Add(researchProjectDef);
        }

        MakeDictionaries();
    }


    public static void MakeDictionaries()
    {
        thingDic = new Dictionary<ThingDef, ResearchProjectDef>();
        researchDic = new Dictionary<ResearchProjectDef, List<ThingDef>>();
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            var bestRPDForRecipe = GetBestRPDForRecipe(recipeDef);
            if (bestRPDForRecipe != null && recipeDef.ProducedThingDef != null)
            {
                var producedThingDef = recipeDef.ProducedThingDef;
                if (producedThingDef.GetCompProperties<CompProperties_DArcane>() == null) producedThingDef.comps.Add(new CompProperties_DArcane(bestRPDForRecipe));
                thingDic.SetOrAdd(producedThingDef, bestRPDForRecipe);
                if (researchDic.TryGetValue(bestRPDForRecipe, out var list))
                    list.Add(producedThingDef);
                else
                    researchDic.Add(bestRPDForRecipe, new List<ThingDef>
                    {
                        producedThingDef
                    });
            }
        }

        GearAssigner.HardAssign(ref thingDic, ref researchDic);
        GearAssigner.OverrideAssign(ref thingDic, ref researchDic);
    }


    public static ResearchProjectDef GetBestRPDForRecipe(RecipeDef recipe)
    {
        var producedThingDef = recipe.ProducedThingDef;
        if (producedThingDef == null) return null;
        ResearchProjectDef result;
        if (GearAssigner.GetOverrideAssignment(producedThingDef, out result)) return result;
        if (producedThingDef.category == ThingCategory.Building || (!producedThingDef.IsWeapon && !producedThingDef.IsApparel)) return null;
        if (recipe.researchPrerequisite != null) return recipe.researchPrerequisite;
        if (recipe.researchPrerequisites != null && recipe.researchPrerequisites.Count > 0) return recipe.researchPrerequisites[0];
        if (recipe.recipeUsers != null)
        {
            var num = 99999f;
            var techLevel = TechLevel.Archotech;
            ThingDef thingDef = null;
            foreach (var thingDef2 in recipe.recipeUsers)
            {
                if (thingDef2.researchPrerequisites == null || thingDef2.researchPrerequisites.Count <= 0) return null;
                var statValueAbstract = thingDef2.GetStatValueAbstract(StatDefOf.WorkTableWorkSpeedFactor);
                if (statValueAbstract <= num && thingDef2.researchPrerequisites[0].techLevel <= techLevel)
                {
                    thingDef = thingDef2;
                    num = statValueAbstract;
                    techLevel = thingDef2.researchPrerequisites[0].techLevel;
                }
            }

            if (thingDef != null) return thingDef.researchPrerequisites[0];
        }

        return null;
    }


    public static bool InLockedTechRange(TechLevel rpd)
    {
        if (rpd <= TechnologyLevelSettings.MinToRestrict) return false;
        return  rpd > PlayerTechLevel + (byte)TechnologyLevelSettings.HowManyTechLevelsAheadOfYours;
    }
    


    public static bool Locked(ThingDef thing, ResearchProjectDef rpd)
    {
        var techLevel = thing != null && ArmorSettings.UpdateTechLevel(thing, out var level) && level != TechLevel.Undefined ? level : (rpd != null && !GearAssigner.ProjectIsExempt(rpd) && (!rpd.IsFinished || TechnologyLevelSettings.EvenResearched) ? rpd.techLevel : TechLevel.Undefined);
        return InLockedTechRange(techLevel);
    }


    public static bool IsResearchLocked(ThingDef thingDef, Pawn pawn = null)
    {
        return (pawn == null || pawn.IsColonist) && thingDic.TryGetValue(thingDef, out var rpd) && Locked(thingDef, rpd);
    }


    public static TechLevel GetPlayerTech()
    {
        if (TechnologyLevelSettings.UseHighestResearched)
        {
            for (var i = 7; i > 0; i--)
                if (strataDic.ContainsKey((TechLevel)i))
                    using (var enumerator = strataDic[(TechLevel)i].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            if (enumerator.Current.IsFinished)
                                return (TechLevel)i;
                    }

            return TechLevel.Animal;
        }

        if (TechnologyLevelSettings.UsePercentResearched)
        {
            var num = 0;
            for (var j = 7; j > 0; j--)
                if (strataDic.ContainsKey((TechLevel)j))
                {
                    using (var enumerator = strataDic[(TechLevel)j].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            if (enumerator.Current.IsFinished)
                                num++;
                    }

                    if (num / (float)strataDic[(TechLevel)j].Count >= TechnologyLevelSettings.PercentResearchNeeded) return (TechLevel)j;
                }

            return TechLevel.Animal;
        }

        return Faction.OfPlayer.def.techLevel;
    }
}
