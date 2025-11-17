using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace craftingTask.persistence
{
  public class PriorityToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is int priority)
      {
        switch (priority)
        {
          case 1:
            return "Alta";
          case 2:
            return "Media";
          case 3:
            return "Baja";
          default:
            return "Inmediata";
        }
      }
      return "Desconocida";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string str)
      {
        switch (str)
        {
          case "Alta":
            return 1;
          case "Media":
            return 2;
          case "Baja":
            return 3;
          default:
            return 4;
        }
      }
      return 0;
    }
  }
}
