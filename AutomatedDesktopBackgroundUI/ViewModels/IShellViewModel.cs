using AutomatedDesktopBackgroundUI.Config;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public interface IShellViewModel
    {
        void Handle(CommandNames message);
        void LoadMain();
        void LoadSettings();
        void UpdateConnectionStatus();
    }
}