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
            // Retrieve default material in Revit for duplication
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Material defMat = new FilteredElementCollector(doc)
                .OfClass(typeof(Material))
                .Where(x => x.Name == "Default")
                .FirstOrDefault() as Material;

            ElementId appearanceId = defMat.AppearanceAssetId;
            AppearanceAssetElement appearanceElem = doc.GetElement(appearanceId) as AppearanceAssetElement;
            Asset asset = appearanceElem.GetRenderingAsset();

            // Path to stored textures
            string texturesPath = @"C:\MATERIAL TEXTURES";

            // Retrieve subfolders that contains textures
            var folders = Directory.EnumerateDirectories(texturesPath, "*", SearchOption.TopDirectoryOnly);

            // Check if the textures are located in folders with several levels of nesting
            bool deepNested = true;

            if (deepNested)
            {
                foreach (var textureFolder in folders)
                {
                    // Retrieve subfolders that contains textures
                    var textureFolders = Directory.EnumerateDirectories(texturesPath, "*", SearchOption.TopDirectoryOnly);

                    GenerateMaterials(doc, textureFolders, asset);
                }
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Function to generate materials in Revit project
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="folders"></param>
        /// <param name="asset"></param>
        public void GenerateMaterials(Document doc, IEnumerable<string> folders, Asset asset)
        {
            // Retrieve all allowed files to be used for texture as rendering in subdirectories
            var extensions = new List<string> { "jpg", "tif", "tiff", "png" };
            foreach (var textureFolder in folders)
            {
                var textureFiles = Directory
                    .EnumerateFiles(textureFolder, "*.*", SearchOption.AllDirectories)
                    .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

                // Loop through all textures in the subfolder and select textures for material
                string diffusePath = null;
                string bumpPath = null;
                string textureName = new DirectoryInfo(textureFolder).Name;

                foreach (var tF in textureFiles)
                {
                    // Select bump map
                    if (tF.Contains("_DISP_"))
                    {
                        bumpPath = tF;
                    }
                    // Select diffuse map
                    if (tF.Contains("_COL_"))
                    {
                        //string parentDirectory = Directory.GetParent(tF).FullName;
                        //string grandParentDirectory = Directory.GetParent(textureFolder).Name;
                        diffusePath = tF;
                    }
                }

                // Create material
                if (diffusePath != null)
                {
                    using (var transaction = new Transaction(doc))
                    {
                        // Create new material
                        try
                        {
                            transaction.Start("CreateMaterial");

                            ElementId materialId = Material.Create(doc, textureName);
                            var material = doc.GetElement(materialId) as Material;

                            AppearanceAssetElement assetElement = AppearanceAssetElement
                                .Create(doc, textureName, asset);

                            material.AppearanceAssetId = assetElement.Id;

                            using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
                            {
                                Asset editableAsset = editScope.Start(material.AppearanceAssetId);

                                AssignRenderingTexturePath(editableAsset, diffusePath, bumpPath);

                                editScope.Commit(true);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            //TODO: Log error
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function to assign texture to material
        /// </summary>
        /// <param name="editableAsset"></param>
        /// <param name="texturePath"></param>
        /// <param name="bumpPath"></param>
        public void AssignRenderingTexturePath(Asset editableAsset, string texturePath, string bumpPath)
        {
            // Description
            AssetPropertyString descriptionProperty = editableAsset.FindByName("keyword") as AssetPropertyString;
            descriptionProperty.Value = "blue carpet";
            // Diffuse image
            AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset.FindByName("generic_diffuse") as AssetPropertyDoubleArray4d;
            genericDiffuseProperty.SetValueAsColor(new Color(0x00, 0x00, 0x00));
            Asset connectedAsset = genericDiffuseProperty.GetSingleConnectedAsset();
            AssetPropertyString bitmapProperty = connectedAsset.FindByName("unifiedbitmap_Bitmap") as AssetPropertyString;
            bitmapProperty.Value = texturePath;

            // Assign bump map
            AssetProperty bumpMapProperty = editableAsset.FindByName("generic_bump_map");
            // Find the connected asset
            Asset connectedAssetBump = bumpMapProperty.GetSingleConnectedAsset();
            if (connectedAssetBump == null)
            {
                // Add a new default connected asset
                bumpMapProperty.AddConnectedAsset("UnifiedBitmap");
                connectedAssetBump = bumpMapProperty.GetSingleConnectedAsset();
            }
            if (connectedAssetBump != null)
            {
                // Find the target asset property
                AssetPropertyString bumpmapBitmapProperty = connectedAssetBump.FindByName("unifiedbitmap_Bitmap") as AssetPropertyString;
                if (bumpPath != null)
                {
                    bumpmapBitmapProperty.Value = bumpPath;
                }
            }
        }
    }
}
