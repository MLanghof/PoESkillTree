using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace POESKillTree.TreeGenerator.Views
{
    /// <summary>
    /// Interaction logic for AdvancedGeneratorTab.xaml
    /// </summary>
    public partial class AdvancedGeneratorTab
    {
        public AdvancedGeneratorTab()
        {
            InitializeComponent();
        }

        // If a row with validation errors that is currently edited gets removed,
        // AttrConstraintGrid.Items.Refresh() needs to be called because the validation
        // is not automatically updated on remove (Removing doesn't like to have a button for it, it seems).
        private bool _clicked;

        private void DeleteRowButton_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_clicked)
            {
                AttrConstraintGrid.Items.Refresh();
                _clicked = false;
            }
        }

        private void DeleteRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            _clicked = true;
        }
<<<<<<< HEAD
        
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                // DataGrid.SelectionUnit must not be "FullRow"
                cell.IsSelected = true;
            }
        }
||||||| merged common ancestors
=======

        private void DataGridCell_MouseEnter(object sender, System.EventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                //if (!cell.IsFocused)
                //{
                //    cell.IsEditing = true;
                //    cell.Focus();
                //}
                cell.IsEditing = true;
            }
        }
        private void DataGridCell_MouseLeave(object sender, System.EventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                //if (!cell.IsFocused)
                //{
                //    cell.IsEditing = true;
                //    cell.Focus();
                //}
                cell.IsEditing = false;
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        } 
>>>>>>> origin/treeGen
    }
}