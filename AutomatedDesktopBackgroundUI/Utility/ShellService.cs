using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Utility
{
    public class ShellService : IShellService
    {
        public Action RestartRequest { get; set; }
        private IShellExtension _shell;
        private string _executingAssembly;

        public ShellService(IShellExtension shell)
        {
            _shell = shell;
            _executingAssembly = Assembly.GetExecutingAssembly().Location;
        }
        private bool IsElevated()
        {
            return _shell.IsElevated();
        }
        public void RemoveShortCut()
        {
            if (IsElevated())
            {
                if (_shell.ContextMenuEnabled())
                {
                    _shell.RemoveMenuOption(_executingAssembly);
                }
            }
            else
            {
                RestartRequest?.Invoke();
            }
        }
        public void CreateShortCut()
        {
            if (IsElevated())
            {
                if (!_shell.ContextMenuEnabled())
                {
                    _shell.CreateMenuOption(_executingAssembly);
                }
            }
            else
            {
                RestartRequest?.Invoke();
            }
        }

        public void ElevateApplication()
        {
            _shell.RunAsAdmin(_executingAssembly);
        }
        public bool IsContextMenuEnable()
        {
            return _shell.ContextMenuEnabled();
        }
    }
}
