using UnityEngine;
using UnityEngine.UIElements;


public struct RectOffsetFloat
{
    public RectOffsetFloat(float left, float right, float top, float bottom)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }

    public float Left { get; private set; }
    public float Right { get; private set; }
    public float Top { get; private set; }
    public float Bottom { get; private set; }
}


public static class IPanelExtensions
{
    public static RectOffsetFloat GetSafeArea(this IPanel panel)
    {
        var safeLeftTop = RuntimePanelUtils.ScreenToPanel(
            panel,
            new Vector2(Screen.safeArea.xMin, Screen.height - Screen.safeArea.yMax)
        );
        var safeRightBottom = RuntimePanelUtils.ScreenToPanel(
            panel,
            new Vector2(Screen.width - Screen.safeArea.xMax, Screen.safeArea.yMin)
        );
        
        return new RectOffsetFloat(
            safeLeftTop.x,
            safeRightBottom.x,
            safeLeftTop.y,
            safeRightBottom.y
        );
    }
}
