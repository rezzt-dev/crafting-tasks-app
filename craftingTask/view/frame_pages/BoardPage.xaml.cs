using craftingTask.model.objects;
using craftingTask.persistence;
using craftingTask.persistence.managers;
using craftingTask.view.helpers;
using craftingTask.view.windows.windows_dialogs;
using craftingTask.view.windows.windows_dialogs.panel_dialogs;
using craftingTask.view.windows.windows_dialogs.task_dialogs;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static craftingTask.persistence.HelpMethods;
using Panel = craftingTask.model.objects.Panel;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.frame_pages
{
  /// <summary>
  /// Lógica de interacción para BoardPage.xaml
  /// </summary>
  public partial class BoardPage : Page
  {
    public Board selectedBoard;
    private ObservableCollection<Panel> BoardPanels { get; set; } = new ObservableCollection<Panel>();
    public ObservableCollection<Panel> PanelList { get; set; }
    public TaskDropHandler DefaultDropHandler { get; private set; }
    public ICommand TaskDoubleClickCommand { get; private set; }

    public BoardPage(Board inputSelectedBoard)
    {
      InitializeComponent();
      this.selectedBoard = inputSelectedBoard;

      this.PanelList = new ObservableCollection<Panel>();
      LoadPanels(selectedBoard.BoardId);

      TaskDoubleClickCommand = new RelayCommand(OnTaskDoubleClick);
      DataContext = this;

      DefaultDropHandler = new TaskDropHandler(BoardPanels);
      this.Loaded += (s, e) =>
      {
        foreach (var panel in PanelList)
        {
          var container = panelListBox.ItemContainerGenerator.ContainerFromItem(panel) as ListBoxItem;
          if (container != null)
          {
            var listBox = FindVisualChild<ListBox>(container);
            if (listBox != null)
            {
              GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(listBox, DefaultDropHandler);
            }
          }
        }
      };
    }

    public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is T tChild)
          return tChild;

        var result = FindVisualChild<T>(child);
        if (result != null)
          return result;
      }
      return null;
    }

    private void btnCloseBoardFrame(object sender, RoutedEventArgs e)
    {
      Frame? parentFrame = HelpMethods.FindParentFrame(this);
      if (parentFrame != null)
      {
        parentFrame.Content = null;
      }
    }

    private void LoadPanels(long inputBoardId)
    {
      PanelList.Clear();
      BoardPanels.Clear();

      PanelManager panelManager = new PanelManager();
      TaskManager taskManager = new TaskManager();

      List<Panel> loadedPanels = panelManager.GetAllPanelsFromBoard(inputBoardId);

      foreach (Panel panel in loadedPanels)
      {
        if (panel.Name.ToLower() == "archivadas")
          continue;

        var tasks = taskManager.GetAllTasksFromPanel(panel.PanelId);
        panel.TaskList = new ObservableCollection<Task>(tasks);

        PanelList.Add(panel);
        BoardPanels.Add(panel);
      }

      DefaultDropHandler = new TaskDropHandler(BoardPanels);
      panelListBox.UpdateLayout();
      AssignDropHandlers();
    }


    private void AssignDropHandlers()
    {
      var generator = panelListBox.ItemContainerGenerator;
      if (generator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
      {
        void OnStatusChanged(object? s, EventArgs e)
        {
          if (generator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
          {
            generator.StatusChanged -= OnStatusChanged;
            AttachHandlersToContainers();
          }
        }

        generator.StatusChanged += OnStatusChanged;
        return;
      }

      AttachHandlersToContainers();
    }

    private void AttachHandlersToContainers()
    {
      foreach (var panel in PanelList)
      {
        var container = panelListBox.ItemContainerGenerator.ContainerFromItem(panel) as ListBoxItem;
        if (container == null)
        {
          continue;
        }
        var innerListBox = FindVisualChild<ListBox>(container);
        if (innerListBox != null)
        {
          GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(innerListBox, DefaultDropHandler);
          innerListBox.AllowDrop = true;
        }
      }
    }

    private void btnCreatePanel_Click(object sender, RoutedEventArgs e)
    {
      CreatePanelDialog createPanelDialog = new CreatePanelDialog(selectedBoard);
      createPanelDialog.PanelCreated += (panel) =>
      {
        LoadPanels(selectedBoard.BoardId);
        DefaultDropHandler = new TaskDropHandler(BoardPanels);
      };
      createPanelDialog.ShowDialog();
    }

    private void btnDeletePanels_Click(object sender, RoutedEventArgs e)
    {
      var excludedNames = new[] { "Realizadas", "En Progreso", "Pendientes", "Archivadas" };
      var filtered = PanelList.Where(p => !excludedNames.Contains(p.Name)).ToList();

      DeletePanelsDialog deletePanelsDialog = new DeletePanelsDialog(PanelList, filtered);
      deletePanelsDialog.ShowDialog();
    }

    private void btnCreateTask_Click(object sender, RoutedEventArgs e)
    {
      Panel? toDoPanel = BoardPanels.FirstOrDefault(p => p.Name.ToLower() == "pendientes");
      if (toDoPanel != null)
      {
        long panelId = toDoPanel.PanelId;
        CreateTaskDialog createTaskDialog = new CreateTaskDialog(toDoPanel);
        createTaskDialog.TaskCreated += () =>
        {
          LoadPanels(selectedBoard.BoardId);
        };
        createTaskDialog.ShowDialog();
      }
    }

    private void btnShowArchiveTasks_Click(object sender, RoutedEventArgs e)
    {
      PanelManager panelManager = new PanelManager();
      List<Panel> loadedPanels = panelManager.GetAllPanelsFromBoard(selectedBoard.BoardId);
      Panel donePanel = loadedPanels.FirstOrDefault(p => p.Name.ToLower() == "realizadas")!;
      Panel archivePanel = loadedPanels.FirstOrDefault(p => p.Name.ToLower() == "archivadas")!;

      if (archivePanel != null)
      {
        ShowTaskArchive showTaskArchive = new ShowTaskArchive(archivePanel, donePanel);
        showTaskArchive.TaskUnarchive += () =>
        {
          RefreshPanelTasks();
          LoadPanels(selectedBoard.BoardId);
        };
        showTaskArchive.ShowDialog();
      }
    }

    private void btnArchiveTasks_Click(object sender, RoutedEventArgs e)
    {
      PanelManager panelManager = new PanelManager();
      List<Panel> loadedPanels = panelManager.GetAllPanelsFromBoard(selectedBoard.BoardId);
      Panel donePanel = loadedPanels.FirstOrDefault(p => p.Name.ToLower() == "realizadas")!;
      Panel archivePanel = loadedPanels.FirstOrDefault(p => p.Name.ToLower() == "archivadas")!;

      if (donePanel != null)
      {
        ArchiveTaskDialog archiveTaskDialog = new ArchiveTaskDialog(donePanel, archivePanel);
        archiveTaskDialog.TasksArchive += () =>
        {
          RefreshPanelTasks();
          LoadPanels(selectedBoard.BoardId);
        };
        archiveTaskDialog.ShowDialog();
      }
    }
    private void RefreshPanelTasks()
    {
      TaskManager taskManager = new TaskManager();

      foreach (var panel in PanelList)
      {
        var tasks = taskManager.GetAllTasksFromPanel(panel.PanelId);
        panel.TaskList.Clear();
        foreach (var t in tasks)
          panel.TaskList.Add(t);
      }
    }

    private void OnTaskDoubleClick(object parameter)
    {
      if (parameter is Task task)
      {
        TaskDetailsDialog taskDetailsDialog = new TaskDetailsDialog(task);
        taskDetailsDialog.ShowDialog();
      }
    }
  }

  // Helper class for ICommand implementation
  public class RelayCommand : ICommand
  {
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }
  }
}
