using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ContactListGroup.xaml
	/// </summary>
	public partial class ContactListGroup : UserControl
	{
		public ContactListGroup()
		{
			this.InitializeComponent();

			Contacts = new List<ContactListItem>();
		}

		public List<ContactListItem> Contacts
		{
			get;
			set;
		}
	}
}