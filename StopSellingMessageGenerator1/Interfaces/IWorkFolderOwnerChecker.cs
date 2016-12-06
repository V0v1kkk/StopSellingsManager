namespace StopSellingMessageGenerator.Interfaces
{
    public interface IWorkFolderOwnerChecker
    {
        /// <summary>
        /// Check work folder on exist
        /// </summary>
        /// <returns>Return true if work folder is exist, and false else</returns>
        bool WorkFolderExist();

        /// <summary>
        /// Return true if work folder occupied current user
        /// </summary>
        /// <returns>Return true if work folder occupied current user</returns>
        bool MeIsOwner();

        /// <summary>
        /// Make current user owner of work folder and may write some additional data as last data access time and so on.
        /// </summary>
        void MakeMeOwner();

        /// <summary>
        /// Return iformation about user which occupy work folder, and may return additional data.
        /// </summary>
        /// <returns></returns>
        string GetOwnerData();

        /// <summary>
        /// Delete information about ownership in work folder
        /// </summary>
        void ClearOwnership();
    }
}