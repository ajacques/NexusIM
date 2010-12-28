using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using InstantMessage;
using NexusIM.Properties;

namespace NexusIM.Controls
{
	class BuddyListSorter : IComparer
	{
		public int Compare(object x, object y)
		{
			return iCompare.Compare(((ListViewItem)x).Text, ((ListViewItem)y).Text);
		}
		CaseInsensitiveComparer iCompare = new CaseInsensitiveComparer();
	}
	class ItemExtra
	{
		public ItemExtra(object extra)
		{
			mExtra = extra;
			mOptions = new Dictionary<string, object>();
		}
		public Dictionary<string, object> Options
		{
			get {
				return mOptions;
			}
		}
		public object Extra
		{
			get {
				return mExtra;
			}
		}
		public string TypeName
		{
			get {
				return mExtra.GetType().Name;
			}
		}
		private Dictionary<string, object> mOptions;
		private object mExtra;
	}
	class BuddyItem
	{
		public BuddyItem(IMBuddy buddy)
		{
			mBuddy = buddy;
			mItem = new ListViewItem();
			mExtras = new List<ItemExtra>();
		}
		public string StatusMessage
		{
			get {
				return mBuddy.StatusMessage;
			}
		}
		public ListViewItem ListViewItem
		{
			get {
				return mItem;
			}
		}
		public List<ItemExtra> Extras
		{
			get {
				return mExtras;
			}
		}
		public IMBuddyStatus Status
		{
			get {
				return mBuddy.Status;
			}
		}
		public IMBuddy Buddy
		{
			get {
				return mBuddy;
			}
		}
		public string DisplayName
		{
			get {
				return mBuddy.DisplayName;
			}
		}
		public Image Avatar
		{
			get {
				return mAvatar;
			}
			set {
				mAvatar = value;
			}
		}
		public Size NameTextSize
		{
			get {
				return mNameSize;
			}
		}
		private List<ItemExtra> mExtras;
		private Image mAvatar;
		private IMBuddy mBuddy;
		private ListViewItem mItem;
		private Size mNameSize;
	}
	class BuddyList : ListView
	{
		private ImageList imagelist = new ImageList();
		private Dictionary<ListViewItem, List<object>> itemExtras = new Dictionary<ListViewItem, List<object>>();
		private Dictionary<object, Dictionary<string, object>> extraOptions = new Dictionary<object, Dictionary<string, object>>();
		private Dictionary<ListViewItem, Image> avatars = new Dictionary<ListViewItem, Image>();
		private Dictionary<ListViewItem, IMBuddyStatus> onlinestatus = new Dictionary<ListViewItem, IMBuddyStatus>();
		private Dictionary<ListViewItem, string> statusMsgs = new Dictionary<ListViewItem, string>();
		private List<BuddyItem> mItems;
		private Brush mSelectionBrush;
		private Brush mNameBrush;
		private List<Image> mUserImageInsertQueue = new List<Image>();
		private Dictionary<IMBuddyStatus, Image> mStatusImages;

		public BuddyList()
		{
			//base.ItemMouseHover += new ListViewItemMouseHoverEventHandler(BuddyList_ItemMouseHover);
			//base.MouseMove += new MouseEventHandler(BuddyList_MouseMove);
			base.DrawItem += new DrawListViewItemEventHandler(BuddyList_DrawItem);
			base.MouseDown += new MouseEventHandler(Self_MouseDown);
			base.MouseUp += new MouseEventHandler(Self_MouseUp);
			base.ListViewItemSorter = new BuddyListSorter();
			base.Margin = new Padding(0, 0, 0, 5);
			base.OwnerDraw = true;

			base.Columns.Add("Contact", 120);
			base.Columns.Add("Status", 40);
			base.Columns.Add("Protocol", 40);
			base.AllowColumnReorder = false;
			base.HeaderStyle = ColumnHeaderStyle.None;

			imagelist.ImageSize = new Size(1, 16);
			base.SmallImageList = imagelist;

			mItems = new List<BuddyItem>();
			mSelectionBrush = new SolidBrush(SystemColors.Highlight);
			mNameBrush = new SolidBrush(SystemColors.ControlText);

			mStatusImages = new Dictionary<IMBuddyStatus, Image>();

			// Use pre-loaded resources
			mStatusImages.Add(IMBuddyStatus.Available, Resources.Online);
			mStatusImages.Add(IMBuddyStatus.Idle, Resources.Idle);
			mStatusImages.Add(IMBuddyStatus.Busy, Resources.Busy);
			mStatusImages.Add(IMBuddyStatus.Away, Resources.Away);
			mStatusImages.Add(IMBuddyStatus.Offline, Resources.Offline);
		}

		public void AddContact(BuddyItem item)
		{
			lock (mItems)
			{
				mItems.Add(item);

				if (InvokeRequired)
					Invoke(new MethodInvoker(delegate()
					{
						Items.Add(item.ListViewItem);
					}));
				else
					base.Items.Add(item.ListViewItem);

				item.ListViewItem.Tag = item.Buddy;

				base.Invalidate();
			}
		}
		public void RemoveContact(BuddyItem item)
		{
			lock (mItems)
			{
				mItems.Remove(item);
				base.Items.Remove(item.ListViewItem);
				base.Refresh();
			}
		}

