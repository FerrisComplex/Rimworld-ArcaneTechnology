using System;
using System.Collections.Generic;
using System.Linq;
using DArcaneTechnology;
using RimWorld;
using UnityEngine;
using Verse;

namespace DFerrisArcaneTech.Modules;

public class ArmorSettings : SettingsModuleBase
{
    private static ArmorSettings settings;
    private Dictionary<string, int> ManualRequirements = new Dictionary<string, int>();

    public ArmorSettings()
    {
        settings = this;
    }

    public static bool ModifyTechLevels = false;
    public static bool ModifiesRealTechLevels = false;
    private static readonly Dictionary<ModContentPack, bool> IsCollapsed = new Dictionary<ModContentPack, bool>();
    private static bool _isInvalidCollapsed = true;
    private static bool _isShowingMenu = false;

    public static readonly int CLOTHING_VALUE_TECH_LEVEL = 103;

    public static List<string> clothingValuesInternal = new List<string>();


    public static string GetTechLevelName(TechLevel tech)
    {
        return GetTechLevelName((int)tech);
    }

    public static string GetTechLevelName(int tech)
    {
        if (CLOTHING_VALUE_TECH_LEVEL == tech) return "Clothing";
        return TechnologyLevelSettings.GetTechLevelName(tech, "No Technology Level");
    }

    public static bool IsBannedClothingStat(StatModifier def)
    {
        if (def.stat.category == StatCategoryDefOf.PawnCombat) return true;
        if (def.stat == StatDefOf.MeleeDPS) return true;
        if (def.stat == StatDefOf.PainShockThreshold && def.value > 0) return true;
        if (def.stat == StatDefOf.MeleeDamageFactor) return true;
        if (def.stat == StatDefOf.ArmorRating_Sharp) return true;
        if (def.stat == StatDefOf.ArmorRating_Blunt) return true;
        if (def.stat == StatDefOf.ArmorRating_Heat) return true;
        if (def.stat == StatDefOf.EnergyShieldEnergyMax) return true;
        if (def.stat == StatDefOf.EnergyShieldRechargeRate) return true;
        return false;
    }

    public static bool IsBannedCompProp(CompProperties p)
    {
        if (p is CompProperties_Shield) return true;
        if (p is CompProperties_Launchable) return true;
        if (p is CompProperties_Rechargeable) return true;
        if (p is CompProperties_ApparelReloadable) return true;
        if (p is CompProperties_EquippableAbility) return true;
        if (p is CompProperties_TurretGun) return true;

        var className = p.compClass.Name.ToLower();
        if (className.Contains("shieldbelt")) return true;
        if (className.Contains("shield")) return true;

        return false;
    }

