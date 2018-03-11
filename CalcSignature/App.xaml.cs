using System.Windows;

namespace CalcSignature
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void app_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ComboBoxSignatureType.SelectedIndex = -1; // Undefined as initial selection

            // If no command line arguments were provided, don't process them
            if (e.Args.Length == 0) return;

            // Get command line arguments
            foreach (string argument in e.Args)
            {
                mainWindow.TextBoxFilePath.Text = argument;

                byte[] bytes = mainWindow.readFile(argument);
                byte[] result = mainWindow.computeMd5(bytes);
                string bytesAsString = mainWindow.byte2hexString(result, 16);
                mainWindow.TextBoxSignature.Text = bytesAsString;
                mainWindow.ComboBoxSignatureType.SelectedIndex = 0; // Select MD5
            }
        }
    }
}
