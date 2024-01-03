using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Libraries
{
    public class ElementSelectionFilter : BaseElementSelectionFilter
    {
        private readonly Func<Reference, bool> ValidateReference;

        public ElementSelectionFilter(Func<Element, bool> ValidateElement) : base(ValidateElement) { }

        public ElementSelectionFilter(Func<Element, bool> ValidateElement, Func<Reference, bool> ValidateReference) : base(ValidateElement) => this.ValidateReference = ValidateReference;

        public override bool AllowElement(Element elem) => ValidateElement(elem);

        public override bool AllowReference(Reference reference, XYZ position) => ValidateReference?.Invoke(reference) ?? false;

    }
}
