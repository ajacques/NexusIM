﻿using System;
using Office = Microsoft.Office.Core;

namespace Word_SendVia
{
	public partial class ThisAddIn
	{
		private void ThisAddIn_Startup(object sender, EventArgs e)
		{
			
		}

		private void ThisAddIn_Shutdown(object sender, EventArgs e)
		{
		}

		protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
		{
			return new CustomRibbon();
		}

		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(ThisAddIn_Startup);
			this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
		}
		
		#endregion
	}
}
