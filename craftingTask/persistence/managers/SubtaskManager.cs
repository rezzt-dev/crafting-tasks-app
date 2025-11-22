using craftingTask.model.objects;
using System;
using System.Collections.Generic;
using System.Windows;

namespace craftingTask.persistence.managers
{
  public class SubtaskManager
  {
    public List<Subtask> GetSubtasksForTask(long parentTaskId)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
                    {
                        { "@ParentTaskId", parentTaskId }
                    };

          return broker.ExecuteQuery<Subtask>(
              "SELECT * FROM Subtask WHERE ParentTaskId = @ParentTaskId ORDER BY \"Order\" ASC",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener subtareas: " + ex.Message);
        return new List<Subtask>();
      }
    }

    public void AddSubtask(Subtask subtask)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
                    {
                        { "@ParentTaskId", subtask.ParentTaskId },
                        { "@Title", subtask.Title },
                        { "@IsCompleted", subtask.IsCompleted ? 1 : 0 },
                        { "@Order", subtask.Order }
                    };

          broker.ExecuteNonQuery(
              "INSERT INTO Subtask (ParentTaskId, Title, IsCompleted, \"Order\") VALUES (@ParentTaskId, @Title, @IsCompleted, @Order)",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al agregar subtarea: " + ex.Message);
      }
    }

    public void UpdateSubtaskStatus(long subtaskId, bool isCompleted)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
                    {
                        { "@SubtaskId", subtaskId },
                        { "@IsCompleted", isCompleted ? 1 : 0 }
                    };

          broker.ExecuteNonQuery(
              "UPDATE Subtask SET IsCompleted = @IsCompleted WHERE SubtaskId = @SubtaskId",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al actualizar subtarea: " + ex.Message);
      }
    }

    public void DeleteSubtask(long subtaskId)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
                    {
                        { "@SubtaskId", subtaskId }
                    };

          broker.ExecuteNonQuery("DELETE FROM Subtask WHERE SubtaskId = @SubtaskId", parameters);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al eliminar subtarea: " + ex.Message);
      }
    }
  }
}
