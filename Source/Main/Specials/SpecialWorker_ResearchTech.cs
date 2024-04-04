using System.Linq;
using RimWorld;
using Verse;

namespace DArcaneTechnology;

// Used by Defs dont remove!!!!
internal class StatWorker_ResearchTech : StatWorker
{
    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (req.HasThing && Base.thingDic.ContainsKey(req.Thing.def)) return 0f;
        return -1f;
    }


    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        return "";
    }


    public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
    {
        return "";
    }


    public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
    {
        if (optionalReq.HasThing && Base.thingDic.ContainsKey(optionalReq.Thing.def) && DefDatabase<ResearchProjectDef>.AllDefs.Contains(Base.thingDic[optionalReq.Thing.def])) return Base.thingDic[optionalReq.Thing.def].LabelCap;
        return "None";
    }
}