using System.Xml;

namespace InstantMessage.Protocols.XMPP.Messages.Jingle
{
	internal abstract class JingleBaseMessage : IqMessage
	{
		protected override void WriteBody(XmlWriter writer)
		{
			writer.WriteStartElement("jingle", Namespace);
			WriteAttribute(writer, "action", Action);
			WriteAttribute(writer, "sid", SessionId);

			if (IsInitiating)
			{
				WriteAttribute(writer, "initiator", Source.ToString());
			} else {
				WriteAttribute(writer, "responder", Source.ToString());
			}

			WritePayload(writer);

			writer.WriteEndElement();
		}

		protected abstract void WritePayload(XmlWriter writer);

		protected string ComputeNamespace(string subnamespace)
		{
			return "urn:xmpp:jingle:" + subnamespace;
		}

		public override string Namespace
		{
			get {
				return ComputeNamespace(SubNamespace);
			}
		}
		protected abstract string SubNamespace
		{
			get;
		}
		protected abstract string Action
		{
			get;
		}
		protected abstract bool IsInitiating
		{
			get;
		}
		public string SessionId
		{
			get;
			set;
		}
		public override IqType Type
		{
			get {
				return IqType.set;
			}
		}
		public override string LocalName
		{
			get {
				return "jingle";
			}
		}
	}
}
