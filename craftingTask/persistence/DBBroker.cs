using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace craftingTask.persistence
{
  public class DBBroker : IDisposable
  {
    private readonly string _connectionString;
    private SqliteConnection _connection;

    public DBBroker()
    {
      Batteries.Init();
      DatabaseInitializer.EnsureDatabaseExists();
      _connectionString = $"Data Source=CTDatabase.db;";
      _connection = new SqliteConnection(_connectionString);
    }

    public void Open()
    {
      if (_connection.State != ConnectionState.Open)
      {
        _connection.Open();
      }
    }

    public void Close()
    {
      if (_connection.State != ConnectionState.Closed)
      {
        _connection.Close();
      }
    }

    public SqliteTransaction BeginTransaction()
    {
      Open();
      return _connection.BeginTransaction();
    }

    public void Dispose()
    {
      Close();
      _connection?.Dispose();
    }

    public List<T> ExecuteQuery<T>(string query, Dictionary<string, object> parameters = null) where T : new()
    {
      var results = new List<T>();

      try
      {
        Open();
        using (var command = new SqliteCommand(query, _connection))
        {
          if (parameters != null)
          {
            foreach (var param in parameters)
            {
              command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
          }

          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              T obj = MapToObject<T>(reader);
              results.Add(obj);
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Error ejecutando consulta: {ex.Message}", ex);
      }

      return results;
    }

    private T MapToObject<T>(SqliteDataReader reader) where T : new()
    {
      T obj = new T();
      var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      for (int i = 0; i < reader.FieldCount; i++)
      {
        string columnName = reader.GetName(i);

        var property = properties.FirstOrDefault(p =>
          p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

        if (property != null && property.CanWrite)
        {
          var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

          if (value != null)
          {
            try
            {
              var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
              var convertedValue = Convert.ChangeType(value, targetType);
              property.SetValue(obj, convertedValue);
            }
            catch
            {
              property.SetValue(obj, value);
            }
          }
        }
      }

      return obj;
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
      int affectedRows = 0;

      try
      {
        Open();
        using (var command = new SqliteCommand(query, _connection))
        {
          if (parameters != null)
          {
            foreach (var param in parameters)
            {
              command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
          }

          affectedRows = command.ExecuteNonQuery();
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Error ejecutando comando: {ex.Message}", ex);
      }

      return affectedRows;
    }

    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
      object result = null;

      try
      {
        Open();
        using (var command = new SqliteCommand(query, _connection))
        {
          if (parameters != null)
          {
            foreach (var param in parameters)
            {
              command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
          }

          result = command.ExecuteScalar();
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Error ejecutando consulta escalar: {ex.Message}", ex);
      }

      return result;
    }
  }
}
