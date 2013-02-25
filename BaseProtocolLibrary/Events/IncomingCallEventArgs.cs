using System;
using System.Collections.Generic;
using System.Net;
using InstantMessage.Protocols.AudioVideo;

namespace InstantMessage.Events
{
	public abstract class IncomingCallEventArgs : EventArgs
	{
		public IncomingCallEventArgs(IContact caller)
		{
			Caller = caller;
			IncomingAudioPayloadTypes = new List<SdpPayloadType>();
			OurTransportCandidates = new List<SdpTransportCandidate>();
			IncomingTransportCandidates = new List<SdpTransportCandidate>();
			AcceptedPayloadTypes = new List<SdpPayloadType>();
		}

		public abstract void AddCandidate(int priority, int candidate, IPEndPoint ep);

		public EventHandler BeginIceSelection;

		public bool Accepted
		{
			get;
			set;
		}
		public string Id
		{
			get;
			set;
		}
		public string IceUfrag
		{
			get;
			set;
		}
		public string IcePassword
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the contact that is attempting to initiate a VoIP call with us.
		/// </summary>
		public IContact Caller
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets a list of payload types that the initator supports
		/// </summary>
		public IList<SdpPayloadType> IncomingAudioPayloadTypes
		{
			get;
			protected set;
		}
		/// <summary>
		/// Represents a list of payload types that we support.
		/// </summary>
		public IList<SdpPayloadType> AcceptedPayloadTypes
		{
			get;
			protected set;
		}
		public IList<SdpTransportCandidate> IncomingTransportCandidates
		{
			get;
			private set;
		}
		public IList<SdpTransportCandidate> OurTransportCandidates
		{
			get;
			private set;
		}
	}
}