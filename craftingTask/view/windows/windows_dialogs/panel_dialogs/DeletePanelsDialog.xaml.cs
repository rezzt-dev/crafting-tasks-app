using craftingTask.model.objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Panel = craftingTask.model.objects.Panel;

namespace craftingTask.view.windows.windows_dialogs.panel_dialogs
{
  /// <summary>
  /// Lógica de interacción para DeletePanelsDialog.xaml
  /// </summary>
  public partial class DeletePanelsDialog : Window
  {
    public ObservableCollection<Panel> PanelList { get; set; }
    public List<Panel> FilteredPanels { get; set; }
    public List<Panel> SelectedPanels => panelsListBox.SelectedItems.Cast<Panel>().ToList();

    public DeletePanelsDialog(ObservableCollection<Panel> panelList, List<Panel> filteredPanels)
    {
      InitializeComponent();
      PanelList = panelList;
      FilteredPanels = filteredPanels;
      DataContext = this;
    }

    private void btnMinimizeWindow_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }
    private void btnCancelDelete_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void btnDeletePanels_Click(object sender, RoutedEventArgs e)
    {
      if (SelectedPanels.Count == 0)
      {
        MessageBox.Show("Debes seleccionar al menos un tablero para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var result = MessageBox.Show($"¿Seguro que quieres eliminar {SelectedPanels.Count} paneles?", "Confirmar borrado", MessageBoxButton.YesNo, MessageBoxImage.Question);
      if (result == MessageBoxResult.Yes)
      {
        foreach (var panel in SelectedPanels)
        {
          panel.Remove();
          PanelList.Remove(panel);
        }
        this.Close();
      }
    }
  }
}
