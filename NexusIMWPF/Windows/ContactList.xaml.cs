using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using InstantMessage.Events;
using NexusIM.Controls;
using NexusIM.Managers;
using NexusIM.Misc;

namespace NexusIM.Windows
{
	/// <summary>
	/// Contains the interaction logic for the Contact List Window.
	/// </summary>
	public partial class ContactListWindow : Window
	{
		public ContactListWindow()
		{
			InitializeComponent();

			AccountManager.PropertyChanged += new PropertyChangedEventHandler(AccountManager_PropertyChanged);
			AggregateContactList.Groups.CollectionChanged += new NotifyCollectionChangedEventHandler(ContactList_Changed);
		}

		public void InsertErrorBox(IMProtocolWrapper protocol, SocketErrorEventArgs e)
		{
			Dispatcher.InvokeIfRequired(() => {
				CLErrorBox box = new CLErrorBox();
				box.PopulateControls(protocol, e.Exception);

				Storyboard anim = new Storyboard();
				DoubleAnimation dblAnim = new DoubleAnimation();
				dblAnim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
				dblAnim.From = 0;
				dblAnim.To = 50;
				anim.Children.Add(dblAnim);

				Storyboard.SetTarget(anim, box);
				Storyboard.SetTargetProperty(anim, new PropertyPath("(FrameworkElement.Height)"));
				
				BottomFillPanel.Children.Add(box);
				//anim.Begin();
			});
		}

		private void DeselectAllExcept(ICollection source, UIElement exception)
		{
			foreach (UIElement elem in source)
			{
				if (elem is ContactListItem)
				{
					ContactListItem item = elem as ContactListItem;
					if (item.Selected && item != exception)
						item.Deselect();
				} else if (elem is ContactListGroup) {
					ContactListGroup group = elem as ContactListGroup;					
					if (group != exception)
						group.DeselectAllExcept(exception);
				}
				
			}
		}
		private void HandleStatusChange()
		{
			int selectedIndex = -1;
			switch (AccountManager.Status)
			{
				case IMStatus.Available:
					selectedIndex = 0;
					break;
				case IMStatus.Away:
					selectedIndex = 1;
					break;
				case IMStatus.Busy:
					selectedIndex = 2;
					break;
				case IMStatus.Invisible:
					selectedIndex = 3;
					break;
			}
			
			Dispatcher.BeginInvoke(new GenericEvent(() => StatusComboBox.SelectedIndex = selectedIndex));
		}
		private void AddGroups(IEnumerable source)
		{
			//IEnumerator contact = source.GetEnumerator();
			Dispatcher.InvokeIfRequired(() =>
			{
				foreach (GroupOfContacts contact in source)
				//while (contact.MoveNext())
				{
					ContactListGroup group = new ContactListGroup();
					group.SourceGroup = contact; //contact.Current as GroupOfContacts;
					group.IsExpanded = true;
					ContactList.Add(group);
				}
			}, false);
		}
		private void AddProtocolMenus()
		{
			ProtocolGroup.Children.Add(new IRCProtocolMenu());
			ProtocolGroup.Children.Add(new YahooProtocolMenu());
		}

		// Event Handlers
		private void AccountManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Status":
					if (mIgnoreThisStatusChange)
					{
						mIgnoreThisStatusChange = false;
						break;
					}
					HandleStatusChange();
					mIgnoreThisStatusChange = true;
					break;
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (!SuperTaskbarManager.IsSetup)
				ThreadPool.QueueUserWorkItem(new WaitCallback((object obj) => SuperTaskbarManager.Setup()));

			AddGroups(AggregateContactList.Groups);

			//if (!AccountManager.Accounts.Any(i => i.Enabled))
			//	NoEnabledAccountsWarning.Visibility = Visibility.Visible;

			try	{
				StopwatchManager.Stop("AppInit", "{0} - Time to contact list window loaded: {1}");
			} catch (KeyNotFoundException) { }

			AddProtocolMenus();
		}

