<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editaccount.aspx.cs" Inherits="NexusWeb.SubPages.EditAccount" EnableViewState="false" %>

<form id="form1" runat="server">
<asp:scriptmanager runat="server">
	<Services>
		<asp:ServiceReference Path="../Accounts.svc" />
	</Services>
</asp:scriptmanager>
<table>
	<td><asp:Label runat="Server" AssociatedControlId="protocol" Text="Protocol: " /></td><td><asp:Label runat="Server" ID="protocol" /></td></tr>
	<tr><td><asp:Label runat="Server" AssociatedControlId="username" Text="Username: " /></td><td><asp:TextBox runat="Server" ID="username" /></td></tr>
	<tr><td><asp:Label runat="Server" AssociatedControlId="password" Text="Password: " /></td><td><asp:TextBox runat="Server" ID="password" TextMode="Password" /></td></tr>
</table>
<asp:HiddenField runat="Server" ID="accountid" />
<script type="text/javascript">
function save()
{
	var service = new AccountService();
	var accid = $("input[id^=accountid]").val();
	var username = $("input[id^=username]").val();
	service.EditAccount(accid, username, $("input[id^=password]").val(), function()
	{
		cancelPopup();
		popupAccountEdit(accid, username);
	});
}

$("input#popupaccept").val("Save").bind("click", save);
</script>
</form>
