<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newaccount.aspx.cs" Inherits="NexusWeb.SubPages.NewAccount" EnableViewState="false" %>

<form id="form1" runat="server">
<asp:scriptmanager runat="server">
	<Scripts>
		<asp:ScriptReference Path="../js/newaccount.js" />
	</Scripts>
	<Services>
		<asp:ServiceReference Path="../Accounts.svc" />
		<asp:ServiceReference Path="../SocialNetworks.svc" />
	</Services>
</asp:scriptmanager>
<div runat="Server" id="page1">
	<span>Please choose an account type:</span><br />
	<asp:RadioButtonList runat="Server" id="networktype">
		<asp:ListItem Value="yahoo" Text="Yahoo! IM" />
		<asp:ListItem Value="aim" Text="AIM" />
		<asp:ListItem Value="msn" Text="Windows Live" />
		<asp:ListItem Value="facebook" Text="Facebook" />
	</asp:RadioButtonList>
</div>
<div id="facebook" style="display: none">
	<asp:HyperLink runat="server" ImageUrl="../images/facebookconnect.png" NavigateUrl="javascript:NewAccount.facebookConnect();" />
</div>
<input type="hidden" id="state" value="page1" />
<script type="text/javascript">
	NewAccount.setup();
</script>
</form>