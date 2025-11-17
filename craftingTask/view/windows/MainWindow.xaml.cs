using craftingTask.model.objects;
using craftingTask.persistence.managers;
using craftingTask.view.frame_pages;
using craftingTask.view.windows.windows_dialogs;
using SQLitePCL;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace craftingTask
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public ObservableCollection<Board> BoardList { get; set; }
    private BoardManager mainBoardManager;

    public MainWindow()
    {
      InitializeComponent();

      mainBoardManager = new BoardManager();
      BoardList = new ObservableCollection<Board>(mainBoardManager.GetAllBoards());
      DataContext = this;
    }

    private void BoardsFrame_Navigated(object sender, NavigationEventArgs e)
    {
      if (e.Content is CalendarPage)
        btnCalendarView.IsEnabled = false;
      else
        btnCalendarView.IsEnabled = true;
    }

    private void btnMinimizeWindow(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void btnMaximizeWindow(object sender, RoutedEventArgs e)
    {
      if (this.WindowState == WindowState.Maximized)
      {
        this.WindowState = WindowState.Normal;
      }
      else
      {
        this.WindowState = WindowState.Maximized;
      }
    }

    private void btnCloseWindow(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var selectedBoard = ((ListViewItem)sender).Content as Board;
      if (selectedBoard != null)
      {
        Board mainSelectedBoard = selectedBoard;
        BoardPage boardPage = new BoardPage(mainSelectedBoard);
        this.boardsFrame.Navigate(boardPage);
      }
    }

    private void btnCreateBoard_Click(object sender, RoutedEventArgs e)
    {
      CreateBoardDialog createBoardDialog = new CreateBoardDialog();
      if (createBoardDialog.ShowDialog() == true)
      {
        BoardList.Add(createBoardDialog.CreatedBoard);
      }
    }

    private void btnDeleteBoards_Click(object sender, RoutedEventArgs e)
    {
      DeleteBoardsDialog deleteBoardsDialog = new DeleteBoardsDialog(BoardList);
      deleteBoardsDialog.ShowDialog();
    }

    private void btnCalendarView_Click(object sender, RoutedEventArgs e)
    {
      CalendarPage calendarPage = new CalendarPage();
      this.boardsFrame.Navigate(calendarPage);
    }
  }
}