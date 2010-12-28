using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NexusIM.Managers
{
	class SafeUrlTester
	{
		public static void IsUrlSafe(string url)
		{

		}
	}

	public interface IUrlTester
	{
		bool IsUrlSafe(string url);
	}

	public class GoogleSafeBrowsing : IUrlTester
	{
		public void Setup()
		{
			HttpWebRequest request = WebRequest.Create(setupurl) as HttpWebRequest;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			string data = (new StreamReader(response.GetResponseStream())).ReadToEnd();
			Match mx = Regex.Match(data, "clientkey:[0-9]*:([a-zA-Z0-9=_\\]*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Match mx2 = Regex.Match(data, "wrappedkey:[0-9]*:([a-zA-Z0-9=_\\]*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			clientkey = mx.Groups[1].Value;
			wrappedkey = mx2.Groups[1].Value;
		}

		public bool IsUrlSafe(string url)
		{
			Random r = new Random();
			string nonce = r.Next(0, Int32.MaxValue - 1).ToString();
			string encparams = encryptParameters(clientkey, nonce, url);

			HttpWebRequest request = WebRequest.Create(String.Format(queryurl, nonce, wrappedkey, encparams)) as HttpWebRequest;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			string data = (new StreamReader(response.GetResponseStream())).ReadToEnd();

			return true;
		}

		private string encryptParameters(string clientkey, string nonce, string query)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			byte[] output = md5.ComputeHash(encoder.GetBytes(clientkey + nonce));

			RC4 crypto = new RC4();
			crypto.Key = output;
			ICryptoTransform crypt = crypto.CreateEncryptor();
			byte[] encryptdata = crypt.TransformFinalBlock(encoder.GetBytes(query), 0, query.Length);

			return Convert.ToBase64String(encryptdata);
		}

		private Encoding encoder = Encoding.Default;
		private string clientkey;
		private string wrappedkey;
		private string setupurl = "https://sb-ssl.google.com/safebrowsing/getkey?client=api";
		private string queryurl = "http://safebrowsing.clients.google.com/safebrowsing/lookup?sourceid=nexusim&features=TrustRank&client=navclient-auto-tbff&encver=1&nonce={0}&wrkey={1}&encparams={2}";
	}

	class Ut<T>
	{
		public static void Swap(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

	}

	internal class RC4Cryptor : ICryptoTransform
	{
		internal byte[] S;

		public bool CanReuseTransform { get { throw new NotImplementedException(); } }
		public bool CanTransformMultipleBlocks { get { throw new NotImplementedException(); } }
		public int InputBlockSize { get { throw new NotImplementedException(); } }
		public int OutputBlockSize { get { throw new NotImplementedException(); } }
		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] r = new byte[inputCount];
			for (int i = 0, j = 0; i < inputCount; )
			{
				j += S[++i % 256];
				Ut<byte>.Swap(ref S[i % 256], ref S[j % 256]);
				int t = S[i % 256] + S[j % 256];
				r[i - 1] = (byte)(inputBuffer[inputOffset + i - 1] ^ S[t % 256]);
			}
			return r;
		}
	}

	public class RC4 : SymmetricAlgorithm
	{

		byte[] S = new byte[256];
		byte[] K = new byte[256];

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			throw new NotImplementedException();
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			throw new NotImplementedException();
		}

		public override void GenerateIV()
		{
			throw new NotImplementedException();
		}

		public override void GenerateKey()
		{
			var rnd = new Random();
			var key = new byte[16];
			rnd.NextBytes(key);
			Key = key;
		}

		public override ICryptoTransform CreateEncryptor()
		{
			return new RC4Cryptor() { S = (byte[])S.Clone() };
		}

		public override ICryptoTransform CreateDecryptor()
		{
			return new RC4Cryptor() { S = (byte[])S.Clone() };
		}

		public override byte[] Key
		{
			set
			{
				base.Key = value;

				for (int i = 0; i < S.Length; ++i)
				{
					S[i] = (byte)i;
					K[i] = Key[i % Key.Length];
				}

				for (int i = 0, j = 0; i < S.Length; ++i)
					Ut<byte>.Swap(ref S[i], ref S[j = (j + S[i] + K[i]) % 256]);
			}
		}
	}
}