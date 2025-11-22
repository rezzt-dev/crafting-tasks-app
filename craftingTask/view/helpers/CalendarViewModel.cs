using craftingTask.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using craftingTask.model.objects;
using Task = craftingTask.model.objects.Task;

namespace craftingTask.view.helpers
{
  public class CalendarViewModel : INotifyPropertyChanged
  {
    public ObservableCollection<CalendarDay> Days { get; set; }
    public ObservableCollection<CalendarDay> WeekDays { get; set; }
    public ObservableCollection<Task> SelectedDayTasks { get; set; }
    public ObservableCollection<YearMonth> YearMonths { get; set; }

    public string CurrentMonthString => new DateTime(CurrentYear, CurrentMonth, 1)
        .ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

    public string CurrentWeekString
    {
      get
      {
        int weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(CurrentWeekStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        return $"{CurrentWeekStart.Year} | Semana {weekNum}";
      }
    }

    private string selectedDayString;
    public string SelectedDayString
    {
      get => selectedDayString;
      set
      {
        selectedDayString = value;
        OnPropertyChanged(nameof(SelectedDayString));
      }
    }

    private string selectedDayInfo;
    public string SelectedDayInfo
    {
      get => selectedDayInfo;
      set
      {
        selectedDayInfo = value;
        OnPropertyChanged(nameof(SelectedDayInfo));
      }
    }

    private int currentYear;
    public int CurrentYear
    {
      get => currentYear;
      set
      {
        currentYear = value;
        OnPropertyChanged(nameof(CurrentYear));
      }
    }

    public int CurrentMonth { get; set; }

    public DateTime CurrentWeekStart { get; set; }



    private List<Task> allTasks;

    // Propiedad de transición para animaciones
    private string transitionDirection = "Left";
    public string TransitionDirection
    {
      get => transitionDirection;
      set
      {
        transitionDirection = value;
        OnPropertyChanged(nameof(TransitionDirection));
      }
    }

    public CalendarViewModel(List<Task> tasks)
    {
      allTasks = tasks;
      SelectedDayTasks = new ObservableCollection<Task>();

      CurrentYear = DateTime.Now.Year;
      CurrentMonth = DateTime.Now.Month;

      Days = new ObservableCollection<CalendarDay>();
      WeekDays = new ObservableCollection<CalendarDay>();
      GenerateCalendar(CurrentYear, CurrentMonth);
      GenerateYearView(CurrentYear);

      // Initialize Week View to current week
      DateTime today = DateTime.Today;
      int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
      CurrentWeekStart = today.AddDays(-1 * diff).Date;
      GenerateWeekView(CurrentWeekStart);
    }

    public void GenerateCalendar(int year, int month)
    {
      if (Days == null) Days = new ObservableCollection<CalendarDay>();
      else Days.Clear();

      var firstDay = new DateTime(year, month, 1);
      int daysInMonth = DateTime.DaysInMonth(year, month);

      int dayOfWeek = (int)firstDay.DayOfWeek; // Domingo = 0
      int offset = (dayOfWeek == 0) ? 6 : dayOfWeek - 1; // Ajustamos lunes = 0

      // Placeholders antes del primer día
      for (int i = 0; i < offset; i++)
      {
        Days.Add(new CalendarDay { IsPlaceholder = true });
      }

      for (int i = 0; i < daysInMonth; i++)
      {
        DateTime date = firstDay.AddDays(i);
        var tasksForDay = new ObservableCollection<Task>(allTasks.Where(t => t.EndDate.Date == date.Date && t.StatusId != 4).ToList());

        var dots = new List<TaskDot>();
        foreach (var task in tasksForDay)
        {
          try
          {
            dots.Add(new TaskDot
            {
              Color = (SolidColorBrush)new BrushConverter().ConvertFromString(task.Color)
            });
          }
          catch
          {
            dots.Add(new TaskDot { Color = Brushes.Gray });
          }
        }

        Days.Add(new CalendarDay
        {
          Date = date,
          DayNumber = date.Day.ToString(),
          TaskDots = dots,
          Tasks = tasksForDay,
          IsSelected = false
        });
      }

      OnPropertyChanged(nameof(Days));
      OnPropertyChanged(nameof(CurrentMonthString));
    }

    public void GenerateYearView(int year)
    {
      YearMonths = new ObservableCollection<YearMonth>();

      for (int month = 1; month <= 12; month++)
      {
        var firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);

        var days = new ObservableCollection<CalendarDay>();

        // Relleno para alinear la semana (lunes = 0)
        int dayOfWeek = (int)firstDay.DayOfWeek;
        int offset = (dayOfWeek == 0) ? 6 : dayOfWeek - 1;
        for (int i = 0; i < offset; i++)
          days.Add(new CalendarDay { IsPlaceholder = true });

        for (int i = 0; i < daysInMonth; i++)
        {
          DateTime date = firstDay.AddDays(i);
          var tasksForDay = new ObservableCollection<Task>(allTasks.Where(t => t.EndDate.Date == date.Date && t.StatusId != 4).ToList());

          var dots = new List<TaskDot>();
          foreach (var task in tasksForDay)
          {
            try { dots.Add(new TaskDot { Color = (SolidColorBrush)new BrushConverter().ConvertFromString(task.Color) }); }
            catch { dots.Add(new TaskDot { Color = Brushes.Gray }); }
          }

          days.Add(new CalendarDay
          {
            Date = date,
            DayNumber = date.Day.ToString(),
            TaskDots = dots,
            Tasks = tasksForDay,
            IsSelected = false,
            IsToday = date.Date == DateTime.Now.Date
          });
        }

        YearMonths.Add(new YearMonth
        {
          MonthName = firstDay.ToString("MMMM", new CultureInfo("es-ES")).ToUpper(),
          Days = days
        });
      }

      OnPropertyChanged(nameof(YearMonths));
    }

