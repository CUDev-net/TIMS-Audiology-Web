using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Services;

/// <summary>
///     Writer to write out NHA data
/// </summary>
internal class NoahBlobWriter : IDisposable
{
	#region Constructors

	internal NoahBlobWriter(MemoryStream ms)
	{
		mWriter = new BinaryWriter(ms);
	}

	#endregion Constructors

	#region NoahBlobWriter Members

	/// <summary>
	///     Disposes of writer
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
	}

	#endregion NoahBlobWriter Members

	#region Protected Members

	/// <summary>
	///     Disposes of writer
	/// </summary>
	/// <param name="disposing">True if disposing</param>
	/// <returns></returns>
	protected void Dispose(bool disposing)
	{
		// Only dispose once!
		if (false == mDisposed)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
				//Free other state (managed objects).
				if (mWriter != null)
				{
					mWriter.Dispose();
					mWriter = null;
				}
			}

			//Free your own state (unmanaged objects).
			mDisposed = true;
		}
	}

	#endregion Protected Members

	#region Private Members

	private BinaryWriter mWriter;
	private bool mDisposed;

	#endregion Private Members

	#region Internal Members

	/// <summary>
	///     Gets or sets the position of the writer
	/// </summary>
	internal long Position
	{
		get => mWriter.BaseStream.Position;
		set => mWriter.BaseStream.Position = value;
	}

	/// <summary>
	///     Serialized data to nha format
	/// </summary>
	/// <param name="anything"></param>
	/// <returns></returns>
	internal static byte[] RawSerialize(object anything)
	{
		var rawsize = Marshal.SizeOf(anything);
		var buffer = Marshal.AllocHGlobal(rawsize);
		Marshal.StructureToPtr(anything, buffer, false);
		var rawdatas = new byte[rawsize];
		Marshal.Copy(buffer, rawdatas, 0, rawsize);
		Marshal.FreeHGlobal(buffer);
		return rawdatas;
	}

	/// <summary>
	///     Writes out a blob of data
	/// </summary>
	/// <param name="data">Data</param>
	/// <param name="type"></param>
	/// <param name="compressed">True if compression needed</param>
	/// <returns>Bytes writte, current location</returns>
	internal long WriteBlobAsync(byte[] data, uint type, bool compressed = false)
	{
		try
		{
			if (compressed)
			{
				var orglength = data.Length;
				data = CompressionHelper.CompressAsync(data);
				type = type | 0x80000000;

				var length = data.Length + sizeof(int) + sizeof(uint) + sizeof(int);
				mWriter.Write(length);
				mWriter.Write(type);
				mWriter.Write(orglength);
			}
			else
			{
				var length = data.Length + sizeof(int) + sizeof(uint);
				mWriter.Write(length);
				mWriter.Write(type);
			}

			mWriter.Write(data);
		}
		catch (Exception e)
		{
			Trace.WriteLine(string.Format("WriteBlob: {0}", e));
		}

		return mWriter.BaseStream.Position;
	}

	#endregion Internal Members
}