using System.Collections.Generic;
using System.Xml;
using Verse;

namespace DArcaneTechnology;

public class OverrideResearch : PatchOperation
{
    public List<Override> Overrides;


    protected override bool ApplyWorker(XmlDocument xml)
    {
        if (Overrides == null) return true;
        foreach (var @override in Overrides) GearAssigner.overrideAssignment.SetOrAdd(@override.thingDef, @override.researchDefName);
        return true;
    }
}
