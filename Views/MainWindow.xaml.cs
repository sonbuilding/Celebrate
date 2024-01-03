using APIUser.Models;
using APIUser.ViewModels;
using System;
using System.Collections.Generic;
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

namespace APIUser.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel _mainViewModel;
        public MainWindow(MainViewModel mainViewModel)
        {

            this.DataContext = mainViewModel;
            _mainViewModel = mainViewModel;
            InitializeComponent();
            
        }

        private void UpNumber(object sender, RoutedEventArgs e)
        {

            if(_mainViewModel.SelectedParameter.ParameterPosition == string.Empty)
            {
                _mainViewModel.SelectedParameter.ParameterPosition = "1";
            }
            else
            {
                int i = Convert.ToInt32(_mainViewModel.SelectedParameter.ParameterPosition);
                i++;
                _mainViewModel.SelectedParameter.ParameterPosition = i.ToString();
            }
            _mainViewModel.ParameterSettingCheckList();

        }
        private void DnNumber(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedParameter.ParameterPosition != string.Empty)
            {
                int i = Convert.ToInt32(_mainViewModel.SelectedParameter.ParameterPosition);
                if (i > 1) { i--; }
                _mainViewModel.SelectedParameter.ParameterPosition = i.ToString();
            }
            _mainViewModel.ParameterSettingCheckList();

        }

        private void GoToSearchClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ParameterFilter = _mainViewModel.SelectedParameter.Name;
            _mainViewModel.ClearSearchVisible = System.Windows.Visibility.Visible;
        }

    }
}
