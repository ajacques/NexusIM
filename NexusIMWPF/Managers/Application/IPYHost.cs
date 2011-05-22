using System;
using System.Security;
using System.Security.Policy;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace NexusIM.Managers
{
	internal static class IPYHost
	{
		public static void Setup()
		{
			CreateSecureDomain();
			mEngine = Python.CreateEngine(mScriptDomain);
			
			ScriptSource test = mEngine.CreateScriptSourceFromString("print 'hello world!'", SourceCodeKind.Statements);
			test.Execute();
		}

		private static void CreateSecureDomain()
		{
			Evidence evidence = new Evidence();
			evidence.AddAssemblyEvidence<Zone>(new Zone(SecurityZone.Untrusted));
			
			mScriptDomain = AppDomain.CreateDomain("ScriptDomain", evidence);
		}

		private static AppDomain mScriptDomain;
		private static ScriptEngine mEngine;
	}
}