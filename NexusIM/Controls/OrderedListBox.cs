using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NexusIM.Controls
{
	public class OrderedListBox : ListBox
	{
		public OrderedListBox()
		{
			base.DrawMode = DrawMode.OwnerDrawFixed;
			//base.DrawItem += new DrawItemEventHandler(DrawItem);
		}

		public OrderedListBox(IContainer container)
		{
			container.Add(this);
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();
			e.Graphics.DrawString((e.Index + 1).ToString(), base.Font, new SolidBrush(Color.DarkGray), new Point(5, e.Bounds.Top));
			e.Graphics.DrawString(base.Items[e.Index].ToString(), base.Font, new SolidBrush(Color.Black), new Point(20, e.Bounds.Top));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			foreach (string item in base.Items)
			{
				e.Graphics.DrawString(item, base.Font, new SolidBrush(Color.Black), new Point(10, 10));
			}
		}
	}
}
