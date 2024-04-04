using Verse;

namespace DArcaneTechnology;

// Used by Defs dont remove!!!!
internal class SpecialThingFilterWorker_DResearchedApparel : SpecialThingFilterWorker
{
    public override bool Matches(Thing t)
    {
        return t.def.IsApparel && !Base.IsResearchLocked(t.def);
    }


    public override bool CanEverMatch(ThingDef def)
    {
        return def.IsApparel && Base.thingDic.ContainsKey(def);
    }
}