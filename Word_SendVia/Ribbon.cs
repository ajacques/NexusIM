using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Forms;

namespace Word_SendVia
{
	[ComVisible(true)]
	public class CustomRibbon : Office.IRibbonExtensibility
	{
		private Office.IRibbonUI ribbon;

		#region IRibbonExtensibility Members

		public string GetCustomUI(string ribbonID)
		{
			return GetResourceText("Word_SendVia.Ribbon.xml");
		}

		#endregion

		#region Ribbon Callbacks
		//Create callback methods here. For more information about adding callback methods, select the Ribbon XML item in Solution Explorer and then press F1

		public void Ribbon_Load(Office.IRibbonUI ribbonUI)
		{
			this.ribbon = ribbonUI;
		}

		public void SendAsAttach(Office.IRibbonControl control)
		{
			Mutex testMutex = new Mutex(false, "Local\\NexusIM");
			bool isRunning = !testMutex.WaitOne(0, false);
			testMutex.Close();
			Word.Document document = Globals.ThisAddIn.Application.Application.ActiveDocument;
			string fileName = document.FullName;

			if (isRunning)
			{
				NamedPipeClientStream cStream = new NamedPipeClientStream("nexusim");
				BinaryWriter writer = new BinaryWriter(cStream);
				writer.Write(IPCMessageTypes.SendFile);
			}
		}

		#endregion

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

	internal static class IPCMessageTypes
	{
		public static const byte SendFile = 0x01;
	}
}