using craftingTask.model.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace craftingTask.persistence.managers
{
  public class TaskManager
  {
    private List<model.objects.Task> taskList;
    private DBBroker broker;

    public TaskManager ()
    {
      taskList = new List<model.objects.Task> ();
    }

    public long GetTaskLastId()
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var taskCount = broker.ExecuteScalar("SELECT COUNT(*) FROM Task");
          return Convert.ToInt64(taskCount) + 1;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener el último TaskId: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return 1;
      }
    }

    public void AddTask(model.objects.Task inputTask)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@TaskId", inputTask.TaskId },
                {"@PanelId", inputTask.PanelId },
                {"@Title", inputTask.Title },
                {"@Description", inputTask.Description },
                {"@Tag", inputTask.Tag },
                {"@CreationDate", inputTask.CreationDate },
                {"@EndDate", inputTask.EndDate },
                {"@Priority", inputTask.Priority },
                {"@StatusId", inputTask.StatusId },
                {"@Color", inputTask.Color }
            };

          broker.ExecuteNonQuery(
              "INSERT INTO Task (TaskId, PanelId, Title, Description, Tag, CreationDate, EndDate, Priority, StatusId, Color) " +
              "VALUES (@TaskId, @PanelId, @Title, @Description, @Tag, @CreationDate, @EndDate, @Priority, @StatusId, @Color)",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al agregar tarea: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public void UpdateTask(model.objects.Task inputTask)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@TaskId", inputTask.TaskId },
                {"@PanelId", inputTask.PanelId },
                {"@Title", inputTask.Title },
                {"@Description", inputTask.Description },
                {"@Tag", inputTask.Tag },
                {"@CreationDate", inputTask.CreationDate },
                {"@EndDate", inputTask.EndDate },
                {"@Priority", inputTask.Priority },
                {"@StatusId", inputTask.StatusId },
                {"@Color", inputTask.Color }
            };

          broker.ExecuteNonQuery(
              "UPDATE Task SET PanelId = @PanelId, Title = @Title, Description = @Description, Tag = @Tag, " +
              "CreationDate = @CreationDate, EndDate = @EndDate, Priority = @Priority, StatusId = @StatusId, Color = @Color " +
              "WHERE TaskId = @TaskId",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al actualizar tarea: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public void UpdateTaskState(model.objects.Task inputTask, long inputStatusId, long inputPanelId, string inputColor)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@TaskId", inputTask.TaskId },
                {"@PanelId", inputPanelId },
                {"@StatusId", inputStatusId },
                {"@Color", inputColor }
            };

          broker.ExecuteNonQuery(
              "UPDATE Task SET StatusId = @StatusId, Color = @Color, PanelId = @PanelId WHERE TaskId = @TaskId",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al actualizar estado de tarea: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public void RemoveTask(model.objects.Task inputTask)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@TaskId", inputTask.TaskId }
            };

          broker.ExecuteNonQuery(
              "DELETE FROM Task WHERE TaskId = @TaskId",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al eliminar tarea: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public List<model.objects.Task> GetAllTasksFromPanel(long inputPanelId)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@PanelId", inputPanelId }
            };

          return broker.ExecuteQuery<model.objects.Task>(
              "SELECT * FROM Task WHERE PanelId = @PanelId",
              parameters
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener tareas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return new List<model.objects.Task>();
      }
    }

    public List<model.objects.Task> GetAllTasks()
    {
      try
      {
        using (var broker = new DBBroker())
        {
          return broker.ExecuteQuery<model.objects.Task>("SELECT * FROM Task");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener tareas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return new List<model.objects.Task>();
      }
    }
  }
}
