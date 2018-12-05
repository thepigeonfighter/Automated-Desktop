namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public interface IShellExtension
    {
        bool ContextMenuEnabled();
        void CreateMenuOption(string executingAssembly);
        void RemoveMenuOption(string executingAssembly);
        bool IsElevated();
        void RunAsAdmin(string executingAssembly);
    }
}