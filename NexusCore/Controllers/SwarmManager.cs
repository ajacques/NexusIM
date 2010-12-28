using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using NexusCore.DataContracts;
using NexusCore.Services;

namespace NexusCore.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Obsolete("", false)]
	public static class SwarmManager
	{
		public static Swarm AddToSwarm(SwarmMember member)
		{
			if (mSwarms.Any(s => s.UserId == member.UserId))
			{
				Swarm single = mSwarms.Where(s => s.UserId == member.UserId).First();
				single.Add(member);
				return single;
			} else {
				Swarm swarm = new Swarm(member.UserId);
				swarm.Add(member);
				mSwarms.Add(swarm);
				return swarm;
			}
		}
		public static Swarm NewSwarm(int userid)
		{
			Swarm sm = new Swarm(userid);

			mSwarms.Add(sm);

			return sm;
		}
		public static void RemoveFromSwarm(int deviceid)
		{

		}
		public static Swarm FindSwarmByUserId(int userid)
		{
			return mSwarms.Where(s => s.UserId == userid).FirstOrDefault();
		}
		public static bool SwarmHasDevice(int deviceid)
		{
			return mSwarms.Any(s => s.Members.Any(sm => sm.DeviceId == deviceid));
		}
		public static SwarmMember GetDevice(int deviceid)
		{
			return mSwarms.First(s => s.Members.Any(sm => sm.DeviceId == deviceid)).Members.First(sm => sm.DeviceId == deviceid);
		}
		public static void SendMessage(int userid, ISwarmMessage message, MessageOptions options)
		{
			Swarm swarm = FindSwarmByUserId(userid);

			if (options == MessageOptions.None)
			{
				SwarmMember member = swarm.Members.First(sm => sm.DeviceId == message.mDeviceRecipient);
				member.Callback.OnSwarmMessage(message);
			} else if (options.HasFlag(MessageOptions.SendToAllDevices)) {
				swarm.Members.ForEach(sm => { message.mDeviceRecipient = sm.DeviceId; sm.Callback.OnSwarmMessage(message); });
			}
		}

		public static List<Swarm> Swarms
		{
			get {
				return mSwarms;
			}
		}
		private static List<Swarm> mSwarms = new List<Swarm>();
	}

	public class Swarm
	{
		public Swarm(int userid)
		{
			mUserId = userid;
		}

		public void Add(SwarmMember member)
		{
			mCallbacks.Add(member);
		}
		public void Remove(SwarmMember member)
		{
			mCallbacks.Remove(member);
		}
		public void SendMessage(ISwarmMessage message)
		{
			message.Swarm = this;
			foreach (var member in mCallbacks)
			{
				message.RecipientDevice = member.DeviceId;
				member.Callback.OnSwarmMessage(message);
			}
		}
		public void SendMessage(ISwarmMessage message, int ignoreDevice)
		{
			message.Swarm = this;
			foreach (SwarmMember member in mCallbacks.Where(s => s.DeviceId != ignoreDevice))
			{
				message.RecipientDevice = member.DeviceId;
				try	{
					member.Callback.OnSwarmMessage(message);
				} catch (Exception) {}
			}
		}

		public int UserId
		{
			get {
				return mUserId;
			}
		}
		public List<SwarmMember> Members
		{
			get {
				return mCallbacks;
			}
		}

		private List<SwarmMember> mCallbacks = new List<SwarmMember>();
		private int mUserId;
	}

	public class SwarmMember
	{
		public SwarmMember(int userid, int deviceid, ISwarmCallback callback)
		{
			mUserId = userid;
			mCallback = callback;
			mDeviceId = deviceid;
		}

		public ISwarmCallback Callback
		{
			get {
				return mCallback;
			}
			set {
				mCallback = value;
			}
		}
		public Swarm Swarm
		{
			get {
				return mSwarm;
			}
		}
		public int DeviceId
		{
			get {
				return mDeviceId;
			}
		}
		public int UserId
		{
			get {
				return mUserId;
			}
		}
				

		private int mDeviceId;
		private int mUserId;
		private ISwarmCallback mCallback;
		private Swarm mSwarm;
	}
}