using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace InstantMessage.Events
{
	public class CertErrorEventArgs : IMErrorEventArgs
	{
		public CertErrorEventArgs(X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) : base(IMProtocolErrorReason.Warning)
		{
			base.IsUserCorrectable = true;
			base.Message = "An X.509 certificate error has occurred.";

			Certificate = certificate;
			Chain = chain;
			PolicyErrors = errors;
		}

		public X509Certificate Certificate
		{
			get;
			private set;
		}
		public X509Chain Chain
		{
			get;
			private set;
		}
		public SslPolicyErrors PolicyErrors
		{
			get;
			private set;
		}
		public bool Continue
		{
			get;
			set;
		}
	}
}