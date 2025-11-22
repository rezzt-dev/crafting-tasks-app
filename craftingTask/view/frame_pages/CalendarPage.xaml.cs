using craftingTask.model;
using craftingTask.persistence;
using craftingTask.persistence.managers;
using craftingTask.view.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.frame_pages
{
  /// <summary>
  /// Lógica de interacción para CalendarPage.xaml
  /// </summary>
  public partial class CalendarPage : Page
  {
    private CalendarViewModel vm;

    public CalendarPage()
    {
      InitializeComponent();

      TaskManager taskManager = new TaskManager();
      List<Task> tasks = taskManager.GetAllTasks();

      vm = new CalendarViewModel(tasks);
      DataContext = vm;

      CalendarDay today = vm.Days.FirstOrDefault(d => d.Date.Date == DateTime.Today);
      if (today != null)
      {
        vm.SelectDay(today);
      }

      // Actualizamos los Tags de los días cuando cambian los días
      vm.Days.CollectionChanged += (s, e) => UpdateDayTags();
      UpdateDayTags();
    }

    private void btnCloseCalendarFrame(object sender, RoutedEventArgs e)
    {
      Frame? parentFrame = HelpMethods.FindParentFrame(this);
      if (parentFrame != null)
      {
        parentFrame.Content = null;
      }
    }

    private void BtnMonthly_Click(object sender, RoutedEventArgs e)
    {
      MonthlyPanel.Visibility = Visibility.Visible;
      YearlyPanel.Visibility = Visibility.Collapsed;
      WeeklyPanel.Visibility = Visibility.Collapsed;
    }

    private void BtnYearly_Click(object sender, RoutedEventArgs e)
    {
      MonthlyPanel.Visibility = Visibility.Collapsed;
      YearlyPanel.Visibility = Visibility.Visible;
      WeeklyPanel.Visibility = Visibility.Collapsed;
    }

    private void BtnWeekly_Click(object sender, RoutedEventArgs e)
    {
      MonthlyPanel.Visibility = Visibility.Collapsed;
      YearlyPanel.Visibility = Visibility.Collapsed;
      WeeklyPanel.Visibility = Visibility.Visible;
    }


    private void UpdateDayTags()
    {
      foreach (var item in CalendarItems.Items)
      {
        var container = CalendarItems.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
        if (container != null)
        {
          var border = FindVisualChild<Border>(container);
          if (border != null)
            border.Tag = item as CalendarDay;
        }
      }
    }

    private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
      if (parent == null) return null;

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is T tChild) return tChild;
        var result = FindVisualChild<T>(child);
        if (result != null) return result;
      }
      return null;
    }

    private void Day_Click(object sender, MouseButtonEventArgs e)
    {
      Border border = sender as Border ?? FindParent<Border>(sender as DependencyObject);
      if (border == null) return;

      if (border.DataContext is CalendarDay day)
      {
        (DataContext as CalendarViewModel)?.SelectDay(day);
      }
    }

    private void Task_Click(object sender, MouseButtonEventArgs e)
    {
      Border border = sender as Border ?? FindParent<Border>(sender as DependencyObject);
      if (border == null) return;

      if (border.DataContext is Task task)
      {
        vm.SelectTask(task);
      }
    }

    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(child);
      while (parent != null && !(parent is T))
      {
        parent = VisualTreeHelper.GetParent(parent);
      }
      return parent as T;
    }

    private void NextMonth_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToNextMonth();
    }

    private void PrevMonth_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToPrevMonth();
    }

    private void NextYear_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToNextYear();
    }

    private void PrevYear_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToPrevYear();
    }

    private void NextWeek_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToNextWeek();
    }

    private void PrevWeek_Click(object sender, MouseButtonEventArgs e)
    {
      vm.GoToPrevWeek();
    }

    private void CalendarItems_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
      Storyboard slide = vm.TransitionDirection == "Left"
          ? (Storyboard)FindResource("SlideLeftStoryboard")
          : (Storyboard)FindResource("SlideRightStoryboard");

      slide?.Begin(CalendarGrid);
    }

    private void Days_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
      var control = sender as ItemsControl;
      control.IsHitTestVisible = true;
    }
  }
}
