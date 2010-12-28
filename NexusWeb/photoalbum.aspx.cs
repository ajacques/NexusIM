using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NexusWeb.Pages
{
	public partial class PhotoAlbum : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.GetCurrent(this).Services.Add(new ServiceReference("http://core.nexus-im.com/Services/CoreService.svc"));
		}
	}
}