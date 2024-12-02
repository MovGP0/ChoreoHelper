using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ReactiveMarbles.ObservableEvents;

namespace ChoreoHelper.Shell;

public sealed partial class ShellWindow : IViewFor<ShellViewModel>, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public ShellWindow()
    {
        InitializeComponent();

        this.Events().MouseDown.Subscribe(e =>
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }).DisposeWith(_disposables);

        MinimizeAppButton.Events().Click.Subscribe(e =>
        {
            WindowState = WindowState.Minimized;
        }).DisposeWith(_disposables);

        MaximizeRestoreButton.Events().Click.Subscribe(e =>
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeRestoreButton.Content = "🗖";
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaximizeRestoreButton.Content = "🗗";
            }
        }).DisposeWith(_disposables);

        CloseAppButton.Events().Click.Subscribe(e =>
        {
            Close();
        }).DisposeWith(_disposables);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        EnableAlignWindowPopup();
    }

    [Conditional("DISABLED")]
    private void EnableAlignWindowPopup()
    {
        if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
        {
            hwndSource.AddHook(HwndSourceHook);
        }
    }

    private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_NCHITTEST = 0x0084;
        const int HTMAXBUTTON = 9;

        if (msg == WM_NCHITTEST)
        {
            Point mousePos = PointFromScreen(new Point((lParam.ToInt32() & 0xFFFF), (lParam.ToInt32() >> 16)));
            Rect maxButtonRect = new Rect(MaximizeRestoreButton.TranslatePoint(new Point(), this), new Size(MaximizeRestoreButton.ActualWidth, MaximizeRestoreButton.ActualHeight));

            if (maxButtonRect.Contains(mousePos))
            {
                handled = true;
                return new IntPtr(HTMAXBUTTON);
            }
        }

        return IntPtr.Zero;
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as ShellViewModel;
    }

    public ShellViewModel? ViewModel { get; set; }

    public void Dispose() => _disposables.Dispose();
}