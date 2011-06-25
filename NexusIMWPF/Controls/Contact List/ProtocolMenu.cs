using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM.Controls
{
	abstract class ProtocolMenu<T> : Button where T : IMProtocol
	{
		public ProtocolMenu()
		{
			this.ContextMenu = new CustomContextMenu(this);
			this.Width = 16;
			this.Height = 16;
		}

		protected abstract string ProtocolName
		{
			get;
		}

		private class CustomContextMenu : ContextMenu
		{
			public CustomContextMenu(ProtocolMenu<T> button)
			{
				mButton = button;
			}

			protected override void OnOpened(RoutedEventArgs e)
			{
				base.OnOpened(e);

				this.Items.Clear();

				IEnumerable<IMProtocolWrapper> protocols = AccountManager.Accounts.Where(w => w.Protocol is T);

				if (protocols.Count() >= 2)
				{
					MenuItem header = new MenuItem();
					header.Header = mButton.ProtocolName;
					header.FontWeight = FontWeight.FromOpenTypeWeight(500);
					header.IsEnabled = false;
					this.Items.Add(header);
					this.Items.Add(new Separator());

					foreach (IMProtocolWrapper wrapper in protocols)
					{
						if (wrapper == null)
							continue;

						MenuItem main = new MenuItem();
						this.Items.Add(main);
						main.Header = wrapper.Protocol.ToString();
						mButton.GenerateItemSet(wrapper, main.Items);
					}
				} else
					mButton.GenerateItemSet(protocols.FirstOrDefault(), this.Items);
			}

			private ProtocolMenu<T> mButton;
		}

		protected override void OnClick()
		{
			base.OnClick();

			this.ContextMenu.IsOpen = true;
		}

		protected abstract void GenerateItemSet(IMProtocolWrapper wrapper, ItemCollection coll);
	}
}