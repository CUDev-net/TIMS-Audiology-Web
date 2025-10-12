using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable SYSLIB0022

namespace TIMS_X.Server.Utils;

public static class CryptographyHelper
{
	#region CryptographyHelper Members

	public static string Decrypt(string value, string password)
	{
		return Decrypt<RijndaelManaged>(value, password);
	}

	public static byte[] Decrypt(byte[] value, string password)
	{
		int byteCount;
		return Decrypt<RijndaelManaged>(value, password, out byteCount);
	}

	public static string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
	{
		var valueBytes = Convert.FromBase64String(value);
		int decryptedByteCount;
		var decrypted = Decrypt<T>(valueBytes, password, out decryptedByteCount);
		return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
	}

	public static byte[] Decrypt<T>(byte[] value, string password, out int decryptedByteCount)
		where T : SymmetricAlgorithm, new()
	{
		var vectorBytes = Encoding.ASCII.GetBytes(mVector);
		var saltBytes = Encoding.ASCII.GetBytes(mSalt);

		byte[] decrypted;
		decryptedByteCount = 0;

		using (var cipher = new T())
		{
			var _passwordBytes = new PasswordDeriveBytes(password, saltBytes, mHash, mIterations);
			var keyBytes = _passwordBytes.GetBytes(mKeySize / 8);

			cipher.Mode = CipherMode.CBC;

			try
			{
				using (var decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
				{
					using (var from = new MemoryStream(value))
					{
						using (var reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
						{
							using (var output = new MemoryStream())
							{
								var buffer = new byte[1024];
								var bytesRead = reader.Read(buffer, 0, buffer.Length);
								while (bytesRead > 0)
								{
									decryptedByteCount += bytesRead;
									output.Write(buffer, 0, bytesRead);
									bytesRead = reader.Read(buffer, 0, buffer.Length);
								}

								decrypted = output.ToArray();
							}
						}
					}
				}
			}
			catch (Exception)
			{
				return new byte[0];
			}

			cipher.Clear();
		}

		return decrypted;
	}

	public static string Encrypt(string value, string password)
	{
		return Encrypt<RijndaelManaged>(value, password);
	}

	public static byte[] Encrypt(byte[] value, string password)
	{
		return Encrypt<RijndaelManaged>(value, password);
	}

	public static string Encrypt<T>(string value, string password)
		where T : SymmetricAlgorithm, new()
	{
		var valueBytes = Encoding.ASCII.GetBytes(value);
		var result = Encrypt<T>(valueBytes, password);
		return Convert.ToBase64String(result);
	}

	public static byte[] Encrypt<T>(byte[] value, string password)
		where T : SymmetricAlgorithm, new()
	{
		var vectorBytes = Encoding.ASCII.GetBytes(mVector);
		var saltBytes = Encoding.ASCII.GetBytes(mSalt);


		byte[] encrypted;
		using (var cipher = new T())
		{
			var _passwordBytes =
				new PasswordDeriveBytes(password, saltBytes, mHash, mIterations);
			var keyBytes = _passwordBytes.GetBytes(mKeySize / 8);

			cipher.Mode = CipherMode.CBC;

			using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
			{
				using (var to = new MemoryStream())
				{
					using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
					{
						writer.Write(value, 0, value.Length);
						writer.FlushFinalBlock();
						encrypted = to.ToArray();
					}
				}
			}

			cipher.Clear();
		}

		return encrypted;
	}

	#endregion CryptographyHelper Members

	#region Fields

	private static readonly int mIterations = 2;
	private static readonly int mKeySize = 256;
	private static readonly string mHash = "SHA1";
	private static readonly string mSalt = "aselrias38490a32";
	private static readonly string mVector = "8947az34awl34kjq";

	#endregion Fields
}