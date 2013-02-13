using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	interface IJingleDescriptionType
	{
		void WriteBody(XmlWriter writer);

		string SubNamespace
		{
			get;
		}
	}
}
