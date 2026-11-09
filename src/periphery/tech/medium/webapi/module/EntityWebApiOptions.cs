namespace Tmobis.Medium.WebApi;

public class EntityWebApiOptions
{
    public const string EntityWebApi =  "EntityWebApi";

    public int DefaultPerPage { get; set; } = 10;
    
    public int MaxPerPage { get; set; } = 100;
}
