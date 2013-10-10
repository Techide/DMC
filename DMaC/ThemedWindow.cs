using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using DMaC.UImgmt;

namespace DMaC
{
    public class ThemedWindow : Window
    {

        private readonly WindowChrome _chrome = new WindowChrome();

        private Point _restoreTop;

        private Point _cursorOffset;

        private readonly Color _maxedColor = Color.FromRgb(
            (byte)int.Parse("FC", NumberStyles.HexNumber),
            (byte)int.Parse("AC", NumberStyles.HexNumber),
            (byte)int.Parse("0C", NumberStyles.HexNumber));

        private Brush _normalBorderStyle;
        private Brush _highlightBorderBrush;
        
        private readonly Color _normalColor = Colors.Chartreuse;

        private Border _borderLeft;
        private FrameworkElement _borderTopLeft;
        private FrameworkElement _borderTop;
        private FrameworkElement _borderTopRight;
        private FrameworkElement _borderRight;
        private FrameworkElement _borderBottomRight;
        private FrameworkElement _borderBottom;
        private FrameworkElement _borderBottomLeft;
        private FrameworkElement _caption;
        private TextBlock _captionTextBlock;
        private FrameworkElement _frame;

        private Button _minimizeButton;
        private Button _maximizeButton;
        private Button _closeButton;

        private IntPtr _handle;

        public ThemedWindow()
        {
            SourceInitialized += (sender, e) =>
                {
                    _handle = new WindowInteropHelper(this).Handle;
                    var hwndSource = HwndSource.FromHwnd(_handle);
                    if (hwndSource != null)
                        hwndSource.AddHook(WndProc);
                };

            WindowChrome.SetWindowChrome(this, _chrome);
            _chrome.CaptionHeight = 0;
            _chrome.GlassFrameThickness = new Thickness(0, 1, 0, 0);
            _chrome.UseAeroCaptionButtons = false;
            _chrome.ResizeBorderThickness = new Thickness(4);

            Style = (Style)TryFindResource("ThemedWindowStyle");

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            RenderOptions.SetClearTypeHint(this, ClearTypeHint.Enabled);
            RenderOptions.SetCachingHint(this, CachingHint.Cache);

            StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, EventArgs eventArgs)
        {
            _frame.Margin = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
            _borderLeft.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderTopLeft.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderTop.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderTopRight.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderRight.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderBottomRight.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderBottom.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            _borderBottomLeft.Visibility = WindowState == WindowState.Maximized ? Visibility.Hidden : Visibility.Visible;
            ToggleMaximizeButtonColor();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RegisterFrame();
            RegisterBorders();
            RegisterCaption();
            RegisterMinimizeButton();
            RegisterMaximizeButton();
            RegisterCloseButton();
            RegisterCaptionText();
        }

