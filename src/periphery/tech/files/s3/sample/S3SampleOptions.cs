namespace Tmobis.Files.Adapter.S3.Sample;

public class S3SampleOptions
{
  public const string SectionName = "S3Sample";

  public string FileName { get; set; } = "tmobis-s3-sample.txt";

  public string Payload { get; set; }
    = "Tmobis S3 sample payload.";
}
