using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace InstantMessage
{
	public class AccountsListView : ListView
	{
		public AccountsListView()
		{
			base.DrawItem += new DrawListViewItemEventHandler(DoDrawItem);
			base.OwnerDraw = true;
			base.View = View.Tile;
		}

		private Dictionary<ListViewItem, CheckBox> checks = new Dictionary<ListViewItem, CheckBox>();
		public Dictionary<ListViewItem, Image> statusImages = new Dictionary<ListViewItem, Image>();

		private void DoDrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawBackground();

			if ((e.State & ListViewItemStates.Focused) != 0)
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), e.Bounds);
			}

			if (statusImages.ContainsKey(e.Item))
			{
				Point point = new Point(e.Bounds.X, e.Bounds.Y);
				point.X = 20;

				e.Graphics.DrawImageUnscaled(statusImages[e.Item], point);
			}

			if (!checks.ContainsKey(e.Item))
			{
				CheckBox check = new CheckBox();
				check.CheckedChanged += new EventHandler(check_CheckedChanged);
				check.Parent = this;
				Rectangle r = e.Bounds;
				r.Width = 20;
				check.Bounds = r;
				checks.Add(e.Item, check);
			} else {
				Rectangle r = e.Bounds;
				if (r.Top >= 0 && r.Bottom < this.Height)
				{
					r.Width = 20;
					checks[e.Item].Bounds = r;
					checks[e.Item].Visible = true; // are these useful?
				} else {
					checks[e.Item].Bounds = r;
					checks[e.Item].Visible = false;
				}
			}
			checks[e.Item].Checked = e.Item.Checked;
			Point p = new Point(e.Bounds.X, e.Bounds.Y);
			p.X = 30;

			e.Graphics.DrawString(e.Item.Text, e.Item.Font, new SolidBrush(Color.Black), p);
		}
		public void updateChecks()
		{
			Dictionary<ListViewItem, CheckBox> updated = new Dictionary<ListViewItem, CheckBox>();
			foreach (ListViewItem item in Items)
			{
				if (checks.ContainsKey(item))
					updated.Add(item, checks[item]);
				else
					return;
			}
			List<ListViewItem> garbage = new List<ListViewItem>();

			foreach (var i in checks)
			{
				if (!updated.ContainsKey(i.Key))
				{
					garbage.Add(i.Key);
					i.Value.Dispose();
				}
			}


			foreach (ListViewItem item in garbage)
			{
				checks.Remove(item);
			}
		}
		public void updateImages()
		{
			Dictionary<ListViewItem, Image> updated = new Dictionary<ListViewItem, Image>();
			foreach (ListViewItem item in Items)
			{
				if (checks.ContainsKey(item))
					updated.Add(item, statusImages[item]);
				else
					return;
			}
			List<ListViewItem> garbage = new List<ListViewItem>();

			foreach (var i in checks)
			{
				if (!updated.ContainsKey(i.Key))
				{
					garbage.Add(i.Key);
					i.Value.Dispose();
				}
			}


			foreach (ListViewItem item in garbage)
			{
				statusImages.Remove(item);
			}
		}
		private void check_CheckedChanged(object sender, EventArgs e)
		{
			var key = (from k in checks where (k.Value == (CheckBox)sender) select k.Key).FirstOrDefault();

			key.Checked = ((CheckBox)sender).Checked;

			OnItemChecked(new ItemCheckedEventArgs(key));
		}
	}
}