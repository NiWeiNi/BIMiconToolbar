using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMiconToolbar.CreateMaterials
{
    [TransactionAttribute(TransactionMode.Manual)]
    class CreateMaterials : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            string texturesPath = @"C:\Users\BIMicon\Desktop\Materials";

            // Retrieve all allowed files to be used for texture as rendering
            var extensions = new List<string> { "jpg", "tif", "tiff", "png" };
            var textureFiles = Directory
                .EnumerateFiles(texturesPath, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

            // Retrieve default material in Revit
            Material defMat = new FilteredElementCollector(doc)
                .OfClass(typeof(Material))
                .Where(x => x.Name == "Default")
                .FirstOrDefault() as Material;

            ElementId appearanceId = defMat.AppearanceAssetId;
            AppearanceAssetElement appearanceElem = doc.GetElement(appearanceId) as AppearanceAssetElement;
            Asset asset = appearanceElem.GetRenderingAsset();


            foreach (var tF in textureFiles)
            {
                if (tF.Contains("_COL_"))
                {
                    string parentDirectory = Directory.GetParent(tF).FullName;

                    string grandParentDirectory = Directory.GetParent(parentDirectory).Name;

                    using (var transaction = new Transaction(doc))
                    {
                        transaction.Start("CreateMaterial");

                        // Create new material
                        try
                        {
                            ElementId materialId = Material.Create(doc, grandParentDirectory);
                            var material = doc.GetElement(materialId) as Material;

                            AppearanceAssetElement assetElement = AppearanceAssetElement
                                .Create(doc, grandParentDirectory, asset);

                            material.AppearanceAssetId = assetElement.Id;

                            AssignRenderingTexturePath(doc, material, tF);
                        }
                        catch
                        {
                            //TODO: Log error
                        }

                        // Add attach to class
                        transaction.Commit();
                    }
                }       
            }

            return Result.Succeeded;
        }

        public void AssignRenderingTexturePath(Document doc, Material mat, string texturePath)
        {
            using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
            {
                Asset editableAsset = editScope.Start(mat.AppearanceAssetId);

                AssetProperty assetProperty = editableAsset.FindByName("generic_diffuse");

                Asset connectedAsset = assetProperty.GetConnectedProperty(0) as Asset;
                if (connectedAsset.Name == "UnifiedBitmapSchema")
                {
                    AssetPropertyString path = connectedAsset.FindByName(UnifiedBitmap.UnifiedbitmapBitmap) as AssetPropertyString;
                    if (path.IsValidValue(texturePath))
                        path.Value = texturePath;
                }

                editScope.Commit(true);
            }
        }
    }
}
