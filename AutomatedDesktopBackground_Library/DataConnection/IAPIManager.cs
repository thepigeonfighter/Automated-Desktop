using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IAPIManager
    {
        Task GetImagesBySearch(string query, bool userRequested);
        Task<IRootObject> GetResults(string interestName);
    }
}