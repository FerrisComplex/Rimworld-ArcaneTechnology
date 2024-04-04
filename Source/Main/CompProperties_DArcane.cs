using Verse;

namespace DArcaneTechnology;

internal class CompProperties_DArcane : CompProperties
{
    public ResearchProjectDef project;

    public CompProperties_DArcane(ResearchProjectDef rpd)
    {
        compClass = typeof(CompDArcane);
        project = rpd;
    }
}