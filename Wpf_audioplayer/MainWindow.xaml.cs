using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Wpf_audioplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string startRoot = "G:/TEST";
        private string activeRoot = "G:/TEST";

        public void SetRootLibrary(string str)
        {
            activeRoot = str;
            startRoot = activeRoot;
        }
        public string GetRootLibrary()
        {
            return activeRoot;
        }

        public MainWindow()
        {
            SetRootLibrary(startRoot);
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetRootLibrary(startRoot);
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            PopulateTreeView();
        }

        #region Populate Treeview
        private void PopulateTreeView()
        {
            tree_MediaLibrary.Items.Clear();
            foreach (var folder in Directory.GetDirectories(GetRootLibrary()))
            {
                //Create new item
                var item = new TreeViewItem()
                {
                    //Set header...
                    Header = GetFileOrFolderName(folder),
                    //...and full path
                    Tag = folder
                };                

                //Add null data for expanded
                item.Items.Add(null);

                //Listen to item being expanded
                item.Expanded += FolderExpanded;

                //Add item to main treeView
                tree_MediaLibrary.Items.Add(item);
            }
            SortItemsByAscending(tree_MediaLibrary);
        }
        #endregion

        #region Folder Expanded
        private void FolderExpanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks
            var item = (TreeViewItem)sender;

            // If the item contains only null data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;
            //Clear null data
            item.Items.Clear();

            //Get full path
            var fullPath = (string)item.Tag;
            #endregion

            #region Get Folders

            //Create blank list for directories
            var directiores = new List<string>();
            
            //Try and get directiores from folder
            //ignoring any issues doing so
            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                if (dirs.Length > 0)
                    directiores.AddRange(dirs);
            }
            catch { }

            //For each directory...
            directiores.ForEach(directoryPath =>
            {
                //Create directory iyem
                var subItem = new TreeViewItem()
                {
                    //Set header as foldder name
                    Header = GetFileOrFolderName(directoryPath),
                    //Set tag as full path
                    Tag = directoryPath
                };
                //Null item for expansion
                subItem.Items.Add(null);
                //Recursive function
                subItem.Expanded += FolderExpanded;

                //Add this item to parent
                item.Items.Add(subItem);
            });

            #endregion

            #region Get Files

            //Create blank list for files
            var files = new List<string>();

            //Try and get files from folder
            //ignoring any issues doing so
            try
            {
                var fs = Directory.GetFiles(fullPath);
                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            //For each file...
            files.ForEach(filePath =>
            {
                //Create file item
                var subItem = new TreeViewItem()
                {
                    //Set header as item name
                    Header = GetFileOrFolderName(filePath),
                    //Set tag as full path
                    Tag = filePath
                };

                //Add this item to parent (only mp3)
                if (new FileInfo(subItem.Tag.ToString()).Extension == ".mp3")
                    item.Items.Add(subItem);
            });

            #endregion  
            SortItemsByAscending(item);
        }
        #endregion

        #region Helpers
        public static string GetFileOrFolderName(string path)
        {
            //if we have no path - return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            // Make all slashes back slashes
            var normalizedPath = path.Replace('/', '\\');
            //find last index of backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');
            // check for no backslash, return path itself
            if (lastIndex <= 0)
                return path;
            //Return name adter last backslash
            return path.Substring(lastIndex + 1);
        }

        public void SortItemsByAscending(TreeViewItem tvItem)
        {
            tvItem.Items.SortDescriptions.Clear();
            tvItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
        }
        public void SortItemsByAscending(TreeView tv)
        {
            tv.Items.SortDescriptions.Clear();
            tv.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetRootLibrary(dialog.SelectedPath);
                    PopulateTreeView();
                }
            }
        }
    }
}