        private void RegisterBorderEvents(WindowBorderEdge borderEdge, IInputElement border)
        {
            #region MouseEnter
            border.MouseEnter += (sender, e) =>
                {
                    if (WindowState != WindowState.Maximized && ResizeMode == ResizeMode.CanResize)
                    {
                        _normalBorderStyle = (Brush)TryFindResource("BorderBrush");
                        _highlightBorderBrush = (Brush)TryFindResource("BorderHighlightBrush");
                        
                        switch (borderEdge)
                        {
                            case WindowBorderEdge.Left:
                                using (var cursor = new OverrideCursor(Cursors.SizeWE))
                                {
                                    _borderLeft.Background = _highlightBorderBrush;
                                }
                                    _borderLeft.Background = _highlightBorderBrush;
                                break;
                            case WindowBorderEdge.Right:
                                using (var cursor = new OverrideCursor(Cursors.SizeWE))
                                {
                                    
                                }
                            //Cursor = Cursors.SizeWE;
                                break;
                            case WindowBorderEdge.Top:
                            case WindowBorderEdge.Bottom:
                                using (var cursor = new OverrideCursor(Cursors.SizeNS))
                                {
                                    
                                }
                                //Cursor = Cursors.SizeNS;
                                break;
                            case WindowBorderEdge.TopLeft:
                            case WindowBorderEdge.BottomRight:
                                using (var cursor = new OverrideCursor(Cursors.SizeNWSE))
                                {
                                    
                                }
                                //Cursor = Cursors.SizeNWSE;
                                break;
                            case WindowBorderEdge.TopRight:
                            case WindowBorderEdge.BottomLeft:
                                using (var cursor = new OverrideCursor(Cursors.SizeNESW))
                                {
                                    
                                }
                                //Cursor = Cursors.SizeNESW;
                                break;
                        }
                    }
                    //else
                    //{
                    //    Cursor = Cursors.Arrow;
                    //}
                };
            #endregion

            #region MouseLeave
            //border.MouseLeave += (sender, e) => Cursor = Cursors.Arrow;
            #endregion

            #region MouseLeftButtonDown
            border.MouseLeftButtonDown += (sender, e) =>
                {
                    if (WindowState != WindowState.Maximized && ResizeMode == ResizeMode.CanResize)
                    {
                        var cursorLocation = e.GetPosition(this);
                        var cursorOffset = new Point();

                        switch (borderEdge)
                        {
                            case WindowBorderEdge.Left:
                                cursorOffset.X = cursorLocation.X;
                                break;
                            case WindowBorderEdge.TopLeft:
                                cursorOffset.X = cursorLocation.X;
                                cursorOffset.Y = cursorLocation.Y;
                                break;
                            case WindowBorderEdge.Top:
                                cursorOffset.Y = cursorLocation.Y;
                                break;
                            case WindowBorderEdge.TopRight:
                                cursorOffset.X = (Width - cursorLocation.X);
                                cursorOffset.Y = cursorLocation.Y;
                                break;
                            case WindowBorderEdge.Right:
                                cursorOffset.X = (Width - cursorLocation.X);
                                break;
                            case WindowBorderEdge.BottomRight:
                                cursorOffset.X = (Width - cursorLocation.X);
                                cursorOffset.Y = (Height - cursorLocation.Y);
                                break;
                            case WindowBorderEdge.Bottom:
                                cursorOffset.Y = (Height - cursorLocation.Y);
                                break;
                            case WindowBorderEdge.BottomLeft:
                                cursorOffset.X = cursorLocation.X;
                                cursorOffset.Y = (Height - cursorLocation.Y);
                                break;
                        }

                        _cursorOffset = cursorOffset;

                        border.CaptureMouse();
                    }
                };
            #endregion

            #region MouseMove
            //border.PreviewMouseMove

            border.MouseMove += (sender, e) =>
                {
                    if (WindowState != WindowState.Maximized && border.IsMouseCaptured && ResizeMode == ResizeMode.CanResize)
                    {
                        var cursorLocation = e.GetPosition(this);

                        var nHorizontalChange = (cursorLocation.X - _cursorOffset.X);
                        var pHorizontalChange = (cursorLocation.X + _cursorOffset.X);
                        var nVerticalChange = (cursorLocation.Y - _cursorOffset.Y);
                        var pVerticalChange = (cursorLocation.Y + _cursorOffset.Y);

                        switch (borderEdge)
                        {
                            case WindowBorderEdge.Left:
                                if (Width - nHorizontalChange <= MinWidth) break;
                                Left += nHorizontalChange;
                                Width -= nHorizontalChange;
                                break;
                            case WindowBorderEdge.TopLeft:
                                if (!(Width - nHorizontalChange <= MinWidth))
                                {
                                    Left += nHorizontalChange;
                                    Width -= nHorizontalChange;
                                }
                                if (!(Height - nVerticalChange <= MinHeight))
                                {
                                    Top += nVerticalChange;
                                    Height -= nVerticalChange;
                                }
                                break;
                            case WindowBorderEdge.Top:
                                if (Height - nVerticalChange <= MinHeight) break;
                                Top += nVerticalChange;
                                Height -= nVerticalChange;
                                break;
                            case WindowBorderEdge.TopRight:
                                if (!(pHorizontalChange <= MinWidth))
                                {
                                    Width = pHorizontalChange;
                                }
                                if (!(Height - nVerticalChange <= MinHeight))
                                {
                                    Top += nVerticalChange;
                                    Height -= nVerticalChange;
                                }
                                break;
                            case WindowBorderEdge.Right:
                                if (pHorizontalChange <= MinWidth) break;
                                Width = pHorizontalChange;
                                break;
                            case WindowBorderEdge.BottomRight:
                                if (!(pHorizontalChange <= MinWidth))
                                {
                                    Width = pHorizontalChange;
                                }
                                if (!(pVerticalChange <= MinHeight))
                                {
                                    Height = pVerticalChange;
                                }
                                break;
                            case WindowBorderEdge.Bottom:
                                if (pVerticalChange <= MinHeight) break;
                                Height = pVerticalChange;
                                break;
                            case WindowBorderEdge.BottomLeft:
                                if (!(Width - nHorizontalChange <= MinWidth))
                                {
                                    Left += nHorizontalChange;
                                    Width -= nHorizontalChange;
                                }
                                if (!(pVerticalChange <= MinHeight))
                                {
                                    Height = pVerticalChange;
                                }

                                break;
                        }
                    }
                };
            #endregion

            #region MouseLeftButtonUp
            border.MouseLeftButtonUp += (sender, e) => border.ReleaseMouseCapture();
            #endregion
        }

