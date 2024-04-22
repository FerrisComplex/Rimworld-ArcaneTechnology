using System.Collections.Generic;
using DArcaneTechnology;
using DFerrisArcaneTech.Compatability;
using RimWorld;
using UnityEngine;
using Verse;

namespace DFerrisArcaneTech.Modules;

public class TechnologyLevelSettings : CategoryDef
{
    
    private WeaponSettings weaponSettings = new WeaponSettings();
    private ArmorSettings armorSettings = new ArmorSettings();


    public TechnologyLevelSettings(string name, string tooltip = "", RequirementData requirements = null) : base(name, tooltip, requirements)
    {
    }
    
    public static string GetTechLevelName(int techlevel, string defaultUndefinedName = "")
    {
        if (techlevel == 0) return defaultUndefinedName;
        if (techlevel <= 0 || techlevel >= techLabels.Count) return "Unknown(" + techlevel + ")";
        return techLabels[techlevel].Translate();
    }


    private static readonly List<string> techLabels = new List<string>
    {
        "No Tech Restriction",
        "Animal",
        "Neolithic",
        "Medieval",
        "Industrial",
        "Spacer",
        "Ultra",
        "Archotech"
    };
    
    public static bool UseHighestResearched => SelectedTechMethod == 0;
    public static bool UsePercentResearched => SelectedTechMethod == 1;
    public static bool UseActualTechLevel => SelectedTechMethod == 2;
    
    private static int SelectedTechMethod = 0;
    public static float PercentResearchNeeded = 0.75f;
    
    public static TechLevel MinToRestrict = TechLevel.Undefined;
    public static bool EvenResearched = false;
    public static bool ExemptClothing = true;
    public static bool UseInternalClothingCalc = true;
    public static int HowManyTechLevelsAheadOfYours;
    public static bool ExemptFromWealthCalculation;
    
    public override void DoCategoryContents(Listing_Standard originalListing, string filter)
    {
        var calculatedTextLevel = "";
        if (Current.Game == null) calculatedTextLevel = "N/A (Not in Game!)";
        else calculatedTextLevel = GetTechLevelName((int)Base.PlayerTechLevel);
        originalListing.Label("Your calculated tech level: " + calculatedTextLevel);

        originalListing.Gap();
        originalListing.GapLine();


        List<EnhancedListingStandard.RadioButtonLabel<int>> methods = new List<EnhancedListingStandard.RadioButtonLabel<int>>();
        
        methods.Add(new EnhancedListingStandard.RadioButtonLabel<int>("Highest Tech Researched", 0, "If you have even one tech in a tech level researched, you will be considered that tech for the purpose of raids."));
        methods.Add(new EnhancedListingStandard.RadioButtonLabel<int>("Tech completion of a certain percent", 1, "Once you research a certain % of a tech level's available technologies, you will be considered that tech level for the purpose of raids.", (x) =>
        {
            x.DoSettingFloat(null, "Completion Percentage per Level:", "", ref PercentResearchNeeded, 0.05f, 1f, 0.0001f, true);
            originalListing.Gap();
        }));
        
        methods.Add(new EnhancedListingStandard.RadioButtonLabel<int>("Actual Colonist Tech Level", 2, "This requires another mod to edit your tech level\nI hugely recommend Tech Advancing", (x) =>
        {
            x.DoSettingInt(null, "You can use items of " + HowManyTechLevelsAheadOfYours + " tech level" + ((HowManyTechLevelsAheadOfYours == 0 || HowManyTechLevelsAheadOfYours > 1) ? "s" : "") + " ahead of yours", "", ref HowManyTechLevelsAheadOfYours, 0, 7, null, true);
            originalListing.Gap();
        }));
        originalListing.AddLabeledRadioList("Method by which this mod will calculate your tech level for raids and incidents", methods, ref SelectedTechMethod);
        
        if (!UseHighestResearched && Current.Game != null) Base.PlayerTechLevel = Base.GetPlayerTech();
        

        originalListing.Gap();
        originalListing.GapLine();

        originalListing.DoSettingBool(filter, "Try to exempt clothing research options", "Refers to a list of clothing research projects and exempts their products from restriction.", ref ExemptClothing);
        originalListing.DoSettingBool(filter, "Use Clothing Finder V2", "This is a more complex way to find clothing using stat/category deffs, might not be perfect but will find significantly more than the original method", ref UseInternalClothingCalc);
        originalListing.TechLevelMenu("Minimum tech level to restrict items", "Wont restrict any items less than this tech regardless of your current tech level!", MinToRestrict, -1, Text.CalcSize("No Technology Level").x + 30, x => MinToRestrict = x);
        originalListing.DoSettingBool(filter, "Restrict even if the item is researched", "Restricts the items even if you have the research for said item (IE if your tech level is mideval but you have power armor unlocked, you wont be able to wear the power armor until your tech level is actually spacer", ref EvenResearched);
        originalListing.DoSettingBool(filter, "Exclude restricted items from colony wealth calculation", "Prevents restricted items being used in wealth calculations for raid impacts", ref ExemptFromWealthCalculation);
        
        
        // Weapon Handlers
        weaponSettings.DoTweakContents(originalListing, filter);
        
        originalListing.Gap();
        originalListing.GapLine();
        
        // Armor Handlers
        armorSettings.DoTweakContents(originalListing, filter);
        
        originalListing.Gap();
        originalListing.GapLine();

        // Anything extra
        if (!this.HeldSections.NullOrEmpty())
        {
            foreach (SectionDef def in this.HeldSections)
                if (def.MatchesFilter(filter))
                    originalListing.DoSection(def, filter);
        }
    }
    
