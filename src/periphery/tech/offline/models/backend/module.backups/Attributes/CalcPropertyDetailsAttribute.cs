namespace Tmobis.Offline.Backups.Attributes
{
  using System;

  public class CalcPropertyDetailsAttribute : Attribute
  {
    public readonly string DetailName;
    public IReadOnlyCollection<string> DetailPropertiesPaths;

    public CalcPropertyDetailsAttribute(string detailName, string[] detailPropertiesPaths)
    {
      DetailName = detailName;
      DetailPropertiesPaths = detailPropertiesPaths;
    }
  }
}
