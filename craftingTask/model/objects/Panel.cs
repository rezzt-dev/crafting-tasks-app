using craftingTask.persistence.managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftingTask.model.objects
{
  public class Panel
  {
    public long PanelId { get; set; }
    public long BoardId {  get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public DateTime CreationDate { get; set; }
    public ObservableCollection<Task> TaskList { get; set; } = new ObservableCollection<Task>();

    private List<Panel> panelList { get; set; }
    private long LastPanelId { get; set; }
    private PanelManager panelManager { get; set; }

    public Panel()
    {
      panelManager = new PanelManager();
      panelList = new List<Panel>();

      LastPanelId = panelManager.GetPanelLastId();
      this.PanelId = LastPanelId;
    }

    public Panel(string inputName, string inputColor)
    {
      panelManager = new PanelManager();
      panelList = new List<Panel>();

      LastPanelId = panelManager.GetPanelLastId();
      this.PanelId = LastPanelId;
      this.Name = inputName;
      this.Color = inputColor;
    }

    public Panel (long inputBoardId, string inputName, string inputColor)
    {
      panelManager = new PanelManager();
      panelList = new List<Panel>();

      LastPanelId = panelManager.GetPanelLastId();
      this.PanelId = LastPanelId;
      int lastOrder = panelManager.GetPanelLastOrder();

      this.Name = inputName;
      this.BoardId = inputBoardId;
      this.CreationDate = DateTime.UtcNow;
      this.Order = lastOrder;
      this.Color = inputColor;
    }

    public Panel (long inputPanelId, long inputBoardId, string inputName, int inputOrder, string inputColor,  DateTime inputCreationDate)
    {
      panelManager = new PanelManager();
      panelList = new List<Panel>();

      this.PanelId = inputPanelId;
      this.BoardId = inputBoardId;
      this.Name = inputName;
      this.Order = inputOrder;
      this.CreationDate = inputCreationDate;
      this.Color = inputColor;

      TaskManager taskManager = new TaskManager();
      List<Task> auxTaskList = taskManager.GetAllTasksFromPanel(this.PanelId);
      this.TaskList = new ObservableCollection<Task>(auxTaskList);
    }

    public void Add ()
    {
      panelManager = new PanelManager();
      panelManager.AddPanel (this);
    }
    public void Update ()
    {
      panelManager = new PanelManager();
      panelManager.UpdatePanel(this);
    }
    public void Remove ()
    {
      panelManager = new PanelManager();
      panelManager.RemovePanel(this);
    }
  }
}
