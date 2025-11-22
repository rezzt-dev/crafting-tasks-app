using System.Configuration;
using System.Data;
using System.Windows;

namespace craftingTask
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      // Add global exception handler
      this.DispatcherUnhandledException += (s, args) =>
      {
        MessageBox.Show($"Error: {args.Exception.Message}\n\nStack Trace:\n{args.Exception.StackTrace}",
                              "Application Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
        args.Handled = true;
      };
    }
  }

}
