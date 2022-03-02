using System;
using System.Collections.Generic;
using System.Globalization;
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
using Gum.Drafter.Model;
using Newtonsoft.Json.Linq;

namespace Gum.Drafter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnAddRepositoryButtonClicked(object sender, RoutedEventArgs e)
        {
            RepositoryListBox.Items.Add(new Repository(RepositoryEntryTextBox.Text, new JObject()));
        }

        private void OnRepositoryItemSelected(object sender, RoutedEventArgs e)
        {
            Repository selectedItem = RepositoryListBox.SelectedItem as Repository;
            TreeView.ItemsSource = null;
            TreeView.Items.Clear();
            TreeView.ItemsSource = (selectedItem).Source.Children();
        }
    }
}