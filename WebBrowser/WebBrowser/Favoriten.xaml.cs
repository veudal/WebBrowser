using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for Favoriten.xaml
    /// </summary>
    public partial class Favoriten : Window
    {
        private MainWindow _mainWindow;
        public Favoriten(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            if(!File.Exists(path + "confirmation"))
            {
                Directory.CreateDirectory(path + "confirmation");
            }
            if (!File.Exists(path + "confirmation\\favoritesConfirmation.txt"))
            {
                File.WriteAllText(path + "confirmation\\favoritesConfirmation.txt", "y");
            }


        }

        public Favoriten()
        {
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            if (!File.Exists(path + "confirmation"))
            {
                Directory.CreateDirectory(path + "confirmation");
            }
            if (!File.Exists(path + "confirmation\\favoritesConfirmation.txt"))
            {
                File.WriteAllText(path + "confirmation\\favoritesConfirmation.txt", "y");
            }


        }

        private void explorer_Click(object sender, RoutedEventArgs e)
        {
            string path4Explorer = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\webbrowser\favorites";
            Process.Start("explorer.exe", path4Explorer);
        }

        private void favoritesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)favoritesList.SelectedItem != null)
            {
                try
                {
                    string url = (string)favoritesList.SelectedItem;
                    _mainWindow.webbrowser.Navigate(url);
                }
                catch
                {

                }
                try
                {

                    Clipboard.SetText((string)favoritesList.SelectedItem);
                }
                catch
                {

                }

            }
        }

        private void Window_Activated_1(object sender, EventArgs e)
        {
            favoritesList.Items.Clear();
            string content = string.Empty;
            string favoPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\favorites\\";
            foreach (string file in Directory.EnumerateFiles(favoPath))
            {
                content = File.ReadAllText(file);
                favoritesList.Items.Add(content.Replace("⇕", "/").Replace("↓", ":").Replace("⇋", "?").Replace("⇻", @"\").Replace("☈", "*").Replace("⇜", "\"").Replace("⇠","<").Replace("⇼", ">").Replace("⇴", "|"));
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\confirmation\\favoritesConfirmation.txt";
            if (File.ReadAllText(path) == "y")
            {
                conCeck.IsChecked = true;
            }
            else
            {
                conCeck.IsChecked = false;
            }

        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = new MessageBoxResult();
            string confirmation = string.Empty;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\";
            confirmation = File.ReadAllText(path + "confirmation\\favoritesConfirmation.txt");       
            if (confirmation == "y")
            {
                result = MessageBox.Show("Bist du dir sicher, dass du alle Favoriten entfernen möchtest?", "Bist du dir sicher?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(path + "favorites");

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    favoritesList.Items.Clear();
                }
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path + "favorites");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                favoritesList.Items.Clear();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\confirmation\\favoritesConfirmation.txt";
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
