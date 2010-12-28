using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NexusCore.PushChannel
{
	internal interface IPushMessage : IEquatable<IPushMessage>
	{
		XmlElement XmlMessage
		{
			get;
		}
		PushMessageClass MessageClass
		{
			get;
		}
		TimeSpan MaxQueuePeriod
		{
			get;
		}
	}
	internal interface IToastPushMessage : IPushMessage
	{
		string Text1
		{
			get;
		}
		string Text2
		{
			get;
		}
	}
	internal interface ITilePushMessage : IPushMessage
	{
		Uri BackgroundImage
		{
			get;
		}
		int Count
		{
			get;
		}
	}
}