<%@ Page Title="NexusIM - My Account" Language="C#" MasterPageFile="../centered.master" AutoEventWireup="true" CodeBehind="myaccount.aspx.cs" Inherits="NexusWeb.Pages.MyAccount" %>
<asp:Content ContentPlaceHolderID="rightcolcontent" runat="Server">
	<div class="colheadline">Requests</div>
		<span runat="Server" ID="Span1" />
	<div class="colheadline">Tasks</div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="width: 400px;">
		<asp:Hyperlink runat="server" NavigateUrl="~/w/main.aspx" Text="Buddy List" />
		<!-- My Accounts -->
		<div class="accountsectiontitle">My Accounts
			<span class="accountsectiontitlelink">
			<asp:DropDownList ID="accountState" runat="server" onchange="MyAccount.statusChange();" ClientIDMode="Static" ViewStateMode="Disabled">
				<asp:ListItem Text="Available" />
				<asp:ListItem Text="Busy" />
				<asp:ListItem Text="Away" />
				<asp:ListItem Text="Invisible" />
			</asp:DropDownList>
			[<a id="A2" runat="Server" href="~/w/config/myaccounts.aspx">options</a>]
			</span>
		</div>
		<div class="accountsectioncontent">
			<div runat="Server" id="accountstatusgood" style="color: green">Accounts have been setup</div>
			<div runat="Server" id="accountstatusnone" style="color: red">No accounts have been setup for your account.</div>
		</div>
		<!-- My Devices -->
		<div class="accountsectiontitle">My Devices<span class="accountsectiontitlelink">[<a id="A1" runat="Server" href="~/w/config/mydevices.aspx">options</a>]</span></div>
		<div class="accountsectioncontent">
			<div runat="Server" id="devicesstatusready" style="color: green">Devices are properly configured and ready to use.</div>
			<div runat="Server" id="devicesstatusnodevices" style="color: red">No devices have been added to your account.</div>
			<asp:Label runat="server" ID="deviceonlinecount">0</asp:Label>&nbsp;(<asp:Label runat="server" ID="devicetotalcount">5</asp:Label>) devices connected.
			<asp:BulletedList ID="onlinedevicesample" runat="server" BulletStyle="Square" style="margin-top: 5px; padding-left: 25px; margin-bottom: 5px"></asp:BulletedList>
		</div>
		<!-- Location Sharing -->
		<div class="accountsectiontitle">Location Sharing<span class="accountsectiontitlelink">[<a runat="Server" href="~/w/config/locationconfig.aspx">options</a>]</span></div>
		<div class="accountsectioncontent">
			<a runat="Server" id="locationtogglehref" href="javascript:MyAccount.toggleLocationState();">Enable/Disable</a> | <a href="../locationmap.aspx">View Map</a>
			<asp:HiddenField runat="Server" ID="locationstate" EnableViewState="False" ClientIDMode="Static" />
			<div runat="Server" id="locationstatuson" style="color: green">Location sharing is currently enabled.</div>
			<div runat="Server" id="locationstatusoff" style="color: red">Location sharing is currently disabled.</div>
		</div>
	</div>
</asp:Content>