namespace StopSellingMessageGenerator.Interfaces
{
    public interface IWorkFolderOwnerChecker
    {
        bool MeIsOwner();
        void MakeMeOwner();

        string GetOwnerData();
    }
}