using Autodesk.Revit.DB;
using BIMiconToolbar.Helpers.UserControls.SelectFileReferences.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BIMiconToolbar.Helpers.UserControls.SelectFileReferences.ViewModel
{
    public class SelectFileReferencesViewModel
    {
        public ObservableCollection<SelectFileReferencesModel> FileReferences { get; set; }

        public void LoadFileReferences(ExternalFileReferenceType[] externalFileReferenceTypes)
        {
            ObservableCollection<SelectFileReferencesModel> fileReferencesModels = new ObservableCollection<SelectFileReferencesModel>();

            foreach (var extRefType in externalFileReferenceTypes)
            {
                var selTypeModel = new SelectFileReferencesModel(extRefType);
                fileReferencesModels.Add(selTypeModel);
            }

            FileReferences = fileReferencesModels;
        }
    }
}
