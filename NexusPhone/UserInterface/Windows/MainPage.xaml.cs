using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.Windows.Media;

namespace NexusPhone.UserInterface
{
	partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();

			mThis = this;

			mUserSettings = IsolatedStorageSettings.ApplicationSettings;
			SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;
			mContactContextTimer = new DispatcherTimer();
			mContactContextTimer.Interval = new TimeSpan(0, 0, 1);
			mContactContextTimer.Tick += new EventHandler(mContactContextTimer_Tick);

			AccountManager.onNewContact += new EventHandler<NewContactEventArgs>(AccountManager_onNewContact);
			AccountManager.LoginCompleted += new GenericEvent(AccountManager_LoginCompleted);
		}

		// Event Callbacks
		private void AccountsMenuItem_Click(object sender, EventArgs e)
		{
			contactList.AnimateTiles(EnterMode.Enter, YDirection.TopToBottom, ZDirection.BackToFront);
			//WindowSystem.OpenAccountsWindow();
		}
		private void ContactListItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			pushDownItem = sender as ContactListItem;
			mMouseDown = DateTime.Now;
			/*foreach (Timeline timeline in TiltEffectAnim.Children)
				Storyboard.SetTarget(timeline, pushDownItem);
			TiltEffectAnim.Begin();*/
		}
		private void ContactListItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			mContactContextTimer.Stop();
			/*foreach (Timeline timeline in TiltEffectAnimBack.Children)
				Storyboard.SetTarget(timeline, pushDownItem);
			TiltEffectAnimBack.Begin();*/
			contactList.AnimateTiles(EnterMode.Exit, YDirection.BottomToTop, ZDirection.FrontToBack);
			pushDownItem.Contact.ShowWindow();
		}
		private void mContactContextTimer_Tick(object sender, EventArgs e)
		{
			mContactContextTimer.Stop();
			ContextMenuOpen.Begin();
		}
		private void PhoneApplicationPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (ContactContext.Visibility == System.Windows.Visibility.Collapsed)
				return;

			Point p = e.GetPosition(ContactContext);

			if (p.X < 0 || p.Y < 0 || p.X > ContactContext.Width || p.Y > ContactContext.Height)
			{
				// Reset To Default
				ContactContext.Visibility = System.Windows.Visibility.Collapsed;
				ContactContext.Width = 1;
				ContactContext.Height = 1;				
			}
		}
		private void AccountManager_onNewContact(object sender, NewContactEventArgs e)
		{
			contactList.Dispatch(() => {
				ContactListItem lItem = new ContactListItem(e.Buddy);
				lItem.MouseLeftButtonDown += new MouseButtonEventHandler(ContactListItem_MouseLeftButtonDown);
				lItem.MouseLeftButtonUp += new MouseButtonEventHandler(ContactListItem_MouseLeftButtonUp);
				lItem.Projection = new PlaneProjection();
				contactList.Items.Add(lItem);

			});
		}
		private void AccountManager_LoginCompleted()
		{
			ProgressBar.Visibility = Visibility.Collapsed;
		}

		// Properties
		public static MainPage Instance
		{
			get {
				return mThis;
			}
		}

		// Variables
		private DispatcherTimer mContactContextTimer;
		private ContactListItem pushDownItem;
		private DateTime mMouseDown;
		private static MainPage mThis;
		private IsolatedStorageSettings mUserSettings;		
	}
}