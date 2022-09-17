using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System.Linq;

namespace BIMiconToolbar.EditMatProperties
{
    [TransactionAttribute(TransactionMode.Manual)]
    class EditMatProperties : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Retrieve default material in Revit for duplication
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var colMats = new FilteredElementCollector(doc).OfClass(typeof(Material)).OfType<Material>();

            Transaction transaction = new Transaction(doc);
            transaction.Start("CreateMaterial");

            foreach (Material material in colMats)
            {
                ElementId appearanceId = material.AppearanceAssetId;
                AppearanceAssetElement appearanceElem = doc.GetElement(appearanceId) as AppearanceAssetElement;

                if (material.Name.StartsWith("mat - "))
                {
                    material.AppearanceAssetId = appearanceElem.Id;

                    using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
                    {
                        Asset asset = appearanceElem.GetRenderingAsset();
                        Asset editableAsset = editScope.Start(material.AppearanceAssetId);

                        // Diffuse image
                        AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset.FindByName("generic_diffuse") as AssetPropertyDoubleArray4d;
                    
                        if (genericDiffuseProperty != null)
                        {
                            Asset connectedAsset = genericDiffuseProperty.GetSingleConnectedAsset();
                            if (connectedAsset != null)
                            {
                                AssetPropertyString bitmapProperty = connectedAsset.FindByName("unifiedbitmap_Bitmap") as AssetPropertyString;
                                string oldPath = bitmapProperty.Value;

                                if (oldPath != null && oldPath.Contains(@"C:\Materials"))
                                {
                                    string newPath = oldPath.Replace(@"C:\Old", @"R:\New");

                                    AssetProperty bumpMapProperty = editableAsset.FindByName("generic_bump_map");
                                    // Find the connected asset
                                    Asset connectedAssetBump = bumpMapProperty.GetSingleConnectedAsset();
                                    if (connectedAssetBump != null)
                                    {
                                        // Find the target asset property
                                        AssetPropertyString bumpmapBitmapProperty = connectedAssetBump.FindByName("unifiedbitmap_Bitmap") as AssetPropertyString;
                                        string oldBumpPath = bumpmapBitmapProperty.Value;
                                        string newBumpPath = oldBumpPath.Replace(@"C:\Old", @"R:\New");
                                        bumpmapBitmapProperty.Value = newBumpPath;
                                    }
                                }
                            }
                        }
                    
                    
                        editScope.Commit(true);
                    }
                }

            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}