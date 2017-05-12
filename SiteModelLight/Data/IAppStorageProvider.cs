namespace SiteModelLight
{
    /// <summary>
    /// Provider storage for application
    /// </summary>
    public interface IAppStorageProvider
    {
        //Overload All method with folder is in default folder
        System.IO.Stream OpenLocal(string fileName, string folderName);
        System.IO.Stream OpenAssets(string fileName, string folderName);
        bool RewriteLocal(string assetFileName, string localFileName, string folderName);
        object ObjectFromLocal(string fileName, string folderName);
        bool SaveObjectToLocal(object obj, string fileName, string folderName);
    }
}
