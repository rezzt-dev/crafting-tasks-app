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
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.windows.windows_dialogs.task_dialogs
{
  /// <summary>
  /// Lógica de interacción para TaskDetailsDialog.xaml
  /// </summary>
  public partial class TaskDetailsDialog : Window
  {
    private Task selectedTask;

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
