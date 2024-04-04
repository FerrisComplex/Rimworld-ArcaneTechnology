using Verse;

namespace DArcaneTechnology;

// Used by Defs dont remove!!!!
internal class SpecialThingFilterWorker_DResearchedWeapons : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return t.def.IsWeapon && !Base.IsResearchLocked(t.def);
    }


    public override bool CanEverMatch(ThingDef def)
    {
        return def.IsWeapon && Base.thingDic.ContainsKey(def);
    }
}