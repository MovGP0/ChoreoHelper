using System.Reactive.Disposables;
using SkiaSharp;

namespace ChoreoHelper.Editor;

public sealed class Theme : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    /// <summary>The background color of the grid.</summary>
    public SKColor BackgroundColor { get; } = SKColors.WhiteSmoke;

    /// <summary>The color for figures with Platinum difficulty</summary>
    public SKColor PlatinumColor { get; } = new(0xe5, 0xe4, 0xe2);

    /// <summary>The color for figures with Gold difficulty</summary>
    public SKColor GoldColor { get; } = new(0xd4, 0xaf, 0x37);

    /// <summary>The color for figures with Silver difficulty</summary>
    public SKColor SilverColor { get; } = new(0xc0, 0xc0, 0xc0);

    /// <summary>The color for figures with Bronze difficulty</summary>
    public SKColor BronzeColor { get; } = new(0xcd, 0x7f, 0x32);

    /// <summary>The color for figures with Brass difficulty</summary>
    public SKColor BrassColor { get; } = new(0xb5, 0xa6, 0x42);

    /// <summary>
    /// Paint for drawing the cell borders.
    /// </summary>
    public SKPaint BorderPaint { get; } = new()
    {
        Color = SKColors.Black,
        IsStroke = true,
        StrokeWidth = 1
    };

    /// <summary>
    /// Paint for drawing the cell text.
    /// </summary>
    public SKPaint TextPaint { get; } = new()
    {
        Color = SKColors.Black,
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    public SKPaint PlatinTextPaint { get; } = new()
    {
        Color = new(0xe5, 0xe4, 0xe2),
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    public SKPaint GoldTextPaint { get; } = new()
    {
        Color = new(0xd4, 0xaf, 0x37),
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    public SKPaint SilverTextPaint { get; } = new()
    {
        Color = new(0xc0, 0xc0, 0xc0),
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    /// <summary>
    /// Paint for drawing the cell text in bronze color.
    /// </summary>
    public SKPaint BronzeTextPaint { get; } = new()
    {
        Color = new(0xcd, 0x7f, 0x32),
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    public SKPaint BrassTextPaint { get; } = new()
    {
        Color = new(0xb5, 0xa6, 0x42),
        TextSize = 14,
        IsStroke = true,
        StrokeWidth = 1
    };

    public SKPaint DistanceUnreachablePaint { get; } = new()
    {
        Color = SKColors.White,
        IsStroke = false
    };

    public SKPaint Distance1Paint { get; } = new()
    {
        Color = new SKColor(0x02, 0x45, 0xee),
        IsStroke = false
    };

    public SKPaint Distance2Paint { get; } = new()
    {
        Color = new SKColor(0xee, 0xab, 0x02),
        IsStroke = false
    };

    public SKPaint DistanceInvalidPaint { get; } = new()
    {
        Color = SKColors.Gray,
        IsStroke = false
    };

    public Theme()
    {
        _disposables.Add(TextPaint);
        _disposables.Add(BorderPaint);
        _disposables.Add(PlatinTextPaint);
        _disposables.Add(GoldTextPaint);
        _disposables.Add(SilverTextPaint);
        _disposables.Add(BronzeTextPaint);
        _disposables.Add(BrassTextPaint);
        _disposables.Add(DistanceUnreachablePaint);
        _disposables.Add(Distance1Paint);
        _disposables.Add(Distance2Paint);
        _disposables.Add(DistanceInvalidPaint);
    }

    public void Dispose() => _disposables.Dispose();
}