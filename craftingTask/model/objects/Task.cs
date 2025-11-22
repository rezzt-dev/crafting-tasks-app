using craftingTask.persistence.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace craftingTask.model.objects
{
  public class Task
  {
    public long TaskId { get; set; }
    public long PanelId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public long StatusId { get; set; }
    public string Color { get; set; }
    public ObservableCollection<Subtask> Subtasks { get; set; } = new ObservableCollection<Subtask>();

    private List<Task> taskList { get; set; }
    private long LastTaskId { get; set; }
    private TaskManager taskManager { get; set; }

    public Task()
    {
      taskManager = new TaskManager();
      taskList = new List<Task>();

      this.LastTaskId = taskManager.GetTaskLastId();
      this.TaskId = LastTaskId;
    }

    public Task(long inputPanelId, string inputTitle, string inputDescription, string inputTag,
      int inputPriority, long inputStatusId, string inputColor, DateTime? inputEndDate)
    {
      taskManager = new TaskManager();
      taskList = new List<Task>();

      this.LastTaskId = taskManager.GetTaskLastId();
      this.TaskId = LastTaskId;

      this.PanelId = inputPanelId;
      this.Title = inputTitle;
      this.Description = inputDescription;
      this.Tag = inputTag;
      this.Priority = inputPriority;
      this.CreationDate = DateTime.UtcNow;
      this.StatusId = inputStatusId;
      this.Color = inputColor;
      this.EndDate = inputEndDate ?? this.CreationDate.AddDays(7);
    }

    public Task(long inputPanelId, string inputTitle, string inputDescription, string inputTag,
      DateTime inputCreationDate, int inputPriority, long inputStatusId, string inputColor)
    {
      taskManager = new TaskManager();
      taskList = new List<Task>();

      this.LastTaskId = taskManager.GetTaskLastId();
      this.TaskId = LastTaskId;
      this.Title = inputTitle;
      this.Description = inputDescription;
      this.Tag = inputTag;
      this.Priority = inputPriority;
      this.CreationDate = inputCreationDate;
      this.Priority = inputPriority;
      this.StatusId = inputStatusId;
      this.Color = inputColor;
    }

    public Task(long inputPanelId, string inputTitle, string inputDescription, string inputTag,
      DateTime inputCreationDate, DateTime inputEndDate, int inputPriority, long inputStatusId, string inputColor)
    {
      taskManager = new TaskManager();
      taskList = new List<Task>();

      this.LastTaskId = taskManager.GetTaskLastId();
      this.TaskId = LastTaskId;
      this.Title = inputTitle;
      this.Description = inputDescription;
      this.Tag = inputTag;
      this.Priority = inputPriority;
      this.CreationDate = inputCreationDate;
      this.EndDate = inputEndDate;
      this.Priority = inputPriority;
      this.StatusId = inputStatusId;
      this.Color = inputColor;
    }

    public Task(long inputTaskId, long inputPanelId, string inputTitle, string inputDescription, string inputTag,
      DateTime inputCreationDate, DateTime inputEndDate, int inputPriority, long inputStatusId, string inputColor)
    {
      taskManager = new TaskManager();
      taskList = new List<Task>();

      this.TaskId = inputTaskId;
      this.Title = inputTitle;
      this.Description = inputDescription;
      this.Tag = inputTag;
      this.Priority = inputPriority;
      this.CreationDate = inputCreationDate;
      this.EndDate = inputEndDate;
      this.Priority = inputPriority;
      this.StatusId = inputStatusId;
      this.Color = inputColor;
    }

    public void Add()
    {
      taskManager = new TaskManager();
      taskManager.AddTask(this);
    }
    public void Update()
    {
      taskManager = new TaskManager();
      taskManager.UpdateTask(this);
    }
    public void Remove()
    {
      taskManager = new TaskManager();
      taskManager.RemoveTask(this);
    }

    public void UpdateStateToDo()
    {
      taskManager = new TaskManager();
      taskManager.UpdateTaskState(this, 1, this.PanelId, "#FF6B6B");
    }
    public void UpdateStateDoing()
    {
      taskManager = new TaskManager();
      taskManager.UpdateTaskState(this, 2, this.PanelId, "#FFD93D");
    }
    public void UpdateStateDone()
    {
      taskManager = new TaskManager();
      taskManager.UpdateTaskState(this, 3, this.PanelId, "#6BCB77");
    }
    public void UpdateStateArchived()
    {
      taskManager = new TaskManager();
      taskManager.UpdateTaskState(this, 4, this.PanelId, "#4D96FF");
    }
    public void UpdateStateCustom(string inputPanelColor, long inputPanelId, long inputStatusId)
    {
      taskManager = new TaskManager();
      taskManager.UpdateTaskState(this, inputStatusId, inputPanelId, inputPanelColor);
    }

    public void MoveToPanel(Panel inputTargetPanel)
    {
      this.PanelId = inputTargetPanel.PanelId;
      this.Color = inputTargetPanel.Color;

      switch (inputTargetPanel.Color.ToUpper())
      {
        case "#FF6B6B":
          UpdateStateToDo();
          break;
        case "#FFD93D":
          UpdateStateDoing();
          break;
        case "#6BCB77":
          UpdateStateDone();
          break;
        case "#4D96FF":
          UpdateStateArchived();
          break;
        default:
          UpdateStateCustom(inputTargetPanel.Color, inputTargetPanel.PanelId, 5);
          break;
      }
    }
  }
}
