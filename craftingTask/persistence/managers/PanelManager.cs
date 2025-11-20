using craftingTask.model.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace craftingTask.persistence.managers
{
  public class PanelManager
  {
    private List<Panel> panelList;
    private DBBroker broker;

    public PanelManager()
    {
      panelList = new List<Panel>();
    }

    public long GetPanelLastId()
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var panelCount = broker.ExecuteScalar("SELECT COUNT(*) FROM Panel");
          return Convert.ToInt64(panelCount) + 1;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener el último PanelId: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return 1;
      }
    }

    public int GetPanelLastOrder()
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var panelOrder = broker.ExecuteScalar("SELECT MAX([Order]) FROM Panel");
          int lastOrder = panelOrder != DBNull.Value ? Convert.ToInt32(panelOrder) : 0;
          return lastOrder + 1;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener el último orden de panel: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return 1;
      }
    }

    public void AddPanel(Panel inputPanel)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var insertParams = new Dictionary<string, object>
            {
                {"@PanelId", inputPanel.PanelId },
                {"@BoardId", inputPanel.BoardId},
                {"@Name", inputPanel.Name},
                {"@Order", inputPanel.Order },
                {"@Color", inputPanel.Color },
                {"@CreationDate", inputPanel.CreationDate }
            };

          broker.ExecuteNonQuery(
              "INSERT INTO Panel (PanelId, BoardId, Name, [Order], Color, CreationDate) " +
              "VALUES (@PanelId, @BoardId, @Name, @Order, @Color, @CreationDate)",
              insertParams
          );
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al crear panel: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public void UpdatePanel(Panel inputPanel)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@PanelId", inputPanel.PanelId },
                {"@Name", inputPanel.Name},
                {"@Order", inputPanel.Order },
            };

          broker.ExecuteNonQuery("UPDATE Panel SET Name = @Name, [Order] = @Order WHERE PanelId = @PanelId", parameters);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al actualizar panel: " + ex.Message);
      }
    }

    public void RemovePanel(Panel inputPanel)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                {"@PanelId", inputPanel.PanelId }
            };

          broker.ExecuteNonQuery("DELETE FROM Panel WHERE PanelId = @PanelId", parameters);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al eliminar panel: " + ex.Message);
      }
    }

    public List<Panel> GetAllPanelsFromBoard(long inputBoardId)
    {
      try
      {
        using (var broker = new DBBroker())
        {
          var parameters = new Dictionary<string, object>
            {
                { "@BoardId", inputBoardId }
            };

          return broker.ExecuteQuery<Panel>("SELECT * FROM Panel WHERE BoardId = @BoardId ORDER BY [Order]", parameters);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener paneles: " + ex.Message);
        return new List<Panel>();
      }
    }

    public List<Panel> GetAllPanels()
    {
      try
      {
        using (var broker = new DBBroker())
        {
          return broker.ExecuteQuery<Panel>("SELECT * FROM Panel ORDER BY BoardId, [Order]");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error al obtener todos los paneles: " + ex.Message);
        return new List<Panel>();
      }
    }
  }
}
