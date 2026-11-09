namespace Tmobis.Offline.Backups.Attributes
{
  using System;

  public class CalcPropertyRelationsAttribute : Attribute
  {
    public IReadOnlyCollection<string> RelationsPropertiesPaths;

    public CalcPropertyRelationsAttribute(string[] relationsPropertiesPaths)
    {
      RelationsPropertiesPaths = relationsPropertiesPaths;
    }
  }
}
