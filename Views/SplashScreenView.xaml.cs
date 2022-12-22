// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Ankara_Online.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreenView : WinUIEx.SplashScreen
    {
        public SplashScreenView(Type window) : base(window)
        {
            this.InitializeComponent();
            this.CenterOnScreen();

            /*            
            var windowHandle = WindowNative.GetWindowHandle(this);
            var _windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var _appWindow = AppWindow.GetFromWindowId(_windowId);
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            */
        }
    }
}
