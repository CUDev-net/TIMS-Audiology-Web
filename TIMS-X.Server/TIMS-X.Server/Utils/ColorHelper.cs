namespace TIMS_X.Server.Utils;

public static class ColorHelper
{
	public static string GetForegroundRgbString(int color)
	{
		var r = (byte)((color >> 0) & 0xFF);
		var g = (byte)((color >> 8) & 0xFF);
		var b = (byte)((color >> 16) & 0xFF);
		var grayscale = r * 0.03 + g * 0.59 + b * 0.11;
		return grayscale < 128 ? "RGB(255, 255, 255)" : "RGB(0, 0, 0)";
	}

	public static string ToRgbString(int color)
	{
		var r = (byte)((color >> 0) & 0xFF);
		var g = (byte)((color >> 8) & 0xFF);
		var b = (byte)((color >> 16) & 0xFF);
		return $"RGB({r}, {g}, {b})";
	}
}