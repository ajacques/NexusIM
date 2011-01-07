using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Data.Services;
using System.ServiceModel.Web;

namespace NexusWeb
{
	internal class NewsFeedServiceHost : WebServiceHost
	{
		public NewsFeedServiceHost(Type serviceType, Uri[] baseAddresses) : base(serviceType, baseAddresses)
		{
			
		}
	}

	public class NewsFeedServiceHostFactory : ServiceHostFactory
	{
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new NewsFeedServiceHost(serviceType, baseAddresses);
		}
	}
}