using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftingTask.model.objects
{
  public class YearMonth
  {
    public string MonthName { get; set; }
    public ObservableCollection<CalendarDay> Days { get; set; }
  }
}
