using System.Drawing;

namespace TIMS_X.BLL.Utilities;

public static class ColorHelper
{
    public static string GetHexColor(int? colorFromDb)
    {
        if (!colorFromDb.HasValue)
            return null;

        var dbColor = ColorTranslator.FromWin32(colorFromDb.Value);
        return $"#{dbColor.R:X2}{dbColor.G:X2}{dbColor.B:X2}";
    }

    public static int GetWindowsColor(string hexColor)
    {
        var color = ColorTranslator.FromHtml(hexColor);
        return ColorTranslator.ToWin32(color);
    }
}