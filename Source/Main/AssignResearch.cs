using System.Collections.Generic;
using System.Xml;
using Verse;

namespace DArcaneTechnology;

public class AssignResearch : PatchOperation
{
    public List<Assignment> Assignments;


    protected override bool ApplyWorker(XmlDocument xml)
    {
        foreach (var assignment in Assignments) GearAssigner.hardAssignment.SetOrAdd(assignment.thingDef, assignment.researchDefName);
        return true;
    }
}