using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Libraries
{
    public class RevitLinkElementSelectionFilter : BaseElementSelectionFilter
    {
        private Document doc;
        public RevitLinkElementSelectionFilter(Document doc, Func<Element, bool> ValidateElement) : base(ValidateElement) => this.doc = doc;

        public override bool AllowElement(Element elem) => true;

        public override bool AllowReference(Reference reference, XYZ position)
        {
            if (!(doc.GetElement(reference.ElementId) is RevitLinkInstance linkInstance)) return ValidateElement(doc.GetElement(reference));
            return ValidateElement(linkInstance.GetLinkDocument().GetElement(reference.LinkedElementId));
        }
    }
}
