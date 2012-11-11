using System;
using System.Text;
using System.Xml;

namespace InstantMessage.Protocols.XMPP
{
	internal class PlainAuthMessage : SaslAuthMessage
	{
		public PlainAuthMessage(string username, string password) : base(username, password) {}

		protected override void WriteAuthBody(XmlWriter writer)
		{
			byte[] busername = Encoding.GetBytes(Username);
			byte[] bpassword = Encoding.GetBytes(Password);
			byte[] data = new byte[busername.Length + bpassword.Length + 2];
			Buffer.BlockCopy(busername, 0, data, 1, busername.Length);
			Buffer.BlockCopy(bpassword, 0, data, busername.Length + 2, bpassword.Length);

			writer.WriteBase64(data, 0, data.Length);
		}

		protected override string Mechanism
		{
			get {
				return "PLAIN";
			}
		}
	}
}
