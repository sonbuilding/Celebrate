
using APIUser.Libraries;
using APIUser.Libraries.MVVM;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Models
{
    public class MainModel:BaseViewModel
    {
        public readonly Document doc;
        public readonly UIDocument uidoc;
        private int _progressValue;
        public int ProgressValue { get => _progressValue; set { _progressValue = value;OnPropertyChanged(); } }
        public MainModel(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            doc = uidoc.Document;
        }
        public ObservableCollection<SharedParameterItem> GetSharedParameters()
        {
            IEnumerable<SharedParameterItem> parameters = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>().Select(p => new SharedParameterItem(p));
            return new ObservableCollection<SharedParameterItem>(parameters.ToList());
        }
        public List<Element> GetElements(bool IsActiveView)
        {
            List<BuiltInCategory> passCategory = new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_Sheets,
                BuiltInCategory.OST_DetailComponents,
                BuiltInCategory.OST_ProjectInformation,
                BuiltInCategory.OST_Materials,
                BuiltInCategory.OST_RvtLinks,
                BuiltInCategory.OST_ShaftOpening,
                BuiltInCategory.OST_SiteProperty
            };
            ElementMulticategoryFilter cateFilter = new ElementMulticategoryFilter(passCategory, true);
            if (IsActiveView)
            {
                List<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(cateFilter).WhereElementIsNotElementType().ToElements()
                        .Where(e =>
                        e.Category?.CategoryType == CategoryType.Model &&
                        e.Category?.AllowsBoundParameters == true &&
                        e.GetTypeId() != ElementId.InvalidElementId).ToList();

                return elements;
            }
            else
            {
                List<Element> elements = new FilteredElementCollector(doc).WherePasses(cateFilter).WhereElementIsNotElementType()
                         .Where(e =>
                         e.Category?.CategoryType == CategoryType.Model &&
                         e.Category?.AllowsBoundParameters == true &&
                         e.GetTypeId() != ElementId.InvalidElementId).ToList();
                return elements;
            }
        }
        public List<Element> UserSelection()
        {
            List<Element> elements = uidoc.Selection.PickObjects(ObjectType.Element, new ElementSelectionFilter(e => e.Category?.CategoryType == CategoryType.Model &&
                         e.Category?.AllowsBoundParameters == true &&
                         e.GetTypeId() != ElementId.InvalidElementId)).Select(r => doc.GetElement(r)).ToList();
            return elements;
        }
        public void ElementsExtractNameToParameters(List<Element> elements, List<SharedParameterItem> parameterItems, char[] Separator)
        {
            ProgressReport progressReport = new ProgressReport(elements.Count, value => 
            {
                ProgressValue = value;

            });
            foreach (var item in elements.GroupBy(e => GetElementFamilyNameOrTypeName(e)))
            {
                
                //Does all parameters exist?
                if (parameterItems.Select(pItem => pItem.Name).Select(p => item.First().LookupParameter(p) != null).All(b => b == true))
                {
                    //item.Select(e => parameterItems.Zip(parameterItems.Select(p => item.Key.Split(Separator)[Convert.ToInt32(p.ParameterPosition) - 1]), (pItem, v) => e.LookupParameter(pItem.Name).Set(v)).ToArray()).ToArray();
                    //Get and Set Parameter
                    string[] values = item.Key.Split(Separator);
                    if (values.Length >= parameterItems.Select(para=>Convert.ToInt32(para.ParameterPosition)).Max())
                    {
                       
                        foreach (Element e in item)
                        {
                            foreach (var paraItem in parameterItems)
                            {
                                e.LookupParameter(paraItem.Name).Set(values[Convert.ToInt32(paraItem.ParameterPosition) - 1]);
                            }
                        }
                    }

                }
               
            }
        }

        #region internal methods
        private string GetElementFamilyNameOrTypeName(Element e)
        {
            ElementType eType = e.Document.GetElement(e.GetTypeId()) as ElementType;
            if (eType is FamilySymbol familySymbol)
            {
                return familySymbol.FamilyName;
            }
            return eType.Name;

        }
        #endregion
        #region For Test
        public void ShowElement(List<ElementId> ids)
        {
            uidoc.Selection.SetElementIds(ids);
        }
        public void ShowDialog(string messege)
        {
            TaskDialog.Show("Hello", messege);
        }
        #endregion
    }
}
