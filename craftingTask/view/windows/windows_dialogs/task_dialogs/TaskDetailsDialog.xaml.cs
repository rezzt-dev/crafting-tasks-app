using craftingTask.model.objects;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using craftingTask.persistence.managers;
using System.Collections.ObjectModel;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.windows.windows_dialogs.task_dialogs
{
  /// <summary>
  /// Lógica de interacción para TaskDetailsDialog.xaml
  /// </summary>
  public partial class TaskDetailsDialog : Window
  {
    private Task selectedTask;
    private SubtaskManager subtaskManager = new SubtaskManager();

    public TaskDetailsDialog(Task inputTask)
    {
      InitializeComponent();
      DataContext = this;
      this.selectedTask = inputTask;
      LoadTaskDetails();
    }

    private void LoadTaskDetails()
    {
      if (selectedTask == null)
        return;

      // Cargar título, descripción y tag
      txtTaskTitle.Text = selectedTask.Title;
      txtTaskDescription.Text = selectedTask.Description;
      txtTaskTag.Text = selectedTask.Tag;

      // Formatear y mostrar fechas
      txtCreationDate.Text = selectedTask.CreationDate.ToString("dd/MM/yyyy HH:mm");
      txtEndDate.Text = selectedTask.EndDate.ToString("dd/MM/yyyy");

      // Convertir prioridad a texto
      txtPriority.Text = ConvertPriorityToString(selectedTask.Priority);

      // Mostrar color del panel
      try
      {
        colorIndicator.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(selectedTask.Color));
      }
      catch
      {
        colorIndicator.Background = new SolidColorBrush(Colors.Gray);
      }

      LoadSubtasks();
    }

    private void LoadSubtasks()
    {
      var subtasks = subtaskManager.GetSubtasksForTask(selectedTask.TaskId);
      selectedTask.Subtasks = new ObservableCollection<Subtask>(subtasks);
      lstSubtasks.ItemsSource = selectedTask.Subtasks;
      UpdateProgress();
    }

    private void UpdateProgress()
    {
      if (selectedTask.Subtasks == null || selectedTask.Subtasks.Count == 0)
      {
        txtProgress.Text = "0/0";
        progressBar.Value = 0;
        return;
      }

      int total = selectedTask.Subtasks.Count;
      int completed = selectedTask.Subtasks.Count(s => s.IsCompleted);
      txtProgress.Text = $"{completed}/{total}";

      progressBar.Value = (double)completed / total * 100;
    }

    private void BtnAddSubtask_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtNewSubtask.Text)) return;

      var newSubtask = new Subtask(selectedTask.TaskId, txtNewSubtask.Text, selectedTask.Subtasks.Count);

      // Guardar en BD
      subtaskManager.AddSubtask(newSubtask);

      // Actualizar UI (Recargar es lo más seguro para obtener el ID generado)
      LoadSubtasks();
      txtNewSubtask.Text = "";
    }

    private void Subtask_Checked(object sender, RoutedEventArgs e)
    {
      var checkBox = sender as CheckBox;
      if (checkBox?.Tag is long subtaskId)
      {
        subtaskManager.UpdateSubtaskStatus(subtaskId, true);
        UpdateProgress();
      }
    }

    private void Subtask_Unchecked(object sender, RoutedEventArgs e)
    {
      var checkBox = sender as CheckBox;
      if (checkBox?.Tag is long subtaskId)
      {
        subtaskManager.UpdateSubtaskStatus(subtaskId, false);
        UpdateProgress();
      }
    }

    private void BtnDeleteSubtask_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      if (button?.Tag is long subtaskId)
      {
        if (MessageBox.Show("¿Eliminar subtarea?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          subtaskManager.DeleteSubtask(subtaskId);
          LoadSubtasks();
        }
      }
    }

    private string ConvertPriorityToString(int priority)
    {
      return priority switch
      {
        1 => "Baja",
        2 => "Media",
        3 => "Alta",
        4 => "Inmediata",
        _ => "Desconocida"
      };
    }

    private void btnMinimizeWindowClick(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void btnCloseWindowClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
