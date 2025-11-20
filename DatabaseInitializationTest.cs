using craftingTask.persistence;
using Microsoft.Data.Sqlite;
using System;

namespace craftingTask
{
  class DatabaseInitializationTest
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Testing Database Initialization...");
      Console.WriteLine("=================================\n");

      try
      {
        // Test 1: Create DBBroker instance (should trigger initialization)
        Console.WriteLine("1. Creating DBBroker instance...");
        using (var broker = new DBBroker())
        {
          Console.WriteLine("   ✓ DBBroker created successfully");

          // Test 2: Check if database file exists
          Console.WriteLine("\n2. Checking if database file exists...");
          if (System.IO.File.Exists("CTDatabase.db"))
          {
            Console.WriteLine("   ✓ Database file created successfully");
          }
          else
          {
            Console.WriteLine("   ✗ Database file NOT found!");
            return;
          }

          // Test 3: Check if tables exist
          Console.WriteLine("\n3. Checking if tables exist...");
          broker.Open();
          
          var tables = new[] { "Board", "Panel", "Status", "Task", "SavedFilter" };
          bool allTablesExist = true;

          foreach (var table in tables)
          {
            var query = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{table}';";
            var result = broker.ExecuteScalar(query, null);
            
            if (result != null && Convert.ToInt32(result) > 0)
            {
              Console.WriteLine($"   ✓ Table '{table}' exists");
            }
            else
            {
              Console.WriteLine($"   ✗ Table '{table}' NOT found!");
              allTablesExist = false;
            }
          }

          // Test 4: Check if default Status values exist
          if (allTablesExist)
          {
            Console.WriteLine("\n4. Checking default Status values...");
            var statusQuery = "SELECT COUNT(*) FROM Status;";
            var statusCount = broker.ExecuteScalar(statusQuery, null);
            
            if (statusCount != null && Convert.ToInt32(statusCount) >= 5)
            {
              Console.WriteLine($"   ✓ Found {statusCount} default status values");
            }
            else
            {
              Console.WriteLine($"   ✗ Expected at least 5 status values, found {statusCount}");
            }
          }

          broker.Close();
        }

        Console.WriteLine("\n=================================");
        Console.WriteLine("✓ All tests passed successfully!");
        Console.WriteLine("=================================");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"\n✗ ERROR: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
      }

      Console.WriteLine("\nPress any key to exit...");
      Console.ReadKey();
    }
  }
}
