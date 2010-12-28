using System;
using System.Collections;

namespace InstantMessage
{
	public abstract class MessageContents
	{
		public abstract string ToString();
		public bool DisplayTimestamp
		{
			get	{
				return timestampdisplay;
			}
			set	{
				timestampdisplay = value;
			}
		}

		protected bool timestampdisplay = true;
	}
}