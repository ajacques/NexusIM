using System;
using System.Web.UI;

namespace NexusWeb.SubPages
{
	public partial class DeleteAccount : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			accid.Value = Request["id"];
		}
	}
}