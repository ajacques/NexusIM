<%@ Page Title="Location Service Configuration" Language="C#" MasterPageFile="../centered.master" AutoEventWireup="true" CodeBehind="locationconfig.aspx.cs" Inherits="NexusWeb.Pages.LocationConfig" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link rel="StyleSheet" href="../css/location.css" type="text/css" />
	<script type="text/javascript" src="../js/location.js"></script>
	<script type="text/javascript" src="../Services/Accounts.svc/jsdebug"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ErrorMessagePlaceHolder" runat="server">
	<div id="errormessage" class="errormessage" runat="Server" visible="false" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="rightcolcontent" runat="Server">
	<div class="colheadline">Location Requests</div>
		<span runat="Server" ID="requestpnl" />
	<div class="colheadline">Suggestions</div>
		<span runat="Server" ID="suggestionpnl" style="margin-left: 5px">
			<asp:BulletedList runat="server" ID="SuggestionList" BulletStyle="NotSet" />
		</span>
	<div class="colheadline">Tasks</div>
	<ul id="tasklist">
		<li><a href="javascript:LocationConfig.requestperm();">Request Permission</a></li>
		<li><a href="">Give Permission</a></li>
		<li><a href="javascript:LocationConfig.addcustom();">Add Custom User</a></li>
	</ul>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
<form id="Form1" runat="server">
	<asp:SiteMapPath ID="SiteMapPath1" runat="server" Font-Names="Verdana" 
	Font-Size="0.8em" PathSeparator=" : ">
		<PathSeparatorStyle Font-Bold="True" ForeColor="#5D7B9D" />
		<CurrentNodeStyle ForeColor="#333333" />
		<NodeStyle Font-Bold="True" ForeColor="#7C6F57" />
		<RootNodeStyle Font-Bold="True" ForeColor="#5D7B9D" />
</asp:SiteMapPath>
	<div id="leftcol">
		<asp:Panel ID="yesmyservices" runat="Server">
			You have successfully setup the following location service: <br />
			<asp:BulletedList runat="Server" ID="myservices">
				<asp:ListItem>Browser Location <a>Add Now</a></asp:ListItem>
			</asp:BulletedList>
		</asp:Panel>
		<asp:Panel ID="friendpermissions" runat="Server">
			You have permission to view the following people's locations:
			<asp:Table runat="Server" ID="friendrows" CssClass="headertable">
				<asp:TableHeaderRow runat="Server">
					<asp:TableHeaderCell CssClass="firstfriendcol">Name</asp:TableHeaderCell>
					<asp:TableHeaderCell style="width: 110px">Service</asp:TableHeaderCell>
					<asp:TableHeaderCell>Options</asp:TableHeaderCell>
				</asp:TableHeaderRow>
			</asp:Table>		
		</asp:Panel>
	</div>
	<div id="popup" class="popupouter" style="display: none">
		<div class="popupmid">
			<div class="popupinner">
				<div id="popuptitle" class="popuptitle">Title Here</div>
				<div id="popupcontent" class="popupcontent"></div>
				<div id ="popupbuttoncontainer">
					<span id="popupbuttons" class="popupbuttons">
						<input type="button" value="Accept" id="popupaccept" />
						<input type="button" value="Cancel" onclick="javascript:cancelPopup();" />
					</span>
				</div>
			</div>
		</div>
	</div>
</form>
</asp:Content>