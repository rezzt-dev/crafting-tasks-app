using craftingTask.model.objects;
using craftingTask.persistence.managers;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.helpers
{
  public class TrashDropHandler : IDropTarget
  {
    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Task)
      {
        dropInfo.Effects = DragDropEffects.Move;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Task task)
      {
        var sourceList = dropInfo.DragInfo.SourceCollection as ObservableCollection<Task>;
        if (sourceList != null)
        {
          sourceList.Remove(task);
        }

        TaskManager taskManager = new TaskManager();
        taskManager.RemoveTask(task);
      }
    }
  }
}
