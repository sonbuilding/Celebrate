using APIUser.Libraries.MVVM;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Models
{
    public class SharedParameterItem:BaseViewModel
    {
        #region extend property
        private bool _IsChecked = false;
        public bool IsChecked { get=>_IsChecked; set { _IsChecked = value;OnPropertyChanged(); } }
        public bool IsEnable { get; set; }
        private string _ParameterPosition = string.Empty;
        public string ParameterPosition { get => _ParameterPosition; set { _ParameterPosition = value; OnPropertyChanged(); } }
        #endregion
        public string Name { get; set; }
        public string Group { get; set; }

        public SharedParameterItem(SharedParameterElement sharedParameterElement)
        {
            IsChecked = false;
            IsEnable = true;
          
            Name = sharedParameterElement.Name;
            Group = LabelUtils.GetLabelFor(sharedParameterElement.GetDefinition().ParameterGroup);
        }

    }
}
