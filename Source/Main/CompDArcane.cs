using Verse;

namespace DArcaneTechnology;

internal class CompDArcane : ThingComp
{
    public virtual CompProperties_DArcane PropsArcane => (CompProperties_DArcane)props;


    public override string CompInspectStringExtra()
    {
        if (Base.IsResearchLocked(parent.def)) return "Unknown technology (" + PropsArcane.project.LabelCap + ")";
        return base.CompInspectStringExtra();
    }
}