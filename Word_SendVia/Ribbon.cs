using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Threading;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Text;

namespace Word_SendVia
{
	[ComVisible(true)]
	public class CustomRibbon : Office.IRibbonExtensibility
	{
		private Office.IRibbonUI ribbon;

		public string GetCustomUI(string ribbonID)
		{
			return GetResourceText("Word_SendVia.Ribbon.xml");
		}
		public void Ribbon_Load(Office.IRibbonUI ribbonUI)
		{
			this.ribbon = ribbonUI;
		}
		public void SendAsAttach(Office.IRibbonControl control)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(SendAsAttachAsync));
		}
		private void SendAsAttachAsync(object sender)
		{
			Mutex testMutex = new Mutex(false, "Local\\NexusIM");
			bool isRunning = !testMutex.WaitOne(0, false);
			testMutex.Close();
			Word.Document document = Globals.ThisAddIn.Application.Application.ActiveDocument;
			string fileName = document.FullName;

			if (isRunning)
			{
				NamedPipeClientStream cStream = new NamedPipeClientStream("nexusim");
				cStream.Connect(500);
				StreamWriter writer = new StreamWriter(cStream);
				writer.Write("SENDIM file=b64:");
				string fNameb64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(fileName));
				writer.WriteLine(fNameb64);
				writer.Close();
			}
		}

		#region Helpers

		private static string GetResourceText(string resourceName)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			string[] resourceNames = asm.GetManifestResourceNames();
			for (int i = 0; i < resourceNames.Length; ++i)
			{
				if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
					{
						if (resourceReader != null)
						{
							return resourceReader.ReadToEnd();
						}
					}
				}
			}
			return null;
		}

		#endregion
	}
}