		// User Interface Event Handlers
		private void EditAccounts_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.OpenSingletonWindow(typeof(AccountsEdit));
		}
		private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (mIgnoreThisStatusChange)
			{
				mIgnoreThisStatusChange = false;
				return;
			}

			int selectedIndex = StatusComboBox.SelectedIndex;
			
			ThreadPool.QueueUserWorkItem(new WaitCallback((object obj) =>
				{
					IMStatus status;
					switch (selectedIndex)
					{
						case 0:
							status = IMStatus.Available;
							break;
						case 1:
							status = IMStatus.Away;
							break;
						case 2:
							status = IMStatus.Busy;
							break;
						case 3:
							status = IMStatus.Invisible;
							break;
						case 5:
							AccountManager.Connected = false;
							return;
						default:
							status = IMStatus.Available;
							break;
					}
					mIgnoreThisStatusChange = true;
					AccountManager.Status = status;
				}), null);
		}
		private void ContactList_Changed(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				AddGroups(e.NewItems);
		}
		private void JoinRoom_Click(object sender, RoutedEventArgs e)
		{
			JoinChatRoom window = new JoinChatRoom();
			window.Owner = this;
			window.ShowDialog();
		}
		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow about = new AboutWindow();
			about.Owner = this;
			about.ShowDialog();
		}
		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			DisplayImageWindow window = new DisplayImageWindow();
			window.Owner = this;
			window.ShowDialog();
		}

		// Status Message Related Event handlers
		private void SetStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			Storyboard story = FindResource("ShareMessageOpen") as Storyboard;

			story.Begin();

			for (int i = StatusTargetSelector.Items.Count - 1; i >= 3; i--)
				StatusTargetSelector.Items.RemoveAt(i);

			int count = 0;
			foreach (IMProtocolWrapper protocol in AccountManager.Accounts/*.Where(a => a.Enabled && a.Protocol.ProtocolStatus == IMProtocolStatus.Online)*/)
			{
				CheckBox check = new CheckBox();
				TextBlock label = new TextBlock();
				
				label.Inlines.Add(new Run(protocol.Protocol.Username));

				check.Content = label;
				check.IsChecked = true;
				check.Checked += new RoutedEventHandler(ChangeStatusTargets_Click);
				check.Unchecked += new RoutedEventHandler(ChangeStatusTargets_Click);

				mStatusMsgAccountsYes = ++mStatusMsgMaxAccounts;

				StatusTargetSelector.Items.Add(check);
				StatusTargetSelector.Items.Add(new CheckBox() { Content = (++count).ToString() + " Account" + (count == 1 ? "" : "s"), Height = 0 });
			}
		}
		private void CancelStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			Storyboard story = FindResource("ShareMessageClose") as Storyboard;

			story.Begin();
		}
		private void ChangeStatusTargets_Click(object sender, RoutedEventArgs e)
		{
			CheckBox source = (CheckBox)sender;

			if (source.IsChecked.GetValueOrDefault())
				mStatusMsgAccountsYes++;
			else
				mStatusMsgAccountsYes--;

			if (mStatusMsgAccountsYes == mStatusMsgMaxAccounts)
				StatusTargetSelector.Text = "All Accounts";
			else if (mStatusMsgAccountsYes == 0)
				StatusTargetSelector.Text = "No Accounts";
			else
				StatusTargetSelector.Text = mStatusMsgAccountsYes.ToString() + " Accounts";
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Left = SystemParameters.PrimaryScreenWidth - (double)GetValue(WidthProperty);
			Top = SystemParameters.PrimaryScreenHeight / 2 - ((double)GetValue(HeightProperty) / 2);
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			HitTestResult result = VisualTreeHelper.HitTest(ContactListControl, e.GetPosition(ContactListControl));
			VisualTreeHelper.HitTest(ContactListControl, new HitTestFilterCallback(OnHitTestFilter), new HitTestResultCallback(OnHitTestResult), new PointHitTestParameters(e.GetPosition(ContactListControl)));
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			e.Cancel = true;

			this.Hide();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}
		private HitTestResultBehavior OnHitTestResult(HitTestResult result)
		{
			DeselectAllExcept(ContactList, (UIElement)result.VisualHit);
			if (result.VisualHit is ContactListItem)
				((ContactListItem)result.VisualHit).Select();

			return HitTestResultBehavior.Stop;
		}
		private HitTestFilterBehavior OnHitTestFilter(DependencyObject uiElement)
		{
			if (uiElement is ContactListGroup)
				return HitTestFilterBehavior.Continue;
			else if (uiElement is ContactListItem)
				return HitTestFilterBehavior.ContinueSkipChildren;
			else
				return HitTestFilterBehavior.ContinueSkipSelf;
		}	

		// Properties
		public UIElementCollection ContactList
		{
			get	{
				return ContactListControl.Children;
			}
		}

		// Variables
		private int mStatusMsgMaxAccounts;
		private int mStatusMsgAccountsYes;
		private bool mIgnoreThisStatusChange = true; // Ignore the first change
	}
}