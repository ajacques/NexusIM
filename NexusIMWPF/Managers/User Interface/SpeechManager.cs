﻿using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace NexusIM
{
	static class SpeechManager
	{
		public static void Setup()
		{
			mRecognizer = new SpeechRecognizer();
		}

		private static void BuildGrammar()
		{
			GrammarBuilder builder = new GrammarBuilder();
		}

		private static SpeechRecognizer mRecognizer;
	}
}