        private void RegisterBorders()
        {
            _borderLeft = (Border)GetTemplateChild("PART_WindowBorderLeft");
            _borderTopLeft = (FrameworkElement)GetTemplateChild("PART_WindowBorderTopLeft");
            _borderTop = (FrameworkElement)GetTemplateChild("PART_WindowBorderTop");
            _borderTopRight = (FrameworkElement)GetTemplateChild("PART_WindowBorderTopRight");
            _borderRight = (FrameworkElement)GetTemplateChild("PART_WindowBorderRight");
            _borderBottomRight = (FrameworkElement)GetTemplateChild("PART_WindowBorderBottomRight");
            _borderBottom = (FrameworkElement)GetTemplateChild("PART_WindowBorderBottom");
            _borderBottomLeft = (FrameworkElement)GetTemplateChild("PART_WindowBorderBottomLeft");

            RegisterBorderEvents(WindowBorderEdge.Left, _borderLeft);
            RegisterBorderEvents(WindowBorderEdge.TopLeft, _borderTopLeft);
            RegisterBorderEvents(WindowBorderEdge.Top, _borderTop);
            RegisterBorderEvents(WindowBorderEdge.TopRight, _borderTopRight);
            RegisterBorderEvents(WindowBorderEdge.Right, _borderRight);
            RegisterBorderEvents(WindowBorderEdge.BottomRight, _borderBottomRight);
            RegisterBorderEvents(WindowBorderEdge.Bottom, _borderBottom);
            RegisterBorderEvents(WindowBorderEdge.BottomLeft, _borderBottomLeft);
        }

        private void RegisterCaption()
        {
            _caption = (FrameworkElement)GetTemplateChild("PART_WindowCaption");

            if (_caption != null)
            {
                #region MouseLeftButtonDown
                _caption.MouseLeftButtonDown += (sender, e) =>
                    {
                        _restoreTop = e.GetPosition(this);//.Y;

                        if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left && (ResizeMode != ResizeMode.CanMinimize && ResizeMode != ResizeMode.NoResize))
                        {
                            WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
                            return;
                        }

                        DragMove();
                    };
                #endregion

                #region MouseMove
                _caption.MouseMove += (sender, e) =>
                    {
                        if (e.LeftButton == MouseButtonState.Pressed && _caption.IsMouseOver)
                        {
                            if (WindowState == WindowState.Maximized)
                            {
                                WindowState = WindowState.Normal;
                                Top = _restoreTop.Y - 10;
                                Left = _restoreTop.X - (Width / 2);
                                DragMove();
                            }
                        }
                    };


                #endregion
            }
        }

        private void RegisterCaptionText()
        {
            _captionTextBlock = (TextBlock)GetTemplateChild("PART_WindowCaptionText");
        }

        private void RegisterCloseButton()
        {
            _closeButton = (Button)GetTemplateChild("PART_WindowCaptionCloseButton");

            if (_closeButton != null)
            {
                _closeButton.Style = (Style)TryFindResource("RoundGlassButtonStyle");
                _closeButton.Click += (sender, e) => Close();
            }
        }

        private void RegisterFrame()
        {
            _frame = (FrameworkElement)GetTemplateChild("PART_WindowFrame");
        }

        private void RegisterMaximizeButton()
        {
            _maximizeButton = (Button)GetTemplateChild("PART_WindowCaptionMaximizeButton");

            if (_maximizeButton != null)
            {
                _maximizeButton.Style = (Style)TryFindResource("RoundGlassButtonStyle");
                _maximizeButton.Click += (sender, e) =>
                    {
                        WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                    };
                ToggleMaximizeButtonColor();
            }
        }

        private void RegisterMinimizeButton()
        {
            _minimizeButton = (Button)GetTemplateChild("PART_WindowCaptionMinimizeButton");

            if (_minimizeButton != null)
            {
                _minimizeButton.Style = (Style)TryFindResource("RoundGlassButtonStyle");
                _minimizeButton.Click += (sender, e) => WindowState = WindowState.Minimized;
            }
        }

        private void ToggleMaximizeButtonColor()
        {
            _maximizeButton.Background = WindowState == WindowState.Maximized ? new SolidColorBrush(_maxedColor) : new SolidColorBrush(_normalColor);
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lparam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lparam, typeof(MINMAXINFO));

            const int MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);

                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;

                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lparam, true);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch ((WM)msg)
            {
                case WM.GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lparam);
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private enum WindowBorderEdge
        {
            Left,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft
        }

        private enum WM
        {
            GETMINMAXINFO = 0x0024
        }
    }
}
