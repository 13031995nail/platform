namespace Tmobis.Data.DB.MSSQL;

// TODO: Обобщить?
public static class MSSQLConnectionStringValidator
{
  public static bool IsValidConnectionString(string connectionString)
  => connectionString?.Contains("Data Source=") ?? false;
}