<%@ Page Title="" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="NexusWeb.Pages.UserPage" %>
<asp:Content ID="Content4" ContentPlaceHolderID="rightcolcontent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
	<asp:Image runat="server" ID="avatar" class="BackShadow" Width="100px" ImageUrl="~/Services/MessageFeed.svc/GetUserImage?userid={0}&size=128" style="float: left; margin-right: 10px; margin-left: -50px" />
	<asp:Label runat="server" ID="majorname" style="font-size: 22pt; font-family: Verdana, sans-serif" /><br />
	<asp:Label runat="server" ID="laststatus" ClientIDMode="Static" style="padding-left: 100px" />
	<div>
		<h2 style="font-size: 12pt">Photos</h2>
	</div>
	<div id="statbox" class="BackShadow" style="position: relative; float: left; margin: 20px 10px 0 -60px; background-color: #ffffff; width: 100px">
		<div id="statCntrDobHead" class="colheadline" style="padding-left: 5px; width: inherit; display: none;">Birthday</div>
		<div id="statCntrBirthday" style="padding-left: 5px; display: none;">
			<asp:Label runat="server" ID="dateofbirth" ClientIDMode="Static" style="display: none; padding: 1px 5px 1px 0;" />
			<div id="statsAge"></div>
		</div>
		<div id="curlocStatContainer" style="display: none">
			<div class="colheadline" style="padding-left: 5px; width: inherit;">Location</div>
			<div id="curlocation" style="text-align: center; overflow: hidden"><img src="images/loadinggif.gif" /></div>
		</div>
	</div>
	<script type="text/javascript">
	var userid = <%= remoteid %>;
	User.loadData();
	</script>
</asp:Content>