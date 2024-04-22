using DFerrisArcaneTech;
using UnityEngine;
using Verse;

namespace DArcaneTechnology;

internal class ArcaneTechnologyMod : Mod
{
    private Settings settings;

    public ArcaneTechnologyMod(ModContentPack content) : base(content)
    {
        SettingsHelper.RegisterSettingModules();
        settings = GetSettings<Settings>();
        LongEventHandler.QueueLongEvent(Base.Initialize, "DArcaneTech.BuildingDatabase", false, null);
        LongEventHandler.QueueLongEvent(SettingsHelper.OnGameInitialization, "DArcaneTech.OnGameInit", false, null);
    }


        
    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DrawSettings(inRect);
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Arcane Technology";
    }


    public override void WriteSettings()
    {
        foreach (var allCategory in SettingsHelper.AllCategories)
            allCategory.OnPreSave();
        foreach (var allCategory in SettingsHelper.AllCategories)
            allCategory.OnDoSave();
        base.WriteSettings();
        foreach (var allCategory in SettingsHelper.AllCategories)
            allCategory.OnPostSave();
    }
}
