<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newdevice.aspx.cs" Inherits="NexusWeb.SubPages.NewDevice" EnableViewState="false" %>

<form id="form1" runat="server">
<asp:scriptmanager runat="server">
	<Scripts>
		<asp:ScriptReference Path="../js/newdevice.js" />
	</Scripts>
	<Services>
		<asp:ServiceReference Path="../Devices.svc" />
	</Services>
</asp:scriptmanager>
	<asp:Label runat="Server" AssociatedControlID="devicename" Text="Name:" />&nbsp;<asp:TextBox runat="Server" ID="devicename" Text="Computer1" /><br />
	<span>Platform Type:</span><br />
	<asp:RadioButtonList runat="Server" ID="devicetype">
		<asp:ListItem runat="Server" ID="windows" Text="Windows" />
		<asp:ListItem runat="Server" ID="tester" Text="Test Appliance" />
		<asp:ListItem runat="Server" ID="mobile" Text="Smartphone" />
	</asp:RadioButtonList>
</form>
