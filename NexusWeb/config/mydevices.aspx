<%@ Page Title="My Devices" Language="C#" MasterPageFile="../centered.master" AutoEventWireup="true" CodeBehind="mydevices.aspx.cs" Inherits="NexusWeb.Pages.MyDevices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="rightcolcontent" runat="Server">
	<div class="colheadline">Tasks</div>
	<ul id="tasklist">
		<li><a href="javascript:MyDevices.addDevice();">Add Device</a></li>
	</ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="margin-left: auto; margin-right: auto">
		<span style="float: right; text-align: center">
			<a href="javascript:MyDevices.moveUp();">Up</a><br />
			<a href="javascript:MyDevices.moveDown();">Down</a>
		</span>
		<asp:Table runat="Server" ID="devicetable" CssClass="headertable">
			<asp:TableHeaderRow>
				<asp:TableHeaderCell style="width: 200px">Name</asp:TableHeaderCell>
				<asp:TableHeaderCell>Type</asp:TableHeaderCell>
			</asp:TableHeaderRow>
		</asp:Table>
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
	<script type="text/javascript">
		MyDevices.setupPage();
	</script>
</asp:Content>