using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using NexusIM.Controls;

namespace NexusIM.Windows
{
	/// <summary>
	/// A container window that hosts the actual areas (ContactChatArea, etc).
	/// Can use tabs, or the tabs can be hidden.
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatWindow()
		{
			this.InitializeComponent();
		}

		public void AttachAreaAndShow(TabItem tabPage)
		{
			if (tabPage is ContactChatAreaHost)
			{
				ContactChatAreaHost host = (ContactChatAreaHost)tabPage;
				
				host.HostWindow = this;
			}
		}
		public void AttachAreaAndShow(UIElement element)
		{
			Dispatcher.InvokeIfRequired(() =>
				{
					ChatArea.Children.Add(element);
					Grid.SetRow(element, 1);

					UIElement button = GenerateTabButton(element);

					TabButtons.Children.Add(button);
					TabButtons.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Auto) });

					Grid.SetColumn(button, TabButtons.ColumnDefinitions.Count - 1);
				});
		}
		public void HandleTabClose(TabItem tabPage)
		{
		}
		public void IncrementUnread(int step = 1)
		{
			Dispatcher.InvokeIfRequired(() =>
			{
				if (this.IsFocused)
					return;

				mUnread += step;

				UpdateWindowTitle();
			});
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			mUnread = 0;
			UpdateWindowTitle();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (e.Source == this)
				base.DragMove();
		}
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			UpdateGlass();
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (e.Key == Key.Escape)
				this.Close(); // Close the window if the user presses escape
		}
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);

			e.Effects = DragDropEffects.Move;
		}
		protected override void OnPreviewDragEnter(DragEventArgs e)
		{
			base.OnPreviewDragEnter(e);

			e.Effects = DragDropEffects.Move;
		}
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);

			e.Effects = DragDropEffects.Move;
		}
		protected override void OnPreviewDragOver(DragEventArgs e)
		{
			base.OnPreviewDragOver(e);

			e.Effects = DragDropEffects.Move;
		}

		private void ChatAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateWindowTitle();
		}

		// Private Functions
		private UIElement GenerateTabButton(UIElement subItem)
		{
			Grid headerGrid = new Grid();
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });
			headerGrid.MinWidth = 100;

			Rectangle rect = new Rectangle();

			TextBlock mHeaderString = new TextBlock();
			mHeaderString.Padding = new Thickness(2, 0, 5, 0);
			mHeaderString.Text = subItem.ToString();
			Grid.SetColumn(mHeaderString, 1);

			Grid closeButtonGrid = new Grid();
			Grid.SetColumn(closeButtonGrid, 2);

			TextBlock closeButton = new TextBlock();
			closeButton.Text = "×";
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButton.Margin = new Thickness(0, -1.5, 0, 0);
			closeButton.UseLayoutRounding = false;
			closeButton.HorizontalAlignment = HorizontalAlignment.Right;
			closeButtonGrid.Children.Add(closeButton);

			headerGrid.Children.Add(mHeaderString);
			headerGrid.Children.Add(closeButtonGrid);

			return headerGrid;
		}
		private void UpdateWindowTitle()
		{
			/*Dispatcher.InvokeIfRequired(() => {
				string msg;

				if (mUnread == 0)
					msg = ChatAreas.SelectedItem.ToString();
				else
					msg = String.Format("[{0}] {1}", mUnread, ChatAreas.SelectedItem.ToString());

				Title = msg;
			});*/
		}
		private void UpdateGlass()
		{
			Aero.ExtendGlass(this, new Thickness(1, 31, 1, 0));
		}
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == Aero.WM_DWMCOMPOSITIONCHANGED)
			{
				UpdateGlass();
				handled = true;
			}
			return IntPtr.Zero;
		}

		private int mUnread;
		private int mTabCount;
	}
}