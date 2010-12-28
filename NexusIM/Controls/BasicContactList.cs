using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using InstantMessage;

namespace NexusIM.Controls
{
	public class BasicContactList : ListView
	{
		private Dictionary<ListViewItem, IMBuddyStatus> onlinestatus = new Dictionary<ListViewItem, IMBuddyStatus>();
		private Dictionary<IMBuddy, ListViewItem> buddies = new Dictionary<IMBuddy, ListViewItem>();
		private Dictionary<IMBuddyStatus, Image> statusIcons = new Dictionary<IMBuddyStatus, Image>();

		public BasicContactList()
		{
			base.DrawItem += new DrawListViewItemEventHandler(BuddyList_DrawItem);
			base.ListViewItemSorter = new BuddyListSorter();
			base.View = View.Details;
			base.OwnerDraw = true;

			IMBuddy.onGlobalBuddyStatusChange += new EventHandler(IMBuddy_onGlobalBuddyStatusChange);

			base.Columns.Add("Name", 250);
			base.HeaderStyle = ColumnHeaderStyle.None;

			Assembly thisExe = Assembly.GetExecutingAssembly();
			statusIcons.Add(IMBuddyStatus.Available, Image.FromStream(thisExe.GetManifestResourceStream("NexusIM.Resources.online_orb.png")));
			statusIcons.Add(IMBuddyStatus.Offline, Image.FromStream(thisExe.GetManifestResourceStream("NexusIM.Resources.offline_orb.png")));
			statusIcons.Add(IMBuddyStatus.Busy, Image.FromStream(thisExe.GetManifestResourceStream("NexusIM.Resources.busy_orb.png")));
			statusIcons.Add(IMBuddyStatus.Idle, Image.FromStream(thisExe.GetManifestResourceStream("NexusIM.Resources.idle_orb.png")));
			statusIcons.Add(IMBuddyStatus.Away, Image.FromStream(thisExe.GetManifestResourceStream("NexusIM.Resources.away_orb.png")));
		}

		public void AddBuddy(IMBuddy buddy)
		{
			ListViewItem item = new ListViewItem(buddy.DisplayName);
			Items.Add(item);
			buddies.Add(buddy, item);
		}
		private void BuddyList_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			DrawItemSmallStyle(e);
		}
		private void DrawItemSmallStyle(DrawListViewItemEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.DrawBackground();
			Size textsize = TextRenderer.MeasureText(e.Item.Text, e.Item.Font);

			if (((e.State & ListViewItemStates.Selected) != 0))
			{
				SolidBrush selectedbrush = new SolidBrush(Color.Brown);
				e.Graphics.FillRectangle(selectedbrush, e.Bounds);
			}

			Point sp = new Point(e.Bounds.X, e.Bounds.Y);
			sp.X += 26;

			e.Graphics.DrawString(e.Item.Text, e.Item.Font, new SolidBrush(Color.Black), sp);

			Point xp = new Point(e.Bounds.X, e.Bounds.Y);
			xp.Y += 4;
			xp.X += 17;
			e.Graphics.DrawImage(statusIcons[getBuddyFromLViewItem(e.Item).Status], new Rectangle(xp, new Size(8, 8)));
		}
		private void SortGroups()
		{
			if (Groups["Offline"] != null && Groups[0] != Groups["Offline"])
			{
				ListViewGroup group = null;
				group = Groups["Offline"];
				Groups.Remove(group);
				Groups.Insert(0, group);
			}
		}
		private void IMBuddy_onGlobalBuddyStatusChange(object sender, EventArgs e)
		{
			base.Invalidate();
		}

		private IMBuddy getBuddyFromLViewItem(ListViewItem lv)
		{
			return buddies.First(pt => pt.Value == lv).Key;
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case 0x115:
					{
						Message b = new Message();
						b.Msg = 0x00F;
						base.WndProc(ref b);
						b.Msg = 0x0014;
						base.WndProc(ref b);
						break;
					}
			}
			base.WndProc(ref m);
		}
	}
}