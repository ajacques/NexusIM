<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="deleteaccount.aspx.cs" Inherits="NexusWeb.SubPages.DeleteAccount" EnableViewState="false" %>

<form id="form1" runat="server">
<asp:scriptmanager runat="server">
	<Services>
		<asp:ServiceReference Path="../Accounts.svc" />
	</Services>
</asp:scriptmanager>
<asp:HiddenField runat="Server" ID="accid" />
Are you sure you want to delete this account? It will be immediately removed from all accounts. This action is irreversible.
<script type="text/javascript">
function remove()
{
	var service = new AccountService();
	var accid = $("input#accid").val();
	service.DeleteAccount(accid);
	cancelPopup();
}

$("input#popupaccept").val("Remove").bind("click", remove);
$("input#popupcancel").val("Cancel");
</script>
</form>
