using craftingTask.model.objects;
using craftingTask.persistence.managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  /// Lógica de interacción para CreatePanelDialog.xaml
  /// </summary>
  public partial class CreatePanelDialog : Window, INotifyPropertyChanged
  {
    private Brush _selectedColorBrush;
    private string _selectedColorHex;
    public event PropertyChangedEventHandler PropertyChanged;

    public Board SelectedBoard { get; private set; }
    public Panel CreatedPanel { get; private set; }

    public Brush SelectedColorBrush
    {
      get => _selectedColorBrush;
      set
      {
        _selectedColorBrush = value;
        OnPropertyChanged(nameof(SelectedColorBrush));
      }
    }
    public string SelectedColorHex
    {
      get => _selectedColorHex;
      set
      {
        _selectedColorHex = value;
        OnPropertyChanged(nameof(SelectedColorHex));
      }
    }
    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public CreatePanelDialog(Board inputBoard)
    {
      InitializeComponent();
      DataContext = this;

      SelectedColorBrush = new SolidColorBrush(Color.FromRgb(58, 58, 58));
      SelectedColorHex = "#3A3A3A";

      this.SelectedBoard = inputBoard;

      ModernPicker.ColorAccepted += (color) =>
      {
        SelectedColorBrush = new SolidColorBrush(color);
        SelectedColorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        ColorPopup.IsOpen = false;
      };

      ModernPicker.Cancelled += () =>
      {
        ColorPopup.IsOpen = false;
      };
    }

    private void btnMinimizeWindow_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }
    private void btnCancelCreation_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void btnCreatePanel_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtPanelNameSelector.Text) || string.IsNullOrWhiteSpace(txtColorSelector.Text))
      {
        MessageBox.Show("El nombre y el color del panel no pueden estar vacíos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      string panelBoardColor = SelectedColorHex;
      string panelName = txtPanelNameSelector.Text;
      long boardId = SelectedBoard.BoardId;

      Panel panel = new Panel(boardId, panelName, panelBoardColor);
      panel.Add();

      this.CreatedPanel = panel;
      this.DialogResult = true;
      this.Close();
    }

    private void BtnOpenColor_Click(object sender, RoutedEventArgs e)
    {
      if (ModernPicker != null)
      {
        ModernPicker.ColorHex = SelectedColorHex;
      }
      ColorPopup.IsOpen = true;
    }
    private void ApplyColorFromPicker()
    {
      if (ModernPicker == null) return;
      SelectedColorHex = ModernPicker.ColorHex;
      SelectedColorBrush = ModernPicker.SelectedBrush;
      DataContext = null;
      DataContext = this;
    }
    private void ColorPopup_Closed(object sender, EventArgs e)
    {
      ApplyColorFromPicker();
    }
  }
}
