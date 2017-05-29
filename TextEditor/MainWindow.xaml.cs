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
        public string Content { get; set; }
    }

    public partial class MainWindow : Window
    {
        public int counter = 0;
        public ObservableCollection<TextFile> FilesList { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            FilesList = new ObservableCollection<TextFile>();

            TreeBox.IsChecked = true;
            PluginView.Visibility = Visibility.Collapsed;

            this.DataContext = FilesList;
        }

        private void MenuItem_New(object sender, RoutedEventArgs e)
        {
            counter++;
            TabItem tab = new TabItem();
            tab.Header = string.Format("New file {0}", counter);
            tab.HeaderTemplate = MyTabControl.FindResource("TabHeader") as DataTemplate;

            System.Windows.Controls.RichTextBox c = new System.Windows.Controls.RichTextBox();
            c.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tab.Content = c;
            tab.IsSelected = true;

            TextFile text = new TextFile();
            text.Name = string.Format("New file {0}", counter);
            text.Content = "";

            MyTabControl.Items.Add(tab);
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            TabItem tab = new TabItem();
            tab = (TabItem)MyTabControl.SelectedItem;
            System.Windows.Controls.RichTextBox box = new System.Windows.Controls.RichTextBox();
            box = (System.Windows.Controls.RichTextBox)tab.Content;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName,
                                  new TextRange(box.Document.ContentStart,
                                  box.Document.ContentEnd).Text);
            }
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
            tab.HeaderTemplate = MyTabControl.FindResource("TabHeader") as DataTemplate;

            System.Windows.Controls.RichTextBox c = new System.Windows.Controls.RichTextBox();
            tab.Content = c;

            c.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText(path))));
            c.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tab.IsSelected = true;

            TextFile text = new TextFile();
            text.Path = path;
            text.Name = System.IO.Path.GetFileName(path);

            MyTabControl.Items.Add(tab);
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)(sender as System.Windows.Controls.Button).CommandParameter;

            MyTabControl.Items.Remove(tab);
        }

        private void TreeBox_Checked(object sender, RoutedEventArgs e)
        {
            TreeView.Visibility = Visibility.Visible;
        }

        private void TreeBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TreeView.Visibility = Visibility.Collapsed;
        }

        private void PluginBox_Checked(object sender, RoutedEventArgs e)
        {
            PluginView.Visibility = Visibility.Visible;
        }

        private void PluginBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PluginView.Visibility = Visibility.Collapsed;
        }
    }
}