		private void BuddyList_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			if (mIsCompact)
				DrawItemSmallStyle(e);
			else
				DrawItemLargeStyle(e);
		}
		private void DrawItemSmallStyle(DrawListViewItemEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.DrawBackground();

			var item = (from i in mItems where i.ListViewItem == e.Item select i).FirstOrDefault();

			Size textsize = TextRenderer.MeasureText(item.DisplayName, e.Item.Font);

			if (((e.State & ListViewItemStates.Selected) != 0))
			{
				e.Graphics.FillRectangle(mSelectionBrush, e.Bounds);
			}

			if (item.Avatar != null)
			{
				Point p = new Point(e.Bounds.X, e.Bounds.Y);
				e.Graphics.DrawImage(item.Avatar, new Rectangle(p, new Size(16, 16)));
			}

			Point sp = new Point(e.Bounds.X, e.Bounds.Y);
			sp.X += 26;

			if (!String.IsNullOrEmpty(item.StatusMessage))
			{
				Point smp = sp;
				smp.X += textsize.Width + 16;
				e.Graphics.DrawString(item.StatusMessage, e.Item.Font, new SolidBrush(Color.Black), smp);
			}

			e.Graphics.DrawString(item.DisplayName, e.Item.Font, new SolidBrush(Color.Black), sp);

			if (onlinestatus.ContainsKey(e.Item))
			{
				Point xp = new Point(e.Bounds.X, e.Bounds.Y);
				xp.Y += 4;
				xp.X += 17;
				e.Graphics.DrawImage(mStatusImages[item.Status], new Rectangle(xp, new Size(8, 8)));
			}

			RenderExtras(sp, e);
		}
		private void DrawItemLargeStyle(DrawListViewItemEventArgs e)
		{
			e.DrawBackground();

			var item = (from i in mItems where i.ListViewItem == e.Item select i).FirstOrDefault();

			Size textsize = TextRenderer.MeasureText(item.DisplayName, e.Item.Font);

			// Draw the selection box if needed
			if (((e.State & ListViewItemStates.Selected) != 0))
			{
				e.Graphics.FillRectangle(mSelectionBrush, e.Bounds);
			}

			if (item.Avatar != null)
			{
				Point p = new Point(e.Bounds.X, e.Bounds.Y);
				e.Graphics.DrawImage(item.Avatar, new Rectangle(p, new Size(32, 32)));
			}
			Point sp = new Point(e.Bounds.X, e.Bounds.Y);
			sp.X += 42;

			if (!String.IsNullOrEmpty(item.StatusMessage))
			{
				Point smp = sp;
				smp.Y += textsize.Height + 4;
				smp.X += 5;
				
				e.Graphics.DrawString(item.StatusMessage, e.Item.Font, new SolidBrush(Color.Black), smp);
			}

			Point xp = new Point(e.Bounds.X, e.Bounds.Y);
			xp.Y += 4;
			xp.X += 33;
			e.Graphics.DrawImage(mStatusImages[item.Status], new Rectangle(xp, new Size(10, 10)));

			e.Graphics.DrawString(item.DisplayName, e.Item.Font, mNameBrush, sp);

			RenderExtras(sp, e);
		}
		private void RenderExtras(Point sp, DrawListViewItemEventArgs e)
		{
			Size textsize = TextRenderer.MeasureText(e.Item.Text, e.Item.Font);

			if (itemExtras.ContainsKey(e.Item))
			{
				foreach (object obj in itemExtras[e.Item])
				{
					string curview = "";
					if (mIsCompact)
						curview = "compact";
					else
						curview = "large";
					if (extraOptions.ContainsKey(obj) && (!extraOptions[obj].ContainsKey("useview") || (string)extraOptions[obj]["onlyview"] == curview))
					{
						if (obj.GetType().BaseType.Name == "Image")
						{
							Point g = (Point)extraOptions[obj]["point"];
							if (extraOptions[obj].ContainsKey("isrelative") && ((bool)extraOptions[obj]["isrelative"]) == true)
							{
								Point f = new Point(g.X + (textsize.Width + sp.X), e.Bounds.Y + g.Y);
								if (extraOptions[obj].ContainsKey("size"))
								{
									e.Graphics.DrawImageUnscaled((Image)obj, new Rectangle(f, (Size)extraOptions[obj]["size"]));
								} else {
									e.Graphics.DrawImageUnscaled((Image)obj, f);
								}
							} else {
								g.Y += e.Bounds.Y;
								e.Graphics.DrawImageUnscaled((Image)obj, g);
							}
						} else if (obj.GetType().Name == "Button") {
							((Button)obj).Bounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 10, e.Bounds.Width + 20, e.Bounds.Height - 20);
						}
					}
				}
			}
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

		private bool mIsCompact = true;

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
				default:
					base.WndProc(ref m);
					break;
			}
		}
		protected void Self_MouseDown(object sender, MouseEventArgs e)
		{
			int position = e.Y / imagelist.ImageSize.Height;

			if (position < Items.Count)
			{
				ListViewItem item = Items[position];
				item.Selected = true;
			}
		}
		protected void Self_MouseUp(object sender, MouseEventArgs e)
		{
			int position = e.Y / imagelist.ImageSize.Height;

			if (position < Items.Count)
			{
				ListViewItem item = Items[position];
				item.Selected = true;
			}
		}

		[DefaultValue(true)]
		public bool CompactView
		{
			get {
				return mIsCompact;
			}
			set {
				this.SuspendLayout();
				if (value)
					imagelist.ImageSize = new Size(1, 16);
				else
					imagelist.ImageSize = new Size(1, 32);
				this.ResumeLayout();
				this.PerformLayout();
				mIsCompact = value;
				this.Invalidate();
			}
		}
	}
}