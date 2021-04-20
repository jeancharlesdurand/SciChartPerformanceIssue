using System.Windows;
using SciChart.Charting.Visuals;
using SciChart.Examples.ExternalDependencies.Controls.ExceptionView;

namespace SciChartExport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
		     DispatcherUnhandledException += App_DispatcherUnhandledException;

			 InitializeComponent();
             SciChartSurface.SetRuntimeLicenseKey("6dW1YtA9mdnb+CGuyX9oBxlfCaAWGhivKEzCLEUdv94ANJlQgtxzTyQiSTVfWiwfrvIouTRo6U3w0r/I8Xnx3mOBF6L38bpm7lNGradT90p6PODAI7rTRpIyMhVWibl2skrwqxy9qKb7lVPxPrel+G+ZkNr0migAb39YpY2msLBR8npZ32XOn9h8D/oM3Z7ry2KmxG4+HqoqR7giguDmPUcTONXRh276yUc7FkEA8ILxGHHYRt3DhVowq7pHycOPOLW4cKfLHJiP0cqkw5sy66LnWazwtg11wKL+EFuPliVfCbejK/FgCQSbP33vWGfjUKCEIWfKy+ESJqp9mXEa8AzlGkqjPJLNHwszxBG3JXCPjvG+WjFkibs8Wk9O0/q9ouTN9mlk3buZAtX/zQEGhWRIqCExn3yA7aXpgidpY7Yp+ak/1JlLqWl7335Px1qd124N0BppuyujyXAVCP/zNK+/0g==");

            // TODO: Put your SciChart License Key here if needed
            // SciChartSurface.SetRuntimeLicenseKey(@"{YOUR SCICHART WPF v6 LICENSE KEY}");
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {            
            var exceptionView = new ExceptionView(e.Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            exceptionView.ShowDialog();

            e.Handled = true;
        }
    }
}
