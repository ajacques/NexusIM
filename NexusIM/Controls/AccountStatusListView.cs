using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace InstantMessage
{
	public class AccountStatusListView : ListView
	{
		private Button btnConnect = new Button();
		private ListViewItem selected = null;

		private ImageList list = new ImageList();
		public AccountStatusListView()
		{
			base.DrawItem += new DrawListViewItemEventHandler(AccountStatusListView_DrawItem);
			base.Paint += new PaintEventHandler(AccountStatusListView_Paint);
			base.MouseMove += new MouseEventHandler(AccountStatusListView_MouseMove);
			base.OwnerDraw = true;
			base.View = View.Tile;
			base.SmallImageList = list;

			Items.Add("test");
			Items.Add("test2");

			list.ImageSize = new Size(1, 15);
			
			Image bigimg = new Bitmap(1, 40);
			Image smallimg = new Bitmap(1, 5);

			list.Images.Add(bigimg);
			list.Images.Add(smallimg);

			btnConnect.Parent = this;
			btnConnect.Visible = false;
		}

		void AccountStatusListView_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = base.GetItemAt(e.X, e.Y);
			if (item != null)
			{
				item.Tag = "s";
			}

			var items = from ListViewItem s in base.Items where s != item select new { s };
			foreach (var itemg in items)
			{
				itemg.s.Tag = "";
				RedrawItems(0, Items.Count - 1, true);
			}
			RedrawItems(0, Items.Count - 1, true);
			base.Invalidate();
		}

		void AccountStatusListView_Paint(object sender, PaintEventArgs e)
		{
			RedrawItems(0, Items.Count - 1, true);
			e.Graphics.Clear(Color.White);
		}

		void AccountStatusListView_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawBackground();
			Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

			if (e.Item.Tag == "s")
			{
				//e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), e.Bounds);
				Brush gradientbrush = new LinearGradientBrush(rect, Color.FromArgb(204, 217, 234), Color.FromArgb(241, 245, 251), LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(gradientbrush, rect);
			} else {
				e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
			}

			if ((e.State & ListViewItemStates.Selected) != 0)
			{
				selected = e.Item;
				Brush gradientbrush = new LinearGradientBrush(rect, Color.FromArgb(204, 217, 234), Color.White, LinearGradientMode.ForwardDiagonal);
				rect.Height = rect.Height + 20;
				e.Graphics.FillRectangle(gradientbrush, rect);

				rect.Y += 20;
				rect.X += 100;
				rect.Height = 20;
				rect.Width = 30;

				btnConnect.Bounds = rect;
				btnConnect.Visible = true;
			} else {
				if (selected == e.Item)
					btnConnect.Visible = false;
			}

			
			//Brush gradientbrush = new LinearGradientBrush(rect, Color.FromArgb(204, 217, 234), Color.FromArgb(241, 245, 251), LinearGradientMode.Horizontal);
			//e.Graphics.FillRectangle(gradientbrush, rect);
			e.DrawText();
		}

	}
}
