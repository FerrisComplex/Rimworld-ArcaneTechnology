using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace DFerrisArcaneTech.Modules;

public class WeaponSettings : SettingsModuleBase
{
    private static readonly Dictionary<string, int> ManualRequirements = new Dictionary<string, int>();


    public static bool ModifyTechLevels = false;
    public static bool ModifiesRealTechLevels = false;
    private static readonly Dictionary<ModContentPack, bool> IsCollapsed = new Dictionary<ModContentPack, bool>();
    private static bool _isInvalidCollapsed = true;
    private static bool _isShowingMenu = false;

    
    public static bool UpdateTechLevel(ThingDef item, out TechLevel level)
    {
        if (ManualRequirements.TryGetValue(item.defName, out var value) && value >= 0 && value <= 7)
        {
            level = (TechLevel)value;
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


        
        if (action != null && Widgets.ButtonText(new Rect(position - buttonWidth, rect.y, buttonWidth, 29f), ArmorSettings.GetTechLevelName(value) + (value == defaultValue ? " (Default)" : ""), true, true, true, null))
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (var enumValue in Enum.GetValues(typeof(TechLevel)))
                list.Add(new FloatMenuOption(ArmorSettings.GetTechLevelName((TechLevel)enumValue) + (value == defaultValue ? " (Default)" : ""), () => action.Invoke( ((TechLevel)enumValue == defaultValue) ? (TechLevel)128 : (TechLevel)enumValue), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
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
        originalListing.DoSettingBool(filter, "Edit Techlevel of Specific Weapons", "Allows Changing Specific Weapons, ie changing Power Armor to count as mideval", ref ModifyTechLevels);
        originalListing.DoSettingBool(filter, "Show List", "Seperate setting to avoid cluttering this menu", ref _isShowingMenu);
        originalListing.Gap();
        if (_isShowingMenu)
        {
            originalListing.Gap();
            if (originalListing.ButtonTextLabeled("", "Restore Section Defaults", TextAnchor.UpperLeft, null, null))
            {
                OnReset();
                Messages.Message("TechLevel By Weapon tweaks restored to defaults.", MessageTypeDefOf.CautionInput, true);
            }

            var lastIndex = -1;
            bool shouldSkip = false;

            List<ThingDef> invalidDefs = new List<ThingDef>();
            
            float buttonSize = Text.CalcSize("No Technology Level (Default)").x + 15;

            foreach (var v in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsWeapon || x.IsMeleeWeapon || x.IsRangedWeapon).OrderBy(x => x != null && x.modContentPack != null && x.modContentPack.IsCoreMod ? 0 : 1).ThenBy(x => x != null && x.modContentPack != null && x.modContentPack.IsOfficialMod ? 0 : 1).ThenBy(x => x != null && x.modContentPack != null ? x.modContentPack.loadOrder : int.MaxValue - 1).ThenBy(x => x.IsRangedWeapon ? 0 : 1).ThenBy(x => x.IsMeleeWeapon ? 0 : 1))
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
                var reference = ManualRequirements.TryGetValue(v.defName, (int)v.techLevel);
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
                    var reference = ManualRequirements.TryGetValue(v.defName, (int)v.techLevel);
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
        Look(ref ModifiesRealTechLevels, "ModifiesRealTechLevels", true);
        Look(ref ModifyTechLevels, "ModifyTechLevels", false);
        foreach (var v in DefDatabase<ResearchProjectDef>.AllDefs)
        {
            var reference = ManualRequirements.TryGetValue(v.defName, -1);
            Look(ref reference, v.defName, -1);
            if (reference != -1)
                ManualRequirements.SetOrAdd(v.defName, reference);
        }
    }
}
