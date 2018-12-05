using System;

namespace AutomatedDesktopBackgroundUI.Utility
{
    public interface IShellService
    {
        Action RestartRequest { get; set; }
        void CreateShortCut();
        void RemoveShortCut();
        void ElevateApplication();
        bool IsContextMenuEnable();

    }
}