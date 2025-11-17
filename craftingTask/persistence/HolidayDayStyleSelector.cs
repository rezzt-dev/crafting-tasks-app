using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace craftingTask.persistence
{
  class HolidayDayStyleSelector : StyleSelector
  {
    public Style NormalDayStyle { get; set; }
    public Style HolidayDayStyle { get; set; }

    private static readonly HashSet<DateTime> Festivos = new HashSet<DateTime>
    {
      new DateTime(DateTime.Now.Year, 1, 1),
      new (DateTime.Now.Year, 12, 25),
    };

    public override Style SelectStyle(object item, DependencyObject container)
    {
      if (item is DateTime date)
      {
        if (Festivos.Contains(date.Date))
        {
          return HolidayDayStyle;
        }
      }
      return NormalDayStyle;
    }
  }
}
