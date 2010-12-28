using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NexusIM.PushChannel
{
	public static class HostFactory
	{
		public static ServiceHost CreateUDPHost(object instance, int port)
		{
			UriBuilder builder = new UriBuilder();
			builder.Host = "localhost";
			builder.Port = port;

			builder.Scheme = "soap.udp";
			Uri udp = builder.Uri;

			builder.Scheme = "net.tcp";
			Uri tcp = builder.Uri;

			ServiceHost host = new ServiceHost(instance, udp, tcp);
			CustomBinding binding = new CustomBinding();
			binding.Elements.Add(new BinaryMessageEncodingBindingElement());
			binding.Elements.Add(new OneWayBindingElement());
			//binding.Elements.Add(new UdpTransportBindingElement());
			binding.Elements.Add(new TcpTransportBindingElement());
			host.AddServiceEndpoint(typeof(ISwarmCallback), binding, "");

			return host;
		}
	}
}