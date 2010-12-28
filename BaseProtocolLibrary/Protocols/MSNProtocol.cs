using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace InstantMessage
{
	public class IMMSNProtocol : IMProtocol
	{
		public IMMSNProtocol()
		{
			protocolType = "MSN";
			mProtocolTypeShort = "msn";
			supportsUserInvisiblity = true;
		}
		public override void BeginLogin()
		{
			base.BeginLogin();

			beginAuthentication();
		}
		public override void Disconnect()
		{
			socket.Close();
		}

		// Authentication Sequence
		private void beginAuthentication()
		{
			IPAddress[] lookup = Dns.GetHostAddresses("messenger.hotmail.com");
			ConnectTo(lookup[0]);
		}

		private void ConnectTo(IPAddress address)
		{
			socket = new TcpClient();
			socket.Connect(address, 1863);

			sReader = new StreamReader(socket.GetStream());
			sWriter = new StreamWriter(socket.GetStream());
			sWriter.AutoFlush = true;

			sendPacket("VER", mProtocolVersion, "MSNP17", "CVR0");

			receivePacket(null);

			sendPacket("CVR", "0x0409", "winnt", "6.1", "i386", "MSNMSGR", mLiveMessengerVersion, "msmsgs", Username);
			sendPacket("USR", "SSO", "I", Username);

			receivePacket(new EventHandler<PacketEventArgs>(ParsePacket));
		}

		private void ParsePacket(object sender, PacketEventArgs e)
		{
			string[] f = e.PacketData.Replace("\n", "").Split('\r');
			
			foreach (string packet in f)
			{
				string[] parts = packet.Split(' ');
				
				if (parts[0] == "XFR")
				{
					socket.Close();
					string newip = parts[3].Split(':')[0];
					ConnectTo(IPAddress.Parse(newip));
					return;
				} else if (parts[0] == "GCF") {
					//policy = f[3];
					sendPacket("USR", "SSO", "S", getSSOAuth());
				}

			}

			receivePacket(new EventHandler<PacketEventArgs>(ParsePacket));
		}

		private void sendPacket(string type, params string[] param)
		{
			string cmd = type + " " + pcount.ToString() + " ";

			for (int i = 0; i < param.Length; i++)
			{
				cmd += param[i];
				if (i != param.Length - 1)
				{
					cmd += " ";
				}
			}
			pcount++;

			sWriter.WriteLine(cmd);
		}

		private string getSSOAuth()
		{
			HttpWebRequest request;
			
			// Windows
			if (!Username.Contains("@msn.com"))
				request = (HttpWebRequest)WebRequest.Create("https://login.live.com/RST.srf");
			else
				request = (HttpWebRequest)WebRequest.Create("https://msnia.login.live.com/pp550/RST.srf");

			request.Method = "POST";
			string SSOAuth = generateSSOAuthRequest();

			Stream wStream = request.GetRequestStream();
			StreamWriter wsWrite = new StreamWriter(wStream);
			wsWrite.Write(SSOAuth);
			wsWrite.Flush();
			wStream.Close();

			string output = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(output);

			XmlNamespaceManager nmgr = new XmlNamespaceManager(xml.NameTable);
			nmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
			nmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			nmgr.AddNamespace("wsa", "http://schemas.xmlsoap.org/ws/2004/03/addressing");
			nmgr.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
			nmgr.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2004/04/trust");
			nmgr.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
			nmgr.AddNamespace("S", "http://schemas.xmlsoap.org/soap/envelope/");
			nmgr.AddNamespace("wsse", "http://schemas.xmlsoap.org/ws/2003/06/secext");

			XmlNode nsecret = xml.SelectSingleNode("/S:Envelope/S:Body/wst:RequestSecurityTokenResponseCollection/wst:RequestSecurityTokenResponse[wst:TokenType='urn:oasis:names:tc:SAML:1.0']/wst:RequestedProofToken/wst:BinarySecret", nmgr);
			XmlNode nticket = xml.SelectSingleNode("/S:Envelope/S:Body/wst:RequestSecurityTokenResponseCollection/wst:RequestSecurityTokenResponse[wst:TokenType='urn:passport:compact']/wst:RequestedSecurityToken/wsse:BinarySecurityToken", nmgr);

			SSOTicket sTicket = new SSOTicket(nsecret.InnerText, nticket.InnerText);

			return nticket.InnerText + " " + sTicket.value;
		}
		private string generateSSOAuthRequest()
		{
			string xml = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2003/06/secext\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2002/12/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://schemas.xmlsoap.org/ws/2004/03/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2004/04/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2004/04/trust\"><Header>";
			xml += "<ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">";
			xml += "<ps:HostingApp>{7108E71A-9926-4FCB-BCC9-9A9D3F32E423}</ps:HostingApp>";
			xml += "<ps:BinaryVersion>4</ps:BinaryVersion>";
			xml += "<ps:UIVersion>1</ps:UIVersion>";
			xml += "<ps:Cookies></ps:Cookies>";
			xml += "<ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>";
			xml += "</ps:AuthInfo>";
			xml += "<wsse:Security><wsse:UsernameToken Id=\"user\">";
			xml += "<wsse:Username>" + Username + "</wsse:Username>";
			xml += "<wsse:Password>" + Password + "</wsse:Password>";
			xml += "</wsse:UsernameToken></wsse:Security></Header><Body>";
			xml += "<ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">";
			xml += "<wst:RequestSecurityToken Id=\"RST0\">";
			xml += "<wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>";
			xml += "<wsp:AppliesTo><wsa:EndpointReference><wsa:Address>http://Passport.NET/tb";
			xml += "</wsa:Address></wsa:EndpointReference></wsp:AppliesTo></wst:RequestSecurityToken>";
			xml += "<wst:RequestSecurityToken Id=\"RST1\">";
			xml += "<wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference>";
			xml += "<wsa:Address>messengerclear.live.com</wsa:Address></wsa:EndpointReference></wsp:AppliesTo>";
			xml += "<wsse:PolicyReference URI=\"" + policy + "\"></wsse:PolicyReference></wst:RequestSecurityToken>";
			xml += "<wst:RequestSecurityToken Id=\"RST2\">";
			xml += "<wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>";
			xml += "<wsp:AppliesTo>";
			xml += "<wsa:EndpointReference>";
			xml += "<wsa:Address>contacts.msn.com</wsa:Address>";
			xml += "</wsa:EndpointReference>";
			xml += "</wsp:AppliesTo>";
			xml += "<wsse:PolicyReference URI=\"MBI\">";
			xml += "</wsse:PolicyReference>";
			xml += "</wst:RequestSecurityToken>";
			xml += "</ps:RequestMultipleSecurityTokens></Body></Envelope>";

			return xml;
		}

		// Random Junk - Clean Me
#if PocketPC || WINDOWS
		private void OnPacketReceive(IAsyncResult e)
		{
			EventHandler<PacketEventArgs> callback = (EventHandler<PacketEventArgs>)e.AsyncState;
			string pktdata = dEncoding.GetString(dataqueue, 0, dataqueue.Length);
#else
		private void OnPacketReceive(object sender, SocketAsyncEventArgs e)
		{
			EventHandler<PacketEventArgs> callback = (EventHandler<PacketEventArgs>)e.UserToken;
			string pktdata = dEncoding.GetString(e.Buffer, 0, e.Buffer.Length);
		
#endif
			pktdata = pktdata.Trim(new char[] { '\0' });
			if (callback != null)
				callback(this, new PacketEventArgs(pktdata));
		}
		private void receivePacket(EventHandler<PacketEventArgs> callback)
		{
			if (callback != null)
				callback(this, new PacketEventArgs(sReader.ReadToEnd()));
#if PocketPC || WINDOWS
			//dataqueue = new byte[dataqueue.Length];
			//socket.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(OnPacketReceive), callback);
#else
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.Completed += new EventHandler<SocketAsyncEventArgs>(OnPacketReceive);
			args.UserToken = callback;

			socket.ReceiveAsync(args);
#endif
		}

		private class SSOTicket
		{
			public SSOTicket(string key, string nonce)
			{
				// First of all, we need to create a structure of information, which elements' size is 4 bytes 
				// To do that, we use an array which we can turn into a string, later...
				Beginning = new byte[28];

				//StructHeaderSize = 28
				Beginning[0] = 0x1c;
				Beginning[1] = 0x00;
				Beginning[2] = 0x00;
				Beginning[3] = 0x00;

				//CryptMode = 1
				Beginning[4] = 0x01;
				Beginning[5] = 0x00;
				Beginning[6] = 0x00;
				Beginning[7] = 0x00;

				//CipherType = 0x6603
				Beginning[8] = 0x03;
				Beginning[9] = 0x66;
				Beginning[10] = 0x00;
				Beginning[11] = 0x00;

				//HashType = 0x8004
				Beginning[12] = 0x04;
				Beginning[13] = (byte)0x80;
				Beginning[14] = 0x00;
				Beginning[15] = 0x00;

				//IV length = 8
				Beginning[16] = 0x08;
				Beginning[17] = 0x00;
				Beginning[18] = 0x00;
				Beginning[19] = 0x00;

				//hash length = 20
				Beginning[20] = 0x14;
				Beginning[21] = 0x00;
				Beginning[22] = 0x00;
				Beginning[23] = 0x00;

				//cipher length = 72
				Beginning[24] = 0x48;
				Beginning[25] = 0x00;
				Beginning[26] = 0x00;
				Beginning[27] = 0x00;

				// now, we have to create a first, base64 decoded key, which we get from the input key
				byte[] key1 = Convert.FromBase64String(key);

				// then we calculate a second key through a specific algorithm (see function DeriveKey())
				string key2 = DeriveKey(key1, "WS-SecureConversationSESSION KEY HASH");

				// ...and a third key with the same algorithm...
				string key3 = DeriveKey(key1, "WS-SecureConversationSESSION KEY ENCRYPTION");

				// now we will use sha1 to create a hash from the nonce
				HMACSHA1 sha = new HMACSHA1();

				// the key for the algorithm is the second key we calculated above
				sha.Key = Encoding.Default.GetBytes(key2);

				// compute the hash
				byte[] hash = sha.ComputeHash(Encoding.Default.GetBytes(nonce));


				// now, we will use TrippleDES algorithm to transform the nonce to a block of 72 bytes... 
				// create the initialization vector (which number's are not important, but better use random ;-))
				byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7 };


				TripleDESCryptoServiceProvider DES3 = new TripleDESCryptoServiceProvider();
				DES3.Key = Encoding.Default.GetBytes(key3);
				DES3.Mode = CipherMode.CBC;
				DES3.IV = iv;

				ICryptoTransform Encryptor = DES3.CreateEncryptor();


				// we have to fill the nonce with 8*8
				byte[] RestOfNonce = { 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08 };

				// this will be our output after the transforming 
				byte[] output = new byte[72];

				byte[] nonceTransform = Combine(Encoding.Default.GetBytes(nonce), RestOfNonce);

				// now, transform the nonce 
				Encryptor.TransformBlock(nonceTransform, 0, nonceTransform.Length, output, 0);

				// the final key will be a base64 encoded structure, composed by the beginning of the structure, the initialization vector, the SHA1 - Hash and the transformed block
				string struc = Encoding.Default.GetString(Beginning) + Encoding.Default.GetString(iv) + Encoding.Default.GetString(hash) + Encoding.Default.GetString(output);
				value = Convert.ToBase64String(Encoding.Default.GetBytes(struc));
			}

			// combine two byte arrays
			private byte[] Combine(byte[] a, byte[] b)
			{
				byte[] c = new byte[a.Length + b.Length];
				System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
				System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
				return c;
			}

			// specific algorithm to calculate a key...
			private string DeriveKey(byte[] key, string magic)
			{
				HMACSHA1 sha = new HMACSHA1();
				sha.Key = key;
				byte[] Magic = Encoding.Default.GetBytes(magic);

				// compute 4 hashes with HMACSHA1
				byte[] hash1 = sha.ComputeHash(Magic);
				byte[] hash2 = sha.ComputeHash(Combine(hash1, Magic));
				byte[] hash3 = sha.ComputeHash(hash1);
				byte[] hash4 = sha.ComputeHash(Combine(hash3, Magic));

				// create an array with the 4 first bytes of the fourth hash
				byte[] o = { hash4[0], hash4[1], hash4[2], hash4[3] };

				// combine it with hash2 and return the key
				return Encoding.Default.GetString(Combine(hash2, o));
			}

			public string value;    // the ticket in form of a string

			private byte[] Beginning;
		}

		// These change with protocol changes
		private string mProtocolVersion = "MSNP18";
		private string mLiveMessengerVersion = "14.0.8089.0726";

		private byte[] dataqueue = new byte[1024];
		private Encoding dEncoding = Encoding.GetEncoding("windows-1252");
		private TcpClient socket;
		private StreamWriter sWriter;
		private StreamReader sReader;
		private uint pcount = 1;
		private string policy = "";
	}
}