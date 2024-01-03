using APIUser.Libraries.MVVM;
using APIUser.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPIUser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace APIUser.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private MainModel _mainModel;
        public MainModel mainModel { get => _mainModel; set { _mainModel = value; OnPropertyChanged(); } }
        public ObservableCollection<SharedParameterItem> SharedParameters { get; set; }
        private bool HasError { get; set; } = false;
        private List<Element> elements;
        #region Binding Property
        public bool AllProjectOption { get; set; }
        private bool _ActiveViewOption = true;
        public bool ActiveViewOption { get => _ActiveViewOption; set { _ActiveViewOption = value; OnPropertyChanged(); } }
        public bool UserSelectionOption { get; set; }
        public bool HideUncheckedCheckBox { get; set; }
        private bool _UnderScore;
        public bool UnderScore { get => _UnderScore; set { _UnderScore = value; OnPropertyChanged(); } }
        private bool _Hyphren;
        public bool Hyphren { get => _Hyphren; set { _Hyphren = value; OnPropertyChanged(); } }
        private bool _Space;
        public bool Space { get => _Space; set { _Space = value; OnPropertyChanged(); } }
        private string _ErrorMessegeParameterPosition = string.Empty;
        public string ErrorMessegeParameterPosition { get => _ErrorMessegeParameterPosition; set { _ErrorMessegeParameterPosition = value; OnPropertyChanged(); } }
        private string _ParameterFilter = string.Empty;
        public string ParameterFilter { get => _ParameterFilter; set { _ParameterFilter = value; OnPropertyChanged(); ParametersCollectionView.Refresh(); } }
        private ObservableCollection<SharedParameterItem> _SelectedParameters;
        public ObservableCollection<SharedParameterItem> SelectedParameters { get => _SelectedParameters; set { _SelectedParameters = value; OnPropertyChanged(); } }
        private ICollectionView _ParametersCollectionView;
        public ICollectionView ParametersCollectionView { get => _ParametersCollectionView; set { _ParametersCollectionView = value; OnPropertyChanged(); } }
        private SharedParameterItem _SelectedParameter;
        public SharedParameterItem SelectedParameter { get => _SelectedParameter; set { _SelectedParameter = value; OnPropertyChanged(); } }

        private Brush _NotifyTextBlockColor;
        public Brush NotifyTextBlockColor { get => _NotifyTextBlockColor; set { _NotifyTextBlockColor = value;OnPropertyChanged(); } }
        //Make sure not null
        public int _SelectedIndex;
        public int SelectedIndex { get => _SelectedIndex; set { _SelectedIndex = value; OnPropertyChanged(); } }

        private System.Windows.Visibility _ClearSearchVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ClearSearchVisible { get => _ClearSearchVisible; set { _ClearSearchVisible = value;OnPropertyChanged(); } }
        #endregion
        #region Command
        public ICommand ApplyCommand { get; set; }
        public ICommand HideUncheckedCheckBoxCommand { get; set; }
        public ICommand UserSelectionCommand { get; set; }
        public ICommand OKCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand UpCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand UnderScoreCommand { get; set; }
        public ICommand HyphrenCommand { get; set; }
        public ICommand SpaceCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }


        #endregion
        public MainViewModel(MainModel mainModel, ExternalEvent externalEvent, RevitExternalEventHandler externalEventHandler)
        {

            #region Initialize Data
            this.mainModel = mainModel;
            SharedParameters = mainModel.GetSharedParameters();
            ParametersCollectionView = CollectionViewSource.GetDefaultView(SharedParameters);
            ParametersCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SharedParameterItem.Group)));
            ParametersCollectionView.Filter = FilterParameter;
            
            List<char> separator = new List<char>();
            #endregion
            #region Command
            #region Separator
            UnderScoreCommand = new RelayCommand(p => true, p => { if (UnderScore) { separator.Add('_'); } });
            HyphrenCommand = new RelayCommand(p => true, p => { if (Hyphren) { separator.Add('-'); } });
            SpaceCommand = new RelayCommand(p => true, p => { if (Space) { separator.Add(' '); } });

            #endregion
            ClearSearchCommand = new RelayCommand(p => true, p => { ParameterFilter = string.Empty; ClearSearchVisible = System.Windows.Visibility.Collapsed; });
            UserSelectionCommand = new RelayCommand(p => true, p =>
            {
                elements = mainModel.UserSelection();
                if (elements.Count == 0)
                {
                    HasError = true;
                    CanRun();
                }
                if (p is Window window)
                {
                    window.Activate();
                }


            });
            ApplyCommand = new RelayCommand(p => true, p =>
            {

                SelectedParameters = new ObservableCollection<SharedParameterItem>(SharedParameters.Where(para => para.IsChecked).OrderBy(para =>
                {
                    if (para.ParameterPosition == string.Empty)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt32(para.ParameterPosition);
                    }


                }).ToList());
                //SelectedParameters = new ObservableCollection<SharedParameterItem>(SelectedParameters.);
                SelectedIndex = 0;
                SharedParameters.Intersect(SelectedParameters).Select(para => para.IsEnable = false).ToList();
                SharedParameters.Except(SelectedParameters).Select(para => para.ParameterPosition = string.Empty).ToArray();
                ParametersCollectionView.Refresh();
                ParameterSettingCheckList();
            });
            ModifyCommand = new RelayCommand(p => true, p =>
            {
                SharedParameters.Select(para => para.IsEnable = true).ToArray();
                ParametersCollectionView.Refresh();
            });
            HideUncheckedCheckBoxCommand = new RelayCommand(p => true, p =>
             {
                 if (HideUncheckedCheckBox)
                 {
                     ParametersCollectionView.Filter += UnCheckFilter;
                 }
                 else
                 {
                     ParametersCollectionView.Filter -= UnCheckFilter;
                 }
                 
             });
            SortCommand = new RelayCommand(p => true, p =>
            {
                SelectedParameters = new ObservableCollection<SharedParameterItem>(SharedParameters.Where(para => para.IsChecked).OrderBy(para =>
                {
                    if (para.ParameterPosition == string.Empty)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt32(para.ParameterPosition);
                    }


                }).ToList());
            });
            OKCommand = new RelayCommand(p => CanRun(), p =>
            {
                if (!UserSelectionOption)
                {
                    if (AllProjectOption)
                    {
                        elements = mainModel.GetElements(false);
                    }
                    else if (ActiveViewOption)
                    {
                        elements = mainModel.GetElements(true);
                    }
                }
                if (externalEvent != null)
                {
                    externalEvent.Raise();
                    externalEventHandler.Action = (uiapp =>
                    {
                        using (TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document))
                        {
                            transactionGroup.Start("Extract Element Name");
                            Transaction transaction = new Transaction(uiapp.ActiveUIDocument.Document, "SetParameter");
                            transaction.Start();
                            mainModel.ElementsExtractNameToParameters(elements, SelectedParameters.ToList(), separator.Distinct(null).ToArray());
                            transaction.Commit();
                            transactionGroup.Assimilate();
                        }
                    });
                }
            });
            #endregion
        }
        #region Method
        private bool FilterParameter(Object obj)
        {
            if (obj is SharedParameterItem sharedParameterItem)
            {
                if (ParameterFilter != string.Empty)
                {
                    ClearSearchVisible = System.Windows.Visibility.Visible;
                }
                else
                {
                    ClearSearchVisible = System.Windows.Visibility.Collapsed;
                }
                return sharedParameterItem.Name.Contains(ParameterFilter);
            }
            return false;
        }
        private bool UnCheckFilter(Object obj)
        {
            if (obj is SharedParameterItem sharedParameterItem)
            {
                return sharedParameterItem.IsChecked;
            }
            return false;
        }
        private bool CanRun()
        {
            if (SelectedParameters == null || HasError || SelectedParameters.Count == 0)
            {
                return false;
            }

            return UserSelectionOption || ActiveViewOption || AllProjectOption;

        }
        public void ParameterSettingCheckList()
        {
            if (SelectedParameters == null || SelectedParameters.Count == 0)
            {
                HasError = false;
                ErrorMessegeParameterPosition = string.Empty;
                return;
            }
            if (SelectedParameters.Where(para => para.ParameterPosition == string.Empty).Any())
            {
                HasError = true;
                NotifyTextBlockColor = new SolidColorBrush(Colors.Red);
                ErrorMessegeParameterPosition = "Specific Parameter Position !";
            }
            else
            {
                if (SelectedParameters.Select(para => para.ParameterPosition).Distinct(null).Count() != SelectedParameters.Count)
                {
                    HasError = true;
                    NotifyTextBlockColor = new SolidColorBrush(Colors.Red);
                    ErrorMessegeParameterPosition = "Duplicate Parameter Position !";
                }
                else
                {
                    HasError = false;
                    NotifyTextBlockColor = new SolidColorBrush(Colors.Green);
                    ErrorMessegeParameterPosition = "This One Perfect";
                }
            }
        }
        #endregion
    }
}
