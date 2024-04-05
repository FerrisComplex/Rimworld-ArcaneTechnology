using System.Collections.Generic;
using System.Xml;
using Verse;

namespace DArcaneTechnology;

public class ExemptResearch : PatchOperation
{
    public List<string> Exemptions;


    protected override bool ApplyWorker(XmlDocument xml)
    {
        if (Exemptions == null) return true;
        foreach (var item in Exemptions)
            if (!GearAssigner.exemptProjects.Contains(item))
                GearAssigner.exemptProjects.Add(item);
        return true;
    }
}
