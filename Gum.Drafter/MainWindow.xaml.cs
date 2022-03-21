using System;
using System.Windows;
using Gum.Core.Utility;
using Gum.Drafter.Model;

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
            RepositoryListBox.Items.Add(new Repository(RepositoryEntryTextBox.Text));
        }

        private void OnRepositoryItemSelected(object sender, RoutedEventArgs e)
        {
            Repository repository = GetSelectedRepository();
        }

        private Repository GetSelectedRepository()
        {
            return RepositoryListBox.SelectedItem as Repository;
        }

        private void OnAddNewEntryButtonClicked(object sender, RoutedEventArgs e)
        {
            Repository repository = GetSelectedRepository();
        }

        private void OnCodegenButtonClicked(object sender, RoutedEventArgs e)
        {
            Repository repository = RepositoryListBox.SelectedItem as Repository;
            CodeGenerator
                .GenerateFromRepositoryAsync(repository, "Gum",
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).UnhandledAsync();
        }

        private void OnExportButtonClicked(object sender, RoutedEventArgs e)
        {
            Repository repository = GetSelectedRepository();
            MessageBox.Show(repository?.ToJson());
        }
    }
}