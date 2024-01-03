using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using APIUser.Views;
using APIUser.Models;
using APIUser.ViewModels;

namespace RevitAPIUser
{
    [Transaction(TransactionMode.Manual)]
    public class ElementManipulating : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            RevitExternalEventHandler handler = new RevitExternalEventHandler();
            ExternalEvent externalEvent = ExternalEvent.Create(handler);
            MainModel mainModel = new MainModel(uidoc);
            MainViewModel mainViewModel = new MainViewModel(mainModel, externalEvent, handler);
            MainWindow mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();
            return Result.Succeeded;
        }
    }

    //External Event Handler
    public class RevitExternalEventHandler : IExternalEventHandler
    {
        public Action<UIApplication> Action { get; set; }
        public void Execute(UIApplication app)
        {
            Action?.Invoke(app);
        }

        public string GetName()
        {
            return "TransactionNavigation";
        }
    }
}
