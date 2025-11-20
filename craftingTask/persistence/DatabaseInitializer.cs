using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace craftingTask.persistence
{
  public static class DatabaseInitializer
  {
    private const string DatabaseFileName = "CTDatabase.db";
    private const string SqlScriptPath = "resources/database/createDatabase.sql";

    public static void EnsureDatabaseExists()
    {
      try
      {
        bool dbFileExists = DatabaseFileExists();

        if (!dbFileExists)
        {
          CreateDatabaseFile();
        }

        if (!TablesExist())
        {
          ExecuteSqlScript();
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Error al inicializar la base de datos: {ex.Message}", ex);
      }
    }

    private static bool DatabaseFileExists()
    {
      return File.Exists(DatabaseFileName);
    }

    private static void CreateDatabaseFile()
    {
      using (var connection = new SqliteConnection($"Data Source={DatabaseFileName};"))
      {
        connection.Open();
      }
    }

    private static bool TablesExist()
    {
      try
      {
        using (var connection = new SqliteConnection($"Data Source={DatabaseFileName};"))
        {
          connection.Open();
          using (var command = new SqliteCommand(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Board';",
            connection))
          {
            var result = command.ExecuteScalar();
            return result != null && Convert.ToInt32(result) > 0;
          }
        }
      }
      catch
      {
        return false;
      }
    }

    private static void ExecuteSqlScript()
    {
      string sqlScript = ReadSqlScript();

      if (string.IsNullOrWhiteSpace(sqlScript))
      {
        throw new Exception("El script SQL está vacío o no se pudo leer.");
      }

      using (var connection = new SqliteConnection($"Data Source={DatabaseFileName};"))
      {
        connection.Open();

        var statements = sqlScript.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var statement in statements)
        {
          var trimmedStatement = statement.Trim();
          if (!string.IsNullOrWhiteSpace(trimmedStatement))
          {
            using (var command = new SqliteCommand(trimmedStatement, connection))
            {
              command.ExecuteNonQuery();
            }
          }
        }
      }
    }

    private static string ReadSqlScript()
    {
      try
      {
        if (File.Exists(SqlScriptPath))
        {
          return File.ReadAllText(SqlScriptPath);
        }

        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var resourceName = "craftingTask.resources.database.createDatabase.sql";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
        {
          if (stream != null)
          {
            using (StreamReader reader = new StreamReader(stream))
            {
              return reader.ReadToEnd();
            }
          }
        }

        throw new FileNotFoundException($"No se encontró el archivo SQL en: {SqlScriptPath}");
      }
      catch (Exception ex)
      {
        throw new Exception($"Error al leer el script SQL: {ex.Message}", ex);
      }
    }
  }
}
