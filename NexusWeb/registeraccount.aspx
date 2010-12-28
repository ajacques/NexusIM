<%@ Page Title="Register Account" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="registeraccount.aspx.cs" Inherits="NexusWeb.Pages.RegisterAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link rel="StyleSheet" href="css/registeraccount.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
	<h6>Before you can sign-in, you'll first need to give us some information about yourself.</h6>
	<h5 style="border-bottom: solid 1px #000000; max-width: 300px">Account Information</h5>
	<table style="margin-left: 50px">
		<tr>
			<td style="padding-right: 10px"><asp:Label runat="Server" AssociatedControlID="email">Email</asp:Label></td>
			<td><asp:TextBox runat="Server" ID="email" onblur="validateUsername();" ToolTip="You will use this to login into your account." /></td>
			<td><asp:Image runat="Server" ID="usernameValidationResult" CssClass="hidden" /></td>
		</tr>
		<tr>
			<td><asp:Label runat="Server" AssociatedControlID="password">Password</asp:Label></td>
			<td><asp:TextBox runat="Server" ID="password" /></td>
		</tr>
	</table>
	<div id="recaptchahost"></div>
	<script type="text/javascript">
	Recaptcha.create("6LdgTAwAAAAAACmYjmTcFFohp3HaMKGEvfJkF_xZ",
	"recaptchahost",
	{
		theme: "clean"
	});
	</script>
	<input runat="Server" id="submit" type="submit" value="Register" onclick="return validateAll();" />
</asp:Content>