using System.Xml;
using System.Text;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal abstract class XmppMessage
	{
		protected void WriteAttribute(XmlWriter writer, string name, object value)
		{
			writer.WriteStartAttribute(name);
			writer.WriteValue(value);
			writer.WriteEndAttribute();
		}

		protected Encoding Encoding
		{
			get {
				return Encoding.UTF8;
			}
		}

		public abstract void WriteMessage(XmlWriter writer);
	}
}
