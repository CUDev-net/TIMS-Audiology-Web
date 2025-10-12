using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace TIMS_X.Server.Utils;

public static class CompressionHelper
{
	public static byte[] CompressAsync(byte[] input)
	{
		if (input == null || input.Length == 0) return input;

		try
		{
			using (var output = new MemoryStream())
			{
				var deflater = new Deflater(Deflater.DEFAULT_COMPRESSION);
				deflater.SetInput(input);
				deflater.Finish();
				var buffer = new byte[4096];
				while (!deflater.IsFinished)
				{
					var got = deflater.Deflate(buffer);
					output.Write(buffer, 0, got);
					if (got == 0) break;
				}

				return output.ToArray();
			}
		}
		catch (Exception)
		{
			// ignored
		}

		return input;
	}

	public static byte[] Decompress(byte[] input)
	{
		if (input == null || input.Length == 0) return input;

		try
		{
			using (var output = new MemoryStream())
			{
				var inflater = new Inflater();
				inflater.SetInput(input);
				var buffer = new byte[4096];
				while (!inflater.IsFinished)
				{
					var got = inflater.Inflate(buffer);
					output.Write(buffer, 0, got);
					if (got == 0) break;
				}

				return output.ToArray();
			}
		}
		catch (Exception)
		{
			// ignored
		}

		return input;
	}

	public static byte[] DecompressAsync(byte[] input)
	{
		if (input == null || input.Length == 0) return input;

		try
		{
			using (var output = new MemoryStream())
			{
				var inflater = new Inflater();
				inflater.SetInput(input);
				var buffer = new byte[4096];
				while (!inflater.IsFinished)
				{
					var got = inflater.Inflate(buffer);
					output.Write(buffer, 0, got);
					if (got == 0) break;
				}

				return output.ToArray();
			}
		}
		catch (Exception)
		{
			// ignored
		}

		return input;
	}
}