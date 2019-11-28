using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace TwainDotNet.Wpf
{
    /// <summary>
    /// A windows message hook for WPF applications.
    /// </summary>
    /// 

    public class WpfWindowMessageHookX : IWindowsMessageHook
    {
        HwndSource _source;
        System.IntPtr _interopHelper;
        WindowInteropHelper _window;
        bool FlagWindow;

        bool _usingFilter;

        //public System.IntPtr _handle = IntPtr.Zero;

        //public Window TheMainWindow { get { return App.Current.MainWindow; } }
        public WpfWindowMessageHookX(Window window)
        {
            _source = (HwndSource)PresentationSource.FromDependencyObject(window);
            _window = new WindowInteropHelper(window);
            FlagWindow = true;
        }

        public WpfWindowMessageHookX(System.Windows.Controls.UserControl window)
        {

            _source = (HwndSource)PresentationSource.FromDependencyObject(window);
            HwndSource source = (HwndSource)HwndSource.FromVisual(window);


            _interopHelper = source.Handle;


        }


        public IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (FilterMessageCallback != null)
            {
                return FilterMessageCallback(hwnd, msg, wParam, lParam, ref handled);
            }

            return IntPtr.Zero;
        }

        public bool UseFilter
        {
            get
            {
                return _usingFilter;
            }
            set
            {
                if (!_usingFilter && value == true)
                {
                    _source.AddHook(FilterMessage);
                    _usingFilter = true;
                }

                if (_usingFilter && value == false)
                {
                    _source.RemoveHook(FilterMessage);
                    _usingFilter = false;
                }
            }
        }

        public FilterMessage FilterMessageCallback { get; set; }

        public IntPtr WindowHandle
        {
            get
            {
                if (FlagWindow == false)
                {
                    return _interopHelper;
                }
                else
                {
                    return _window.Handle;

                }


            }
        }

    }
}