using System;
using System.Speech.Recognition;

namespace NexusIM
{
	class SpeechManager
	{
		public static void Setup()
		{
			SpeechRecognizer recognizer = new SpeechRecognizer();
			GrammarBuilder builder = new GrammarBuilder();
			builder.Append("im");
			builder.AppendDictation();
			GrammarBuilder builder2 = new GrammarBuilder();
			builder2.Append("message");
			builder2.AppendDictation();
			GrammarBuilder build = new GrammarBuilder();
			build.Append(builder);
			build.Append(builder2);
			try {
				recognizer.LoadGrammar(new Grammar(build));
			} catch (Exception) {

			}
			recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
			recognizer.EmulateRecognize("im Billy Bob");
		}

		private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			e = e;
		}
	}
}