namespace Tmobis.Data.DB.Postgres;

// TODO: Обобщить?
public static class PostgresConnectionStringValidator
{
  public static bool IsValidConnectionString(string connectionString)
  => connectionString?.Contains("Host=") ?? false;
}