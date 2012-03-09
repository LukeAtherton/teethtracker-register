﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace teethtracker_register {
    /// <summary>
    /// Interaction logic for DarkStyle.xaml
    /// </summary>
    public partial class DarkStyle {
        #region sizing event handlers

        void OnSizeSouth(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.South);
            }
        }

        void OnSizeNorth(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.North);
            }
        }

        void OnSizeEast(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.East);
            }
        }

        void OnSizeWest(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.West);
            }
        }

        void OnSizeNorthWest(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.NorthWest);
            }
        }

        void OnSizeNorthEast(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.NorthEast);
            }
        }

        void OnSizeSouthEast(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.SouthEast);
            }
        }

        void OnSizeSouthWest(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Window wnd = ((FrameworkElement)sender).TemplatedParent as Window;
            if (wnd != null) {
                WindowInteropHelper helper = new WindowInteropHelper(wnd);
                DragSize(helper.Handle, SizingAction.SouthWest);
            }
        }


        #endregion

        #region P/Invoke and helper method

        const int WM_SYSCOMMAND = 0x112;
        const int SC_SIZE = 0xF000;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        void DragSize(IntPtr handle, SizingAction sizingAction) {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizingAction), IntPtr.Zero);
                SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion

        #region helper enum

        public enum SizingAction {
            North = 3,
            South = 6,
            East = 2,
            West = 1,
            NorthEast = 5,
            NorthWest = 4,
            SouthEast = 8,
            SouthWest = 7
        }

        #endregion

    }
}