    public void SelectDay(CalendarDay day)
    {
      foreach (var d in Days)
        d.IsSelected = false;

      day.IsSelected = true;

      SelectedDayString = day.Date.ToString("dd MMMM", new CultureInfo("es-ES"));
      SelectedDayInfo = $"{day.Tasks.Count} tareas";

      SelectedDayTasks.Clear();
      foreach (var task in day.Tasks)
        SelectedDayTasks.Add(task);

      OnPropertyChanged(nameof(SelectedDayTasks));
    }

    public void SelectTask(Task task)
    {
      var culture = new CultureInfo("es-ES");
      string dayName = culture.TextInfo.ToTitleCase(task.EndDate.ToString("dddd", culture));
      SelectedDayString = $"{dayName} {task.EndDate.Day}";
      SelectedDayInfo = task.EndDate.ToString("dd MMMM yyyy", culture);

      SelectedDayTasks.Clear();
      SelectedDayTasks.Add(task);

      OnPropertyChanged(nameof(SelectedDayTasks));
    }

    public void GoToNextMonth()
    {
      TransitionDirection = "Left";
      CurrentMonth++;
      if (CurrentMonth > 12) { CurrentMonth = 1; CurrentYear++; }

      GenerateCalendar(CurrentYear, CurrentMonth);
      OnPropertyChanged(nameof(CurrentMonthString));
    }

    public void GoToPrevMonth()
    {
      TransitionDirection = "Right";
      CurrentMonth--;
      if (CurrentMonth < 1) { CurrentMonth = 12; CurrentYear--; }

      GenerateCalendar(CurrentYear, CurrentMonth);
      OnPropertyChanged(nameof(CurrentMonthString));
    }

    public void GoToNextYear()
    {
      CurrentYear++;
      GenerateYearView(CurrentYear);
    }

    public void GoToPrevYear()
    {
      CurrentYear--;
      GenerateYearView(CurrentYear);
    }

    public void GenerateWeekView(DateTime weekStart)
    {
      if (WeekDays == null) WeekDays = new ObservableCollection<CalendarDay>();
      else WeekDays.Clear();

      for (int i = 0; i < 7; i++)
      {
        DateTime date = weekStart.AddDays(i);
        var tasksForDay = new ObservableCollection<Task>(allTasks.Where(t => t.EndDate.Date == date.Date && t.StatusId != 4).ToList());

        WeekDays.Add(new CalendarDay
        {
          Date = date,
          DayNumber = date.Day.ToString(),
          DayName = date.ToString("dddd", new CultureInfo("es-ES")).ToUpper(),
          Tasks = tasksForDay,
          IsToday = date.Date == DateTime.Now.Date
        });
      }
      OnPropertyChanged(nameof(WeekDays));
      OnPropertyChanged(nameof(CurrentWeekString));
    }

    public void GoToNextWeek()
    {
      CurrentWeekStart = CurrentWeekStart.AddDays(7);
      GenerateWeekView(CurrentWeekStart);
    }

    public void GoToPrevWeek()
    {
      CurrentWeekStart = CurrentWeekStart.AddDays(-7);
      GenerateWeekView(CurrentWeekStart);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}
