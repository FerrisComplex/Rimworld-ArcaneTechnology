using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace DArcaneTechnology;

internal class ArcaneTechnologySettings : ModSettings
{
    public static bool useHighestResearched;


    public static bool usePercentResearched = true;


    public static float percentResearchNeeded = 0.25f;


    public static bool useActualTechLevel;


    public static TechLevel minToRestrict = TechLevel.Spacer;


    public static bool restrictOnTechLevel;


    public static bool evenResearched;


    public static bool exemptClothing = true;


    public static int howManyTechLevelsAheadOfYours;


    public static bool exemptFromWealthCalculation;


    // Note: this type is marked as 'beforefieldinit'.
    static ArcaneTechnologySettings()
    {
    }


    public override void ExposeData()
    {
        Scribe_Values.Look(ref useHighestResearched, "useHighestResearched");
        Scribe_Values.Look(ref usePercentResearched, "usePercentResearched", true);
        Scribe_Values.Look(ref percentResearchNeeded, "percentResearchNeeded", 0.25f);
        Scribe_Values.Look(ref useActualTechLevel, "useActualTechLevel");
        Scribe_Values.Look(ref minToRestrict, "minToRestrict", TechLevel.Spacer);
        Scribe_Values.Look(ref restrictOnTechLevel, "restrictOnTechLevel");
        Scribe_Values.Look(ref evenResearched, "evenResearched");
        Scribe_Values.Look(ref exemptClothing, "exemptClothing", true);
        Scribe_Values.Look(ref howManyTechLevelsAheadOfYours, "howManyTechLevelsAheadOfYours");
        Scribe_Values.Look(ref exemptFromWealthCalculation, "exemptFromWealthCalculation");
        base.ExposeData();
    }


    public static void WriteAll()
    {
        if (useHighestResearched)
        {
            usePercentResearched = false;
            useActualTechLevel = false;
        }
        else if (usePercentResearched)
        {
            useHighestResearched = false;
            useActualTechLevel = false;
        }
        else if (useActualTechLevel)
        {
            useHighestResearched = false;
            usePercentResearched = false;
        }

        if (Current.Game != null) Base.playerTechLevel = Base.GetPlayerTech();
    }


    public static void DrawSettings(Rect rect)
    {
        var listing_Standard = new Listing_Standard(GameFont.Small);
        listing_Standard.ColumnWidth = rect.width - 30f;
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        var text = "Your calculated tech level: ";
        if (!restrictOnTechLevel)
            text += "N/A (fixed tech level)";
        else if (Current.Game != null)
            text += Enum.GetName(typeof(TechLevel), Base.playerTechLevel);
        else
            text += "Not in game";
        var height = listing_Standard.Label(text).height;
        listing_Standard.GapLine();
        listing_Standard.CheckboxLabeled("Try to exempt clothing research options", ref exemptClothing, "Refers to a list of clothing research projects and exempts their products from restriction.");
        listing_Standard.GapLine();
        listing_Standard.CheckboxLabeled("Restrict items based on the colony's current tech level", ref restrictOnTechLevel, "Instead of manually setting the tech level above which to restrict tech, you can automatically use the colony's tech level.");
        listing_Standard.Gap();
        listing_Standard.GapLine();
        if (restrictOnTechLevel)
        {
            listing_Standard.Label("Method by which this mod will calculate your tech level:");
            listing_Standard.Gap();
            var flag = listing_Standard.RadioButton("Highest tech researched", useHighestResearched, 0f, "If you have even one tech in a tech level researched, you will be considered that tech for the purpose of raids.");
            listing_Standard.Gap();
            var flag2 = listing_Standard.RadioButton("Tech completion of a certain percent", usePercentResearched, 0f, "Once you research a certain % of a tech level's available technologies, you will be considered that tech level for the purpose of raids.");
            if (usePercentResearched)
            {
                percentResearchNeeded = Mathf.Clamp(percentResearchNeeded, 0.05f, 1f);
                percentResearchNeeded = Widgets.HorizontalSlider(listing_Standard.GetRect(height).LeftPartPixels(450f), percentResearchNeeded, 0.05f, 1f, false, Mathf.RoundToInt(percentResearchNeeded * 100f) + "%", null, null, 0.05f);
            }
            else
            {
                listing_Standard.Gap();
            }

            listing_Standard.Gap();
            var flag3 = listing_Standard.RadioButton("Actual colonist tech level", useActualTechLevel, 0f, "Not recommended unless you have a mod to increase your tech level somehow.");
            listing_Standard.Gap();
            listing_Standard.Label(string.Format("You can use items of {0} tech level(s) ahead of yours", howManyTechLevelsAheadOfYours), -1f, "E.g. when it's set to 1, tribals would be able to use Medieval items.");
            howManyTechLevelsAheadOfYours = (int)Widgets.HorizontalSlider(listing_Standard.GetRect(height).LeftPartPixels(450f), howManyTechLevelsAheadOfYours, 0f, 2f, false, null, null, null, 1f);
            listing_Standard.Gap();
            if (flag && flag != useHighestResearched)
            {
                useHighestResearched = true;
                usePercentResearched = false;
                useActualTechLevel = false;
                if (Current.Game != null) Base.playerTechLevel = Base.GetPlayerTech();
            }
            else if (flag2 && flag2 != usePercentResearched)
            {
                useHighestResearched = false;
                usePercentResearched = true;
                useActualTechLevel = false;
                if (Current.Game != null) Base.playerTechLevel = Base.GetPlayerTech();
            }
            else if (flag3 && flag3 != useActualTechLevel)
            {
                useHighestResearched = false;
                usePercentResearched = false;
                useActualTechLevel = true;
                if (Current.Game != null) Base.playerTechLevel = Base.GetPlayerTech();
            }
        }
        else
        {
            var rect2 = listing_Standard.GetRect(Text.LineHeight);
            var rect3 = rect2.LeftPartPixels(300f);
            var rect4 = rect2.RightPartPixels(rect2.width - 300f);
            Widgets.Label(rect3, "Minimum tech level to restrict items");
            minToRestrict = (TechLevel)Mathf.RoundToInt(Widgets.HorizontalSlider(rect4, (float)minToRestrict, 1f, 7f, false, Enum.GetName(typeof(TechLevel), (int)minToRestrict), null, null, 1f));
        }

        listing_Standard.GapLine();
        listing_Standard.CheckboxLabeled("Restrict even if the item is researched", ref evenResearched, "Warning: without a mod to let your colony advance in tech, you will NEVER be able to use certain items.");
        listing_Standard.GapLine();
        listing_Standard.CheckboxLabeled("Exclude restricted items from colony wealth calculation", ref exemptFromWealthCalculation);
        listing_Standard.Gap();
        listing_Standard.Gap();
        listing_Standard.End();
    }
}
