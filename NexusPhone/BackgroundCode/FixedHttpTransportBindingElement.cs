using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace NexusPhone
{
	public class FixedHttpTransportBindingElement : HttpTransportBindingElement
	{
		public FixedHttpTransportBindingElement() : base()
		{

		}
		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			return (IChannelFactory<TChannel>)new FixedHttpTransportChannelFactory(this, context);
		}

		public override string Scheme
		{
			get {
				return "http";
			}
		}

		public override BindingElement Clone()
		{
			FixedHttpTransportBindingElement elem = new FixedHttpTransportBindingElement();

			return elem;
		}
	}
}