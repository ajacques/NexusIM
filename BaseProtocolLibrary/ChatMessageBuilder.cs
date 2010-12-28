using System;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public class ChatMessageBuilder
	{
		public ChatMessageBuilder() {}
		public void Append(string text)
		{
			if (mNewSpan)
			{
				if (mHasSpan)
					mOutput += "</span>";

				mOutput += "<span style=\"font: " + mFontName + "; color: " + mColorString + "\">";
				mNewSpan = false;
				mHasSpan = true;
			}

			mOutput += text;
		}
		public void SetFont(string name)
		{
			mFontName = name;
			mNewSpan = true;
		}
		public void SetColor(int r, int g, int b)
		{
			
		}

		public override string ToString()
		{
			if (mHasSpan)
				mOutput += "</span>";

			return mOutput;
		}

		private bool mHasSpan = false;
		private bool mNewSpan = false;
		private static string mDefaultFont = "Arial";
		private string mFontName = mDefaultFont;
		private string mColorString = "#000000";
		private string mOutput = "";
	}
}
