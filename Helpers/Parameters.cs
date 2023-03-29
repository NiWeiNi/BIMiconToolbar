using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class Parameters
    {
        /// <summary>
        /// Method to overwrite parameters of value type string
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="parameterId"></param>
        public static void FillRandomStringParameters(Element[] elements, ElementId parameterId)
        {
            foreach (Element el in elements)
            {
                ParameterSet parameterSet = el.Parameters;
                ParameterSetIterator paramIt = parameterSet.ForwardIterator();
                paramIt.Reset();

                while (paramIt.MoveNext())
                {
                    Parameter param = paramIt.Current as Parameter;

                    if (param.Id == parameterId)
                    {
                        string value = "BIMicon" + Guid.NewGuid();
                        param.Set(value);
                    }
                }
            }
        }

        /// <summary>
        /// Method to retrieve filtered project parameters ID by category
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static string[] ProjectParameters(Document doc, string categoryName)
        {
            List<string> parametersID = new List<string>();

            BindingMap map = doc.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();

            while (it.MoveNext())
            {
                ElementBinding eleBinding = it.Current as ElementBinding;
                InstanceBinding insBinding = eleBinding as InstanceBinding;

                if (insBinding != null && IsInstBindingOfCategory(insBinding, categoryName))
                {
                    Definition def = it.Key;
                    if (def != null)
                    {
                        ExternalDefinition extDef = def as ExternalDefinition;

                        if (extDef != null)
                        {
                            string GUID = extDef.GUID.ToString();
                            parametersID.Add(GUID);
                        }
                        else
                        {
                            InternalDefinition intDef = def as InternalDefinition;
                            string ID = intDef.Id.ToString();
                            parametersID.Add(ID);
                        }
                    }
                }
            }
            return parametersID.ToArray();
        }

        /// <summary>
        /// Method to retrieve parameters from element according to storage type
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static Parameter[] GetParametersOfCategoryByStorageType(Document doc, BuiltInCategory category)
        {
            List<Parameter> parameters = new List<Parameter>();

            Element element = new FilteredElementCollector(doc).OfCategory(category).WhereElementIsNotElementType().FirstElement();

            if (element != null)
            {
                ParameterSet parameterSet = element.Parameters;
                ParameterSetIterator paramIt = parameterSet.ForwardIterator();
                paramIt.Reset();

                while (paramIt.MoveNext())
                {
                    Parameter param = paramIt.Current as Parameter;

                    if (param.StorageType == StorageType.String && param.IsReadOnly == false)
                    {
                        parameters.Add(param);
                    }
                }

                return parameters.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Method to retrieve instance parameters of an instance
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Parameter[] GetParametersOfInstance(Element element)
        {
            List<Parameter> parameters = new List<Parameter>();

            ParameterSet parameterSet = element.Parameters;
            ParameterSetIterator paramIt = parameterSet.ForwardIterator();
            paramIt.Reset();

            while (paramIt.MoveNext())
            {
                Parameter param = paramIt.Current as Parameter;

                if (param.StorageType == StorageType.String && param.IsReadOnly == false)
                {
                    parameters.Add(param);
                }
            }
            return parameters.ToArray();
        }


        /// <summary>
        /// Method to check if Instance Binding is of a specific category
        /// </summary>
        /// <param name="insBinding"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool IsInstBindingOfCategory(InstanceBinding insBinding, string categoryName)
        {
            CategorySet catSet = insBinding.Categories;
            CategorySetIterator catSetIt = catSet.ForwardIterator();
            catSetIt.Reset();

            string cat = "";

            while (catSetIt.MoveNext())
            {
                Category category1 = catSetIt.Current as Category;
                cat = category1.Name;
            }

            if (cat != "" && cat == categoryName)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to check if elements of category are level based
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static bool IsElementLevelBased(Document doc, ElementId catId)
        {
            // Retrieve instances in category
            FilteredElementCollector catInstances = new FilteredElementCollector(doc)
                                .OfCategoryId(catId)
                                .WhereElementIsNotElementType();

            // Check if selected category has instances that are level based
            foreach (Element el in catInstances)
            {
                if (el.LevelId.IntegerValue != -1)
                    return true;
            }

            return false;
        }
    }
}
