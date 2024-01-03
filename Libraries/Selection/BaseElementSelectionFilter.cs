using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Libraries
{
    public abstract class BaseElementSelectionFilter : ISelectionFilter
    {
        protected readonly Func<Element, bool> ValidateElement;

        protected BaseElementSelectionFilter(Func<Element,bool> ValidateElement)
        {
            this.ValidateElement = ValidateElement;
        }
        public abstract bool AllowElement(Element elem);
        public abstract bool AllowReference(Reference reference, XYZ position);

    }
}
