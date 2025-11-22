using craftingTask.model.objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.model
{
  public class CalendarDay : INotifyPropertyChanged
  {
    public DateTime Date { get; set; }
    public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();

    public string TaskSummary
    {
      get
      {
        if (Tasks == null || Tasks.Count == 0)
          return "No hay tareas";

        var sb = new StringBuilder();
        foreach (var t in Tasks)
        {
          sb.AppendLine($"• {t.Title} ({t.EndDate:dd/MM})");
        }
        return sb.ToString();
      }
    }

    public List<TaskDot> TaskDots { get; set; }
    public string DayNumber { get; set; }
    public string DayName { get; set; }
    public Brush Foreground { get; set; }
    public Brush Background { get; set; }

    public bool IsSelected { get; set; } = false;
    public bool IsPlaceholder { get; set; } = false;
    public bool IsToday { get; set; } = false;

    public event PropertyChangedEventHandler? PropertyChanged;
  }

  public class TaskDot
  {
    public SolidColorBrush Color { get; set; }
  }
}
