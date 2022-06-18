using System.Windows;

using XTrakr.Models;
using XTrakr.ViewModels;

namespace XTrakr.Views;
/// <summary>
/// Interaction logic for ExplorerWindow.xaml
/// </summary>
public partial class ExplorerWindow : Window
{
    public ExplorerWindow()
    {
        InitializeComponent();
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is ExplorerItem item)
        {
            ((ExplorerViewModel)DataContext).SelectedItem = item;
        }
    }
}
