using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using IWshRuntimeLibrary;
using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        bool accessDenied = false;
        private Verlauf verlauf;
        private Favoriten favoriten;
        string searchEngine;
       
        public MainWindow()
        {
            InitializeComponent();
            string exePath = Directory.GetCurrentDirectory() + "\\webbrowser.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
            startInfo.Verb = "runas";
            string chkLNK = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\WebBrowser.lnk";
            if (!System.IO.File.Exists(chkLNK))
            {
                if ((Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() < 2))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(startInfo);
                        Environment.Exit(1);
                    }
                    catch
                    {
                        accessDenied = true;
                    }

                }


                string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                string appStartMenuPath = System.IO.Path.Combine(commonStartMenuPath, "Programs");
                if (accessDenied == true)
                {
                    MessageBox.Show("Um den Browser an 'Start' zu heften musst du das Programm als Administrator ausführen");
                }
                if (accessDenied == false)
                {
                    AddShortcut();
                }
            }
            dynamic activeX = this.webbrowser.GetType().InvokeMember("ActiveXInstance", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, this.webbrowser, new object[] { });
            activeX.Silent = true;
            searchBox.BorderBrush = Brushes.Gray;
            string content = string.Empty;
            favorite_Clicked.Visibility = Visibility.Hidden;
            favoriten = new Favoriten(this);
            verlauf = new Verlauf(this);
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.SingleBorderWindow;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + "\\favorites");
            string szenario = string.Empty;
            if (System.IO.File.Exists(path + "\\backup.txt"))
            {
                szenario = System.IO.File.ReadAllText(path + "\\backup.txt");
            }
            else
            {
                System.IO.File.WriteAllText(path + "\\backup.txt", "3");
            }
            if (!System.IO.File.Exists(path + "\\engine.txt"))
            {
                bing.IsChecked = true;
                google.IsChecked = false;
                System.IO.File.WriteAllText(path + "\\engine.txt", "bing");
            }
            if (System.IO.File.ReadAllText(path + "\\engine.txt") == "google")
            {
                google.IsChecked = true;
                bing.IsChecked = false;
                searchBox.Text = "https://www.google.de/#spf=1603209159157";
                webbrowser.Navigate("https://www.google.de/#spf=1603209159157");
                searchEngine = "https://www.google.de/search?sxsrf=ALeKk03PlC82J81Omoe67U-d5y_3JmW7XA%3A1603028519848&source=hp&ei=J0aMX5O9MdycjLsPz8ixqAI&q=";
            }
            else
            {
                searchBox.Text = "https://www.bing.com/?cc=de";
                webbrowser.Navigate("https://www.bing.com/?cc=de");
                bing.IsChecked = true;
                google.IsChecked = false;
            }
            searchBox.TextWrapping = TextWrapping.NoWrap;
            webbrowser.Navigated += Webbrowser_Navigated;
            if (szenario != "2")
            {
                if (szenario == "3")
                {
                    _3.IsChecked = true;
                    if (System.IO.File.Exists(path + "\\page.txt"))
                    {
                        if (System.IO.File.ReadAllText(path + "\\page.txt") != "https://www.bing.com/?cc=de")
                        {
                            var result = System.Windows.MessageBox.Show("Gespeicherte Seite gefunden", "Möchtest du die letzte Seite wiederherstellen?", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                string pageContent = System.IO.File.ReadAllText(path + "\\page.txt");
                                try
                                {
                                    webbrowser.Navigate(pageContent);
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                System.IO.File.Delete(path + "\\page.txt");
                            }
                        }
                    }
                }
                else
                {
                    if (System.IO.File.Exists(path + "\\page.txt"))
                    {
                        _1.IsChecked = true;
                        string pageContent = System.IO.File.ReadAllText(path + "\\page.txt");
                        webbrowser.Navigate(pageContent);
                    }
                }
            }
            else
            {
                _2.IsChecked = true;
            }
        }

        private static void AddShortcut()
        {
            string pathToExe = Directory.GetCurrentDirectory() + "\\WebBrowser.exe";
            string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string appStartMenuPath = System.IO.Path.Combine(commonStartMenuPath, "Programs");

            if (!Directory.Exists(appStartMenuPath))
                Directory.CreateDirectory(appStartMenuPath);

            string shortcutLocation = Path.Combine(appStartMenuPath, "WebBrowser" + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Webbrowser mit dem man im Web surfen kann";
            //shortcut.IconLocation = @"C:\Program Files (x86)\TestApp\TestApp.ico"; //uncomment to set the icon of the shortcut
            shortcut.TargetPath = pathToExe;
            shortcut.Save();
        }

        private void Webbrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            favoriteBlock.Foreground = Brushes.White;
            favorite_Clicked.Visibility = Visibility.Hidden;
            searchBox.Text = webbrowser.Source.ToString();// e.Uri.ToString();
            searchBox.UpdateLayout();
            this.UpdateLayout();

            string hisPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\search\\";
            Directory.CreateDirectory(hisPath);
            int i = 1;
            while (System.IO.File.Exists(hisPath + "h" + i + ".txt"))
            {
                i++;
            }
            if (Convert.ToString(searchBox.Text) != "about:blank")
            {
                if (Convert.ToString(searchBox.Text) != "https://www.bing.com/?cc=de")
                {
                    if (!Convert.ToString(searchBox.Text).Contains("google.de/#spf"))
                    {
                        System.IO.File.WriteAllText(hisPath + "h" + i + ".txt", searchBox.Text);
                    }
                }
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            string content = string.Empty;
            foreach (string file in Directory.EnumerateFiles(path + "\\favorites"))
            {
                content = System.IO.File.ReadAllText(file);
                string searchBoxContent = searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace(" ? ", "⇋").Replace(@"\", "⇻").Replace(" * ", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴");
                if (content == searchBoxContent)
                {
                    favoriteBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(134, 191, 235));
                    favorite_Clicked.Visibility = Visibility.Visible;
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            webbrowser.Refresh();
        }

        public void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            string url = searchBox.Text;
            if (System.IO.File.ReadAllText(path + "\\engine.txt") == "google")
            {
                searchEngine = "https://www.google.de/search?sxsrf=ALeKk03PlC82J81Omoe67U-d5y_3JmW7XA%3A1603028519848&source=hp&ei=J0aMX5O9MdycjLsPz8ixqAI&q=";
            }
            else
            {
                searchEngine = "https://www.bing.de/search?q=";
            }
            try
            {
                webbrowser.Navigate(url);
                searchBox.Text = url;
            }
            catch
            {
                if (url.Contains("."))
                {
                    try
                    {
                        webbrowser.Navigate("https://" + url);
                        searchBox.Text = "https://" + url;
                    }
                    catch
                    {
                        try
                        {
                            webbrowser.Navigate("http://" + url);
                            searchBox.Text = "http://" + url;
                        }
                        catch
                        {
                            webbrowser.Navigate(searchEngine + url);
                            searchBox.Text = searchEngine + url;
                        }
                    }
                }
                else
                {
                    webbrowser.Navigate(searchEngine + url);
                    searchBox.Text = searchEngine + url;
                }
            }

        }

        private void searchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchButton_Click(this, new RoutedEventArgs());
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void bing_Click(object sender, RoutedEventArgs e)
        {
            google.IsChecked = false;
            bing.IsChecked = true;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\engine.txt", "bing");
            webbrowser.Navigate("https://www.bing.com/?cc=de");



        }

        private void google_Click(object sender, RoutedEventArgs e)
        {
            google.IsChecked = true;
            bing.IsChecked = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\engine.txt", "google");
            webbrowser.Navigate("https://www.google.de/");
            //searchBox.Text = Convert.ToString(webbrowser.Source);
            //searchBox.UpdateLayout();
            //this.UpdateLayout();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int count = 1;
            while (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\download" + count + ".html"))
            {
                count++;
            }
            string targetLocation = @"C:\Users\armul\OneDrive\Desktop\download" + count + ".html";
            string url = Convert.ToString(webbrowser.Source);
            WebClient download = new WebClient();
            try
            {
                download.DownloadFile(url, targetLocation);
                System.Windows.MessageBox.Show("Website wure unter download" + count + ".html auf dem Destop gespeichert.");
            }
            catch
            {
                System.Windows.MessageBox.Show("Download fehlgeschlagen");
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (webbrowser.CanGoBack)
            {
                webbrowser.GoBack();
            }
        }

        private void forwardButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (webbrowser.CanGoForward)
            {
                webbrowser.GoForward();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var verlauf = new Verlauf(this);
            verlauf.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            string page = searchBox.Text;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\page.txt", page);
            e.Cancel = false;
            Environment.Exit(1);
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            _2.IsChecked = false;
            _3.IsChecked = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\backup.txt", "1");
        }

        private void MenuItem_Checked_1(object sender, RoutedEventArgs e)
        {
            _1.IsChecked = false;
            _3.IsChecked = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\backup.txt", "2");
        }

        private void MenuItem_Checked_2(object sender, RoutedEventArgs e)
        {
            _1.IsChecked = false;
            _2.IsChecked = false;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            System.IO.File.WriteAllText(path + "\\backup.txt", "3");
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(searchBox.Text);
            }
            catch
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
                if (System.IO.File.ReadAllText(path + "\\engine.txt") == "google")
                {
                    searchEngine = "https://www.google.de/search?sxsrf=ALeKk03PlC82J81Omoe67U-d5y_3JmW7XA%3A1603028519848&source=hp&ei=J0aMX5O9MdycjLsPz8ixqAI&q=";
                }
                else
                {
                    searchEngine = "https://www.bing.de/search?q=";

                }
                System.Diagnostics.Process.Start(searchEngine + searchBox.Text);

            }
        }

        private void favorite_Click(object sender, RoutedEventArgs e)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser\\favorites";
            Directory.CreateDirectory(path);
            if (favorite_Clicked.Visibility == Visibility.Hidden)
            {
                favoriteBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(134, 191, 235));
                favorite_Clicked.Visibility = Visibility.Visible;
                try
                {
                    System.IO.File.WriteAllText(path + "\\" + searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace("?", "⇋").Replace(@"\", "⇻").Replace("*", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴"), searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace(" ? ", "⇋").Replace(@"\", "⇻").Replace(" * ", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴"));
                }
                catch
                {
                    try
                    {
                        System.IO.File.WriteAllText(path + "\\" + searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace("?", "⇋").Replace(@"\", "⇻").Replace("*", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴"), searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace(" ? ", "⇋").Replace(@"\", "⇻").Replace(" * ", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴"));
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Du URL ist zu lang und kann nicht gespeichert werden, versuche stattdessen deinen Link einzugeben, aber noch nicht die 'Enter'-Taste zu drücken.");
                    }
                }
            }
            else
            {
                System.IO.File.Delete(path + "\\" + searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace("?", "⇋").Replace(@"\", "⇻").Replace("*", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴"));
                favoriteBlock.Foreground = Brushes.White;
                favorite_Clicked.Visibility = Visibility.Hidden;
            }
        }

        private void Favorites_Click(object sender, RoutedEventArgs e)
        {
            var Favoriten = new Favoriten(this);
            Favoriten.Show();
        }



        private void Window_Activated(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
            Directory.CreateDirectory(path);
            string content = string.Empty;
            foreach (string file in Directory.EnumerateFiles(path + "\\favorites"))
            {
                content = System.IO.File.ReadAllText(file);
                string searchBoxContent = searchBox.Text.Replace("/", "⇕").Replace(":", "↓").Replace(" ? ", "⇋").Replace(@"\", "⇻").Replace(" * ", "☈").Replace("\"", "⇜").Replace("<", "⇠").Replace(">", "⇼").Replace("|", "⇴");
                if (content == searchBoxContent)
                {
                    favoriteBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(134, 191, 235));
                    favorite_Clicked.Visibility = Visibility.Visible;
                    break;
                }
                else
                {
                    favoriteBlock.Foreground = Brushes.White;
                    favorite_Clicked.Visibility = Visibility.Hidden;

                }
            }
            if (Directory.GetFiles(path + "\\favorites").Length == 0 && Directory.GetDirectories(path + "\\favorites").Length == 0)
            {
                favoriteBlock.Foreground = Brushes.White;
                favorite_Clicked.Visibility = Visibility.Hidden;
            }

        }

        private void searchBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            searchBox.SelectAll();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var Result = System.Windows.MessageBox.Show("Es werden alle gespeicherten Daten gelöscht (Favoriten, Verlauf und Einstellungen), anschließend wird der Dateipfad des Programms geöffnet und muss manuell gelöscht werden", "", MessageBoxButton.YesNo);
            if (Result == MessageBoxResult.Yes)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\webbrowser";
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(path);
                Process.Start(Directory.GetCurrentDirectory());
                if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\WebBrowser.lnk"))
                {
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs");
                    MessageBox.Show("Es wurde auch der Dateipfad zum löschen der Startmenü datei geöffnet.");
                }
                Environment.Exit(1);

            }
        }
    }

}
