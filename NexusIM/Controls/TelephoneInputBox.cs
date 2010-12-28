using System.ComponentModel;
using System.Windows.Forms;

namespace NexusIM.Controls
{
	public class TelephoneInputBox : MaskedTextBox
	{
		public TelephoneInputBox()
		{
			base.Mask = "+## (###) ###-####";
		}

		public TelephoneInputBox(IContainer container)
		{
			container.Add(this);
		}
	}
}
