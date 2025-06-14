using System.Drawing;
using SkiaSharp;

namespace ChoreoHelper.TransitionEditor.Extensions;

public static class RectangleFExtensions
{
    /// <summary>
    /// Converts a <see cref="RectangleF"/> to a <see cref="SKRect"/>.
    /// </summary>
    public static SKRect ToSkRect(this RectangleF rect)
    {
        return new SKRect(
            rect.X,
            rect.Y,
            rect.X + rect.Width,
            rect.Y + rect.Height);
    }
}