using System;
using System.IO;
using System.Reflection;
using Autodesk.Windows;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;

namespace BIMicon.BIMiconToolbar.Tab
{
    class Auxiliar
    {
        public static RibbonToolTip ButtonToolTip(string resourceName,
                                                  string resourcessemblyPath,
                                                  string buttonContent,
                                                  string buttonExpandendContent)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), resourceName);

            using (Stream stream = Assembly
              .GetExecutingAssembly()
              .GetManifestResourceStream(resourcessemblyPath))
            {
                var buffer = new byte[stream.Length];

                stream.Read(buffer, 0, buffer.Length);

                using (FileStream fs = new FileStream(tempPath,
                                                      FileMode.Create,
                                                      FileAccess.Write))
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
            }

            RibbonToolTip toolTip = new RibbonToolTip()
            {
                Content = buttonContent,
                ExpandedContent = buttonExpandendContent,
                ExpandedVideo = new Uri(Path.Combine(Path.GetTempPath(), resourceName)),
                IsHelpEnabled = true,
                IsProgressive = true
            };

            return toolTip;
        }

        public static void SetRibbonItemToolTip(RibbonItem item, RibbonToolTip toolTip)
        {
            IUIRevitItemConverter itemConverter =
                new InternalMethodUIRevitItemConverter();

            var ribbonItem = itemConverter.GetRibbonItem(item);
            if (ribbonItem == null)
                return;
            ribbonItem.ToolTip = toolTip;
        }

        interface IUIRevitItemConverter
        {
            Autodesk.Windows.RibbonItem GetRibbonItem(
              RibbonItem item);
        }

        class InternalMethodUIRevitItemConverter : IUIRevitItemConverter
        {
            public Autodesk.Windows.RibbonItem GetRibbonItem(
              RibbonItem item)
            {
                Type itemType = item.GetType();

                var mi = itemType.GetMethod("getRibbonItem",
                  BindingFlags.NonPublic | BindingFlags.Instance);

                var windowRibbonItem = mi.Invoke(item, null);

                return windowRibbonItem
                  as Autodesk.Windows.RibbonItem;
            }
        }
    }
}
