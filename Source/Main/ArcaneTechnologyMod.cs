using UnityEngine;
using Verse;

namespace DArcaneTechnology;

internal class ArcaneTechnologyMod : Mod
{
    private ArcaneTechnologySettings settings;

    public ArcaneTechnologyMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<ArcaneTechnologySettings>();
        LongEventHandler.QueueLongEvent(Base.Initialize, "DArcaneTech.BuildingDatabase", false, null);
    }


    public override void DoSettingsWindowContents(Rect inRect)
    {
        ArcaneTechnologySettings.DrawSettings(inRect);
        base.DoSettingsWindowContents(inRect);
    }


    public override string SettingsCategory()
    {
        return "Arcane Technology";
    }


    public override void WriteSettings()
    {
        ArcaneTechnologySettings.WriteAll();
        base.WriteSettings();
    }
}