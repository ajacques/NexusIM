using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;

namespace NexusIM.Controls.Inlines
{
	class AccountDisconnectedInline : Span
	{
		public AccountDisconnectedInline()
		{
			Run description = new Run();
			description.Text = "Account Disconnected ";

			Hyperlink reconnect = new Hyperlink();
			reconnect.Inlines.Add(new Run("Reconnect"));
			reconnect.Cursor = Cursors.Hand;
			//reconnect.MouseLeftButtonDown += new MouseButtonEventHandler(Protocol_DoReconnect);

			this.Inlines.Add(description);
			this.Inlines.Add(reconnect);
		}
	}
}
