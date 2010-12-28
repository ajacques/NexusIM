<%@ Page Title="" Language="C#" MasterPageFile="~/w/config/centered.master" AutoEventWireup="true" CodeBehind="mylocationsetup.aspx.cs" Inherits="NexusWeb.Pages.MyLocationSetup" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<asp:Label ID="Label1" runat="Server">To retrieve your location from a 3rd-party location service, you will first need to set it up.</asp:Label><br />
	<h3 style="padding-bottom: 0px; margin-bottom: 5px">Location Provider:</h3>
	<span style="margin-left: 10px">
		<asp:RadioButton ID="googlelatitude" runat="server" Text="Google Latitude" oncheckedchanged="googlelatitude_CheckedChanged" AutoPostBack="True" />
	</span>
	<h3 style="color: #808080" id="lblsetup" runat="Server">Setup</h3>
	<asp:Panel id="GoogleLatitudeSetup" runat="Server">
		To use Google Latitude, you will first need to setup your account to allow 3rd-party location requests. To do this please follow these steps. If you already have done this, please go to step 2.
		<ol>
			<li>Go to this <a href="http://www.google.com/latitude/apps/badge" target="_blank">page</a>, click Enable, and click submit.</li>
			<li>Underneath this yellow box will be a box that says "Embed the Google Public Location Badge", Copy all the text in the box into the box below:</li>
			<br /><asp:TextBox id="glatitudeident" runat="Server" Height="55px" TextMode="MultiLine" Width="394px" />
		</ol>
		<input id="Submit" type="submit" value="Save" />
	</asp:Panel>
</asp:Content>