    public static void attemptFindClothing()
    {
        foreach (var v in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.apparel != null).ToList())
        {
            if (v.comps != null && v.comps.Any(IsBannedCompProp)) continue;
            if (v.statBases != null && v.statBases.Any(IsBannedClothingStat)) continue;
            clothingValuesInternal.Add(v.defName);
        }
    }

    public override void OnGameInitialization()
    {
        D.Text("Attempting to scan for Clothing elements");
        attemptFindClothing();
        D.Text("Completed, found " + clothingValuesInternal.Count + " Clothing elements taht will be ignored");
    }

    public static bool UpdateTechLevel(ThingDef item, out TechLevel level, bool returnClothingValue = false)
    {
        if (settings != null && settings.ManualRequirements.TryGetValue(item.defName, out var value))
        {
            if (value == CLOTHING_VALUE_TECH_LEVEL && TechnologyLevelSettings.ExemptClothing)
            {
                level = returnClothingValue ? (TechLevel)CLOTHING_VALUE_TECH_LEVEL : TechLevel.Undefined;
                return true;
            }

            if (value >= 0 && value <= 7)
            {
                level = (TechLevel)value;
                return true;
            }
        }

        if (TechnologyLevelSettings.UseInternalClothingCalc && clothingValuesInternal.Contains(item.defName))
        {
            level = returnClothingValue ? (TechLevel)CLOTHING_VALUE_TECH_LEVEL : TechLevel.Undefined;
            return true;
        }

        if (WeaponSettings.UpdateTechLevel(item, out var valueWeapon))
        {
            level = valueWeapon;
            return true;
        }

        level = item.techLevel;
        return false;
    }


    public static void TechMenu(Listing_Standard listing, string name, TechLevel defaultValue, string explanation, TechLevel value, float position, float buttonWidth, Action<TechLevel> action)
    {
        float curHeight = listing.CurHeight;
        Rect rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing, 1f);
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
        TextAnchor anchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(rect, name);
        Text.Anchor = TextAnchor.MiddleRight;
        // Assume -1 = right side, -2 = left side
        if (position == -1)
            position = rect.x + rect.width;
        else if (position == -2)
            position = rect.x + buttonWidth;


        if (action != null && Widgets.ButtonText(new Rect(position - buttonWidth, rect.y, buttonWidth, 29f), GetTechLevelName(value) + (value == defaultValue ? " (Default)" : ""), true, true, true, null))
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            list.Add(new FloatMenuOption(GetTechLevelName(CLOTHING_VALUE_TECH_LEVEL) + (value == defaultValue ? " (Default)" : ""), () => action.Invoke(((TechLevel)CLOTHING_VALUE_TECH_LEVEL == defaultValue) ? (TechLevel)128 : (TechLevel)CLOTHING_VALUE_TECH_LEVEL), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));

            foreach (var enumValue in Enum.GetValues(typeof(TechLevel)))
                list.Add(new FloatMenuOption(GetTechLevelName((TechLevel)enumValue) + (value == defaultValue ? " (Default)" : ""), () => action.Invoke(((TechLevel)enumValue == defaultValue) ? (TechLevel)128 : (TechLevel)enumValue), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
            Find.WindowStack.Add(new FloatMenu(list));
        }

        Text.Anchor = anchor;
        Text.Font = GameFont.Tiny;
        listing.ColumnWidth -= 34f;
        GUI.color = Color.gray;
        listing.Label(explanation, -1f, null);
        listing.ColumnWidth += 34f;
        Text.Font = GameFont.Small;
        rect = listing.GetRect(0f, 1f);
        rect.height = listing.CurHeight - curHeight;
        rect.y -= rect.height;
        GUI.color = Color.white;
        listing.Gap(6f);
    }


    public override void DoTweakContents(Listing_Standard originalListing, string filter = "")
    {
        originalListing.Gap();
        originalListing.DoSettingBool(filter, "Edit Techlevel of Specific Armor", "Allows Changing Specific Armor, ie changing Power Armor to count as mideval", ref ModifyTechLevels);
        originalListing.DoSettingBool(filter, "Show List", "Seperate setting to avoid cluttering this menu", ref _isShowingMenu);
        originalListing.Gap();
        if (_isShowingMenu)
        {
            originalListing.Gap();
            if (originalListing.ButtonTextLabeled("", "Restore Section Defaults", TextAnchor.UpperLeft, null, null))
            {
                OnReset();
                Messages.Message("TechLevel By Armor tweaks restored to defaults.", MessageTypeDefOf.CautionInput, true);
            }

            var lastIndex = -1;
            bool shouldSkip = false;

            List<ThingDef> invalidDefs = new List<ThingDef>();

            float buttonSize = Text.CalcSize("Industrial (Default)").x + 15;

            foreach (var v in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.apparel != null).OrderBy(x => x.modContentPack != null && x.modContentPack.IsCoreMod ? 0 : 1).ThenBy(x => x.modContentPack != null && x.modContentPack.IsOfficialMod ? 0 : 1).ThenBy(x => x.modContentPack != null ? x.modContentPack.loadOrder : int.MaxValue - 1))
            {
                if (v == null) continue;
                if (v.modContentPack == null)
                {
                    invalidDefs.Add(v);
                    continue;
                }


                if (lastIndex != v.modContentPack.loadOrder)
                {
                    lastIndex = v.modContentPack.loadOrder;
                    originalListing.Gap();
                    bool collapsedCategoryState = IsCollapsed.GetValueOrDefault(v.modContentPack, true);
                    shouldSkip = originalListing.DoSubSectionDirect(v.modContentPack.Name.NullOrEmpty() ? v.modContentPack.PackageId.NullOrEmpty() ? "Unknown(" + v.defName + ")" : v.modContentPack.PackageId : v.modContentPack.Name, ref collapsedCategoryState);
                    IsCollapsed.SetOrAdd(v.modContentPack, collapsedCategoryState);
                }

                if (shouldSkip) continue;

                var reference = ManualRequirements.TryGetValue(v.defName, (int)(UpdateTechLevel(v, out var r, true) ? r : v.techLevel));
                TechMenu(originalListing, v.LabelCap.NullOrEmpty() ? v.label.NullOrEmpty() ? v.defName : v.label : v.LabelCap, v.techLevel, v.description.NullOrEmpty() ? "No Description" : v.description, (TechLevel)reference, -1, buttonSize, (x) =>
                {
                    if ((int)x == 128) ManualRequirements.Remove(v.defName);
                    else ManualRequirements.SetOrAdd(v.defName, (int)x);
                });
            }

            originalListing.Gap();
            if (invalidDefs.Any())
            {
                bool collapsedCategoryState_ = _isInvalidCollapsed;
                shouldSkip = originalListing.DoSubSectionDirect("XML Patch Def", ref collapsedCategoryState_);
                _isInvalidCollapsed = collapsedCategoryState_;
                foreach (var v in invalidDefs)
                {
                    if (shouldSkip) continue;
                    var reference = ManualRequirements.TryGetValue(v.defName, (int)(UpdateTechLevel(v, out var r, true) ? r : v.techLevel));
                    TechMenu(originalListing, v.LabelCap.NullOrEmpty() ? v.label.NullOrEmpty() ? v.defName : v.label : v.LabelCap, v.techLevel, v.description.NullOrEmpty() ? "No Description" : v.description, (TechLevel)reference, -1, buttonSize, (x) =>
                    {
                        if ((int)x == 128) ManualRequirements.Remove(v.defName);
                        else ManualRequirements.SetOrAdd(v.defName, (int)x);
                    });
                }

                originalListing.Gap();
            }
        }
    }

    public override void OnReset()
    {
        ModifiesRealTechLevels = false;
        ManualRequirements.Clear();
    }


    public override void OnExposeData()
    {
        Look(ref ModifyTechLevels, "ModifyTechLevels", true);
        
        foreach (var v in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.apparel != null).OrderBy(x => x.modContentPack != null && x.modContentPack.IsCoreMod ? 0 : 1).ThenBy(x => x.modContentPack != null && x.modContentPack.IsOfficialMod ? 0 : 1).ThenBy(x => x.modContentPack != null ? x.modContentPack.loadOrder : int.MaxValue - 1))
        {
            var value = ManualRequirements.TryGetValue(v.defName, out var valueResult) ? valueResult : -999;
            Look(ref value, "ManualRequirements." + v.defName, value);
            if (value == -999)
                ManualRequirements.Remove(v.defName);
            else
                ManualRequirements.SetOrAdd(v.defName, value);
        }
        
    }
}
