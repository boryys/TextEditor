using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class TextFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }

    public partial class MainWindow : Window
    {
        public int counter = 0;
        public ObservableCollection<TextFile> FilesList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            FilesList = new ObservableCollection<TextFile>();
            this.DataContext = FilesList;
        }

        private void MenuItem_New(object sender, RoutedEventArgs e)
        {
            counter++;
            TabItem tab = new TabItem();
            tab.Header = string.Format("New file {0}", counter);
            tab.Name = string.Format("tab{0}", counter);

            System.Windows.Controls.RichTextBox c = new System.Windows.Controls.RichTextBox();
            tab.Content = c;
            tab.IsSelected = true;

            TabControl.Items.Add(tab);
        }

        private void MenuItem_OpenFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text|*.txt|All|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var path = openFileDialog.FileName;

                Read(path);
            }
        }

        private void Read(string path)
        {
            StreamReader rd = new StreamReader(path);

            string pathname = System.IO.Path.GetFileName(path);
            if (pathname.Length > 30) pathname = pathname.Substring(0, 30) + "...";

            TabItem tab = new TabItem();
            tab.Header = string.Format(pathname);

            System.Windows.Controls.TextBox c = new System.Windows.Controls.TextBox();
            tab.Content = c;

            c.Text = File.ReadAllText(path);
            tab.IsSelected = true;

            TabControl.Items.Add(tab);
        }

        private void MenuItem_OpenFolder(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dlg.SelectedPath;
                    string[] files = Directory.GetFiles(dlg.SelectedPath);

                    FilesList.Clear();

                    foreach (var item in files)
                    {
                        if (System.IO.Path.GetExtension(item) == ".txt")
                        {
                            var file = new TextFile();
                            file.Path = item;

                            string n = System.IO.Path.GetFileName(item);
                            if (n.Length > 15) file.Name = n.Substring(0, 15) + "...";
                            else file.Name = n;

                            FilesList.Add(file);
                        }
                    }
                }
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextFile t = (TextFile)((System.Windows.Controls.ListViewItem)sender).Content;
            string path = t.Path;
            Read(path);
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_About(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("This is simple text editor.", "About");
        }
    }
}
