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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace craftingTask
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public ObservableCollection<Board> BoardList { get; set; }
    private BoardManager mainBoardManager;
    private TaskManager taskManager;
    private DispatcherTimer notificationTimer;

    public MainWindow()
    {
      InitializeComponent();

      mainBoardManager = new BoardManager();
      taskManager = new TaskManager();
      BoardList = new ObservableCollection<Board>(mainBoardManager.GetAllBoards());
      DataContext = this;

      this.Loaded += MainWindow_Loaded;
    }
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
      timer.Tick += (s, args) =>
      {
        timer.Stop();
        ShowTaskNotification();
      };
      timer.Start();
    }

    private void BoardsFrame_Navigated(object sender, NavigationEventArgs e)
    {
      if (e.Content is CalendarPage)
        btnCalendarView.IsEnabled = false;
      else
        btnCalendarView.IsEnabled = true;

      if (e.Content is RemindersPage)
        btnShowReminders.IsEnabled = false;
      else
        btnShowReminders.IsEnabled = true;
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
        this.mainBorder.Padding = new Thickness(0);
      }
      else
      {
        this.WindowState = WindowState.Maximized;
        this.mainBorder.Padding = new Thickness(10);
      }
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

    #region ButtonFunctions 

    private void btnCloseWindow(object sender, RoutedEventArgs e)
    {
      this.Close();
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

    private void btnShowReminders_Click(object sender, RoutedEventArgs e)
    {
      RemindersPage remindersPage = new RemindersPage();
      this.boardsFrame.Navigate(remindersPage);
    }

    private async void btnOpenSearch_Click(object sender, RoutedEventArgs e)
    {
      await System.Threading.Tasks.Task.Delay(50);
      var searchWindow = new SearchWindow();
      searchWindow.ShowDialog();
    }

    #endregion

    #region PopUpFunctions

    private void ShowTaskNotification()
    {
      try
      {
        var upcomingTasks = taskManager.GetUpcomingTasks();

        int taskCount = upcomingTasks.Count;

        if (taskCount > 0)
        {
          if (taskCount == 1)
          {
            txtNotificationMessage.Text = "Tienes 1 tarea próxima a vencer en los próximos 4 días.";
          }
          else
          {
            txtNotificationMessage.Text = $"Tienes {taskCount} tareas próximas a vencer en los próximos 4 días.";
          }

          double windowWidth = contentArea.ActualWidth;
          double windowHeight = contentArea.ActualHeight;
          double popupWidth = 300;

          NotificationPopup.HorizontalOffset = windowWidth - popupWidth - 55;
          NotificationPopup.VerticalOffset = windowHeight - 160;

          NotificationPopup.IsOpen = true;
          AnimatePopupIn();

          notificationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
          notificationTimer.Tick += (s, args) =>
          {
            notificationTimer.Stop();
            AnimatePopupOut();
          };
          notificationTimer.Start();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Error al mostrar notificación: {ex.Message}");
      }
    }

    private void AnimatePopupIn()
    {
      var popup = NotificationPopup.Child as Border;
      if (popup != null)
      {
        var slideAnimation = new DoubleAnimation
        {
          From = 50,
          To = 0,
          Duration = TimeSpan.FromMilliseconds(400),
          EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var fadeAnimation = new DoubleAnimation
        {
          From = 0,
          To = 1,
          Duration = TimeSpan.FromMilliseconds(400)
        };

        var transform = new TranslateTransform();
        popup.RenderTransform = transform;

        transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
        popup.BeginAnimation(OpacityProperty, fadeAnimation);
      }
    }

    private void AnimatePopupOut()
    {
      var popup = NotificationPopup.Child as Border;
      if (popup != null)
      {
        var fadeAnimation = new DoubleAnimation
        {
          From = 1,
          To = 0,
          Duration = TimeSpan.FromMilliseconds(300)
        };

        fadeAnimation.Completed += (s, e) => NotificationPopup.IsOpen = false;
        popup.BeginAnimation(OpacityProperty, fadeAnimation);
      }
    }

    private void btnCloseNotification_Click(object sender, RoutedEventArgs e)
    {
      if (notificationTimer != null)
      {
        notificationTimer.Stop();
      }
      AnimatePopupOut();
    }

    private void btnViewReminders_Click(object sender, RoutedEventArgs e)
    {
      if (notificationTimer != null)
      {
        notificationTimer.Stop();
      }
      NotificationPopup.IsOpen = false;

      RemindersPage remindersPage = new RemindersPage();
      this.boardsFrame.Navigate(remindersPage);
    }

    #endregion
  }
}