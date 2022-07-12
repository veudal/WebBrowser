using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for Verlauf.xaml
    /// </summary>
    public partial class Verlauf : Window
    {
        private MainWindow _mainWindow;

        public Verlauf(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            if (!File.Exists(path + "confirmation"))
            {
                Directory.CreateDirectory(path + "confirmation");
            }
            if (!File.Exists(path + "confirmation\\historyConfirmation.txt"))
            {
                File.WriteAllText(path + "confirmation\\historyConfirmation.txt", "y");
            }

        }

        public Verlauf()
        {
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            if (!File.Exists(path + "confirmation"))
            {
                Directory.CreateDirectory(path + "confirmation");
            }
            if (!File.Exists(path + "confirmation\\historyConfirmation.txt"))
            {
                File.WriteAllText(path + "confirmation\\historyConfirmation.txt", "y");
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            historyList.Items.Clear();
            int i = 1;
            string hisPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\search\\";
            while (File.Exists(hisPath + "h" + i + ".txt"))
            {
                if (!File.ReadAllText(hisPath + "h" + i + ".txt").Contains("https://www.bing.com/?cc=de"))
                {
                    historyList.Items.Add(File.ReadAllText(hisPath + "h" + i + ".txt"));
                    
                }
                i++;
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\confirmation\\historyConfirmation.txt";
            if (File.ReadAllText(path) == "y")
            {
                conCeck.IsChecked = true;
            }
            else
            {
                conCeck.IsChecked = false;
            }
        }

        private void explorer_Click(object sender, RoutedEventArgs e)
        {
            string path4Explorer = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\webbrowser\search";
            Process.Start("explorer.exe", path4Explorer);
        }

        private void historyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)historyList.SelectedItem != null)
            {
                try
                {
                    string url = (string)historyList.SelectedItem;
                    _mainWindow.webbrowser.Navigate(url);
                    
                }
                catch
                {

                }
                try
                {

                    Clipboard.SetText((string)historyList.SelectedItem);
                }
                catch
                {

                }

            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = new MessageBoxResult();
            string confirmation = string.Empty;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            confirmation = File.ReadAllText(path + "confirmation\\historyConfirmation.txt");
            if (confirmation == "y")
            {
                result = MessageBox.Show("Bist du dir sicher, dass du alle Suchanfragen entfernen möchtest?", "Bist du dir sicher?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(path + "search");

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    historyList.Items.Clear();
                }
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path + "search");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                historyList.Items.Clear();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\confirmation\\historyConfirmation.txt";
            if (File.ReadAllText(path) == "y")
            {
                conCeck.IsChecked = false;
                File.WriteAllText(path, "n");
            }
            else
            {
                conCeck.IsChecked = true;
                File.WriteAllText(path, "y");
            }
        }
    }
}
