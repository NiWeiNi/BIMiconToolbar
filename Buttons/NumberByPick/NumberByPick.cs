using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.NumberByPick
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberByPick : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Call WPF for user input
            using (NumberByPickWPF customWindow = new NumberByPickWPF(commandData))
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow)
                {
                    Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle
                };

                customWindow.ShowDialog();

                IList<ElementId> elementIds = customWindow.ElementIds;
                string startNumber = customWindow.StartNumber;
                string prefix = customWindow.Prefix;

                if (prefix == null || startNumber == null)
                {
                    return Result.Failed;
                }

                if (customWindow.Cancel == false && elementIds != null && elementIds.Count > 0)
                {
                    Parameter selParameter = customWindow.SelectedComboItemParameters.Tag as Parameter;

                    // Create two list that contains all selected elements
                    List<Element> selElements = new List<Element>();

                    // Convert collector to list of elements if no level filter is required
                    foreach (ElementId eId in elementIds)
                    {
                        selElements.Add(doc.GetElement(eId));
                    }

                    // Renumber selected elements
                    using (Transaction t = new Transaction(doc, "Renumber elements"))
                    {
                        int number = int.Parse(startNumber);

                        List<Element> elementsToRenumber = new List<Element>();
                        Parameter param = customWindow.SelectedComboItemParameters.Tag as Parameter;

                        t.Start();

                        // Renumber all elements with unique name
                        Parameters.FillRandomStringParameters(selElements.ToArray(), param.Id);

                        foreach (Element el in selElements)
                        {
                            ParameterSet parameterSet = el.Parameters;
                            ParameterSetIterator paramIt = parameterSet.ForwardIterator();
                            paramIt.Reset();

                            while (paramIt.MoveNext())
                            {
                                Parameter parameter = paramIt.Current as Parameter;

                                if (parameter.Id == param.Id)
                                {
                                    string value = prefix + number.ToString();
                                    parameter.Set(value);

                                    number++;
                                    break;
                                }
                            }
                        }

                        t.Commit();
                    }
                    return Result.Succeeded;
                }
                return Result.Cancelled;
            }
        }
    }
}