    public override void OnDoSave()
    {
        this.weaponSettings.OnDoSave();
        this.armorSettings.OnDoSave();
    }
    
    public override void OnPreSave()
    {
        this.weaponSettings.OnPreSave();
        this.armorSettings.OnPreSave();
    }
    
    public override void OnPostSave()
    {
        this.weaponSettings.OnPostSave();
        this.armorSettings.OnPostSave();
    }
    
    public override void OnGameInitialization()
    {
        this.weaponSettings.OnGameInitialization();
        this.armorSettings.OnGameInitialization();
    }
    
    public override void OnMapInitialization()
         {
             // Looping queue to ensure our techlevel is correct
             if (!hasQueued)
             {
                 hasQueued = true;
                 TechnologyQueueSetup();
             }
             
             // Add a delay to loading the techlevel to allow for techadvancing to actually set the tech level first
             Ferris.QueueHelper.AddAction(() =>
             {
                 if (Current.Game != null && !UseHighestResearched) Base.PlayerTechLevel = Base.GetPlayerTech();
             }, 60);
             
             
             
             this.weaponSettings.OnMapInitialization();
             this.armorSettings.OnMapInitialization();
         }
     
         private static bool hasQueued = false;
         private static void TechnologyQueueSetup()
         {
             if (Current.Game != null && UseActualTechLevel) Base.PlayerTechLevel = Base.GetPlayerTech();
             Ferris.QueueHelper.AddAction(TechnologyQueueSetup, 2500);
         }
    
    public override void OnExposeData()
    {
        Look(ref SelectedTechMethod, "SelectedTechMethod", 0);
        Look(ref PercentResearchNeeded, "PercentResearchNeeded", 0.75f);
        Look(ref UseInternalClothingCalc, "UseInternalClothingCalc", true);
        Look(ref ExemptClothing, "ExemptClothing", true);
        Look(ref MinToRestrict, "MinToRestrict", TechLevel.Undefined);
        Look(ref EvenResearched, "EvenResearched", false);
        Look(ref HowManyTechLevelsAheadOfYours, "HowManyTechLevelsAheadOfYours", 0);
        Look(ref ExemptFromWealthCalculation, "ExemptFromWealthCalculation", true);
        
        this.weaponSettings.OnExposeData();
        this.armorSettings.OnExposeData();
    }
}
