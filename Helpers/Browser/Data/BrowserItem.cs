namespace BIMicon.BIMiconToolbar.Helpers.Browser.Data
{
    /// <summary>
    /// Information about a directory item; drives, folders, and files
    /// </summary>
    public class BrowserItem
    {
        /// <summary>
        /// The type of this item
        /// </summary>
        public BrowserItemType Type { get; set; }

        /// <summary>
        /// The absolute path to this item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Name of this directory item
        /// </summary>
        public string Name { get { return this.Type == BrowserItemType.Drive ? this.FullPath : BrowserStructure.GetFileFolderName(this.FullPath); } }
    }
}
