using craftingTask.model.objects;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace craftingTask.view.windows.windows_dialogs
{
  /// <summary>
  /// Lógica de interacción para EditBoardDialog.xaml
  /// </summary>
  public partial class EditBoardDialog : Window, INotifyPropertyChanged
  {
    private Brush _selectedColorBrush;
    private string _selectedColorHex;
    private Board _originalBoard;
    public event PropertyChangedEventHandler PropertyChanged;

    public Board UpdatedBoard { get; private set; }

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


    public EditBoardDialog(Board board)
    {
      InitializeComponent();
      DataContext = this;

      _originalBoard = board;

      // Set read-only fields
      txtBoardName.Text = board.Name;
      txtCreationDate.Text = board.CreationDate.ToString("dd/MM/yyyy");

      // Set current color
      try
      {
        var color = (Color)ColorConverter.ConvertFromString(board.Color);
        SelectedColorBrush = new SolidColorBrush(color);
        SelectedColorHex = board.Color;
      }
      catch
      {
        SelectedColorBrush = new SolidColorBrush(Color.FromRgb(58, 58, 58));
        SelectedColorHex = "#3A3A3A";
      }

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

    private void btnMinimizeWindowClick(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }
    private void btnCloseWindowClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void btnCancelEditClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void btnSaveChangesClick(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtColorSelector.Text))
      {
        MessageBox.Show("El color del tablero no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (string.IsNullOrWhiteSpace(txtBoardName.Text))
      {
        MessageBox.Show("El nombre del tablero no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      // Update name and color
      _originalBoard.Name = txtBoardName.Text;
      _originalBoard.Color = SelectedColorHex;
      _originalBoard.ModificationDate = DateTime.UtcNow;
      _originalBoard.Update();

      this.UpdatedBoard = _originalBoard;
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
  }
}
