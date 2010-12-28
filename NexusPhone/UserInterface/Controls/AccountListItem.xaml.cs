using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NexusPhone.NexusCore;

namespace NexusPhone.UserInterface
{
	public partial class AccountListItem : UserControl
	{
		public AccountListItem()
		{
			InitializeComponent();
		}

		internal AccountListItem(AccountInfo info) : this()
		{
			Username.Text = info.Username;
		}
	}
}
