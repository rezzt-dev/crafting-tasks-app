using craftingTask.model.objects;
using craftingTask.persistence.managers;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Panel = craftingTask.model.objects.Panel;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.helpers
{
  public class TaskDropHandler : IDropTarget
  {
    private ObservableCollection<Panel> boardPanels;

    public TaskDropHandler(ObservableCollection<Panel> boardPanels)
    {
      this.boardPanels = boardPanels;
    }

    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Task && dropInfo.TargetCollection is ObservableCollection<Task>)
      {
        dropInfo.Effects = DragDropEffects.Move;
      }
      else
      {
        dropInfo.NotHandled = true;
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Task task && dropInfo.TargetCollection is ObservableCollection<Task> targetList)
      {
        var sourceList = dropInfo.DragInfo.SourceCollection as ObservableCollection<Task>;
        if (sourceList != null)
          sourceList.Remove(task);

        Panel? targetPanel = dropInfo.TargetItem as Panel;
        if (targetPanel == null && dropInfo.VisualTarget is ListBox listBox && listBox.DataContext is Panel panel)
          targetPanel = panel;

        if (targetPanel != null)
        {
          task.MoveToPanel(targetPanel);
          targetPanel.TaskList.Add(task);
        }
      }
      else
      {
        dropInfo.NotHandled = true;
      }
    }
  }
}
