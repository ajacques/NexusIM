<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="locationmap.aspx.cs" Inherits="NexusWeb.Pages.LocationMap" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head id="Head1" runat="server">
    <title>Contact Location Map</title>
</head>
<body style="margin: 0; height: 100%">
	<form id="form1" runat="server" style="height: 100%">
	<asp:ScriptManager runat="Server" ID="scriptmgr" EnablePartialRendering="true">
		<Scripts>
			<asp:ScriptReference Path="~/js/Silverlight.supportedUserAgent.js" />
			<asp:ScriptReference Path="~/js/Silverlight.js" />
			<asp:ScriptReference Path="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.js" />
			<asp:ScriptReference Name="MicrosoftAjax.js" Path="~/js/scripts/MicrosoftAjax.js" />
		</Scripts>
	</asp:ScriptManager>
	<div id="slErrorNotInstalled" style="display: none">You need to install silverlight.</div>
	<div id="silverlightHost" style="height: 100%">
		<script type="text/javascript">
			function logger(data)
			{
				console.log(data);
			}
			if (!Silverlight.isInstalled("3"))
			{
				$("#slErrorNotInstalled").show();
			}
			Silverlight.createObject(
				"../ClientBin/SilverlightContactMap.xap",
				document.getElementById('silverlightHost'),
				"slPlugin",
				{
					width: "100%",
					height: "100%",
					background: "white",
					version: "3.0.4624.0"
				}
			);
		</script>
	</div>
	</form>
</body>
</html>