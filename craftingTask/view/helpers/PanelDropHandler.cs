using craftingTask.model.objects;
using craftingTask.persistence.managers;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace craftingTask.view.helpers
{
  public class PanelDropHandler : IDropTarget
  {
    private Action? _refreshCallback;

    public PanelDropHandler(Action? refreshCallback = null)
    {
      _refreshCallback = refreshCallback;
    }

    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Panel)
      {
        dropInfo.Effects = DragDropEffects.Move;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      if (dropInfo.Data is Panel sourcePanel)
      {
        var sourceList = dropInfo.DragInfo.SourceCollection as ObservableCollection<Panel>;
        if (sourceList != null)
        {
          int oldIndex = sourceList.IndexOf(sourcePanel);
          int newIndex = dropInfo.InsertIndex;

          // Adjust index if moving forward because removal shifts indices
          if (oldIndex < newIndex)
          {
            newIndex--;
          }

          // Ensure newIndex is within bounds
          if (newIndex < 0) newIndex = 0;
          if (newIndex >= sourceList.Count) newIndex = sourceList.Count - 1;

          if (oldIndex != newIndex)
          {
            sourceList.Move(oldIndex, newIndex);
            UpdatePanelOrder(sourceList);
            _refreshCallback?.Invoke();
          }
        }
      }
    }

    private void UpdatePanelOrder(ObservableCollection<Panel> panels)
    {
      PanelManager panelManager = new PanelManager();
      for (int i = 0; i < panels.Count; i++)
      {
        var panel = panels[i];
        // Update order if it changed
        if (panel.Order != i + 1)
        {
          panel.Order = i + 1;
          panelManager.UpdatePanel(panel);
        }
      }
    }
  }
}
