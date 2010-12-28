using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NexusCore.DataContracts
{
	public enum LocationServiceType
	{
		GoogleLatitude,
		FireEagle
	}

	public enum PushChannelType
	{
		GenericTcp,
		GenericUdp,
		MicrosoftPN
	}

	/// <summary>
	/// Specifies what devices the message should be sent to
	/// </summary>
	[Flags]
	public enum MessageOptions
	{
		/// <summary>
		/// Sends this message to a single destination (unicast)
		/// </summary>
		None,
		/// <summary>
		/// Sends this message only to devices that are in-active.
		/// </summary>
		SendToInactiveDevices,
		/// <summary>
		/// Sends this message only to devices that are active.
		/// </summary>
		SendToActiveDevices,
		SendToAllDevices,
		SendToAccountMaster
	}

	public enum SwarmStreamReconnectReason
	{
		Maintenance,
		ServerError,
		Security
	}

	public enum SwarmStatusChangeTypes
	{
		Added,
		Removed,
		Online,
		Offline
	}

	[Flags]
	public enum AuthenticationTokenTypes
	{
		AllowCoreLogin,
		AllowSwarmConnect,
		AllowLocationGet,
		AllowAccountGet,
		SingleUse
	}
}