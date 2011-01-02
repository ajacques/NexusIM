<%@ Page Title="My IM Accounts" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="myaccounts.aspx.cs" Inherits="NexusWeb.Pages.MyNetworkAccounts" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.headertable tr:hover
		{
			background-color: #ccccee;
		}
	</style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="rightcolcontent" runat="Server">
	<div class="colheadline">Tasks</div>
	<ul id="tasklist">
		<li><a href='javascript:openpopup("Add Account", "popups/newaccount.aspx");'>Add Account</a></li>
	</ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div>
		<div style="font-size: 80%">Here you can edit the accounts you have added to your account. Change the order of your accounts to change their priority. Messages will be received on the highest possible priority account.</div>
		<div style="margin-left: auto; margin-right: auto; width: 302px">
			<asp:Button runat="Server" ID="save" Text="Save" OnClientClick="javascript:MyAccounts.saveChanges();return false;" />&nbsp;
			<asp:Button runat="Server" ID="cancel"  Text="Cancel" OnClientClick="javascript:MyAccounts.revertChanges();return false;" />
			<asp:Image runat="server" ID="saveimg" ImageUrl="../images/loadinggif.gif" style="vertical-align: center; display: none" ClientIDMode="Static" />
			<asp:Label runat="server" ID="savedtext" style="display: none" ClientIDMode="Static">Saved</asp:Label>
		</div>
		<asp:Table runat="server" CssClass="headertable" ID="accounttable" style="margin-left: auto; margin-right: auto; width: 300px">
			<asp:TableHeaderRow TableSection="TableHeader">
				<asp:TableHeaderCell style="text-align: left; width: 20px;"></asp:TableHeaderCell>
				<asp:TableHeaderCell style="width: 130px;">Username</asp:TableHeaderCell>
				<asp:TableHeaderCell style="width: 60px;">Network</asp:TableHeaderCell>
				<asp:TableHeaderCell style="width: 50px;">Options</asp:TableHeaderCell>
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
						<input type="button" value="Cancel" id="popupcancel" onclick="javascript:MyAccounts.cancelPopup();" />
					</span>
				</div>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		MyAccounts.setupPage();
	</script>
</asp:Content>
