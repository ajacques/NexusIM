using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols.VOIP
{
	public enum VOIPCallStatus
	{
		NotConnect,
		Connecting,
		Established
	}

	public interface IVOIPCall
	{
		void Hangup();

		/// <summary>
		/// Gets a modifiable collection that contains all of the profiles that will be established
		/// </summary>
		IList<IMediaProfile> Profiles
		{
			get;
		}
	}
}