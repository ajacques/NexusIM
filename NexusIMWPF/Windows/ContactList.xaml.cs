using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Managers;
using System.Windows.Media.Animation;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for frmMain.xaml
	/// </summary>
	public partial class ContactListWindow : Window
	{
		public ContactListWindow()
		{
			InitializeComponent();

			AccountManager.PropertyChanged += new PropertyChangedEventHandler(AccountManager_PropertyChanged);
			AggregateContactList.Groups.CollectionChanged += new NotifyCollectionChangedEventHandler(ContactList_Changed);
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
			foreach (GroupOfContacts contact in source)
			{
				Dispatcher.BeginInvoke(new GenericEvent(() =>
				{
					ContactListGroup group = new ContactListGroup();
					group.SourceGroup = contact;
					group.IsExpanded = true;
					ContactList.Add(group);
				}));
			}
		}

		// Event Handlers
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
		}
		private void EditAccounts_Click(object sender, RoutedEventArgs e)
		{
			AccountsEdit window = new AccountsEdit();
			window.Show();
		}
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
			window.Show();

			mActiveDialog = window;
		}
		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow about = new AboutWindow();
			about.Owner = this;
			about.Show();
		}
		private void SetStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			Storyboard story = FindResource("ShareMessageOpen") as Storyboard;

			story.Begin();
		}
		private void CancelStatusMessage_Click(object sender, RoutedEventArgs e)
		{
			Storyboard story = FindResource("ShareMessageClose") as Storyboard;

			story.Begin();
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
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (mActiveDialog != null)
				mActiveDialog.Activate();
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
		private bool mIgnoreThisStatusChange = true; // Ignore the first change
		private Window mActiveDialog;
	}
}