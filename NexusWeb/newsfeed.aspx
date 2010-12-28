<%@ Page Title="News Feed" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="newsfeed.aspx.cs" Inherits="NexusWeb.Pages.NewsFeed" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="css/NewsFeed.css" type="text/css" rel="Stylesheet" />
	<script type="text/javascript" src="js/modernizer.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightcolcontent" runat="server">
	<div class="colheadline">Requests</div>
	<span runat="Server" ID="Span1"></span>
	<div class="colheadline">Tasks</div>
	<asp:BulletedList runat="server">
	</asp:BulletedList>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div id="UserHoverCard" onmouseover="cancelHoverBoxHide = true;" onmouseout="cancelHoverBoxHide = false;NewsFeed.onUserLinkHoverOut();">
		<div id="UserHoverCardContent" style="position: absolute"></div>
	</div>
	<div onclick="cancelStatusMsgHide = true;" style="width: 100%; border-bottom: solid 1px #eeeeee; border-right: solid 1px #eeeeee; -moz-border-radius-bottomright: 5px;">
		<label for="statusupdate" style="font-family: 'Lucida Grande',sans-serif; margin-bottom: 10px; font-size: 20px;">How are you today?</label>
		<asp:TextBox runat="server" ID="statusupdate"  onkeypress="return NewsFeed.statusKeyPress();" ClientIDMode="Static" TextMode="MultiLine" /> <!-- onfocus="NewsFeed.clearStatusBox();" onblur="NewsFeed.fillStatusBox();" -->
		<div id="PostSpan" style="height: 25px;">
			<span id="GeoLoc" style="display: none";>
				<a id="GeoLocLink" href="javascript:NewsFeed.enableGeoLocation();" style="font-size: 11px; font-family: 'Lucida Grande',sans-serif;">Add Location</a>
				<span id="GeoLocStatus" style="font-size: smaller"></span>
				<a id="GeoLocDisableLink" href="javascript:NewsFeed.disableGeoLocation();" style="font-size: smaller; display: none">×</a>
			</span>
			<input id="PostButton" type="button" value="Post" onclick="NewsFeed.postStatusMessage();" style="float: right; margin-right: 5px; font-family: 'helvetica'" />
		</div>
	</div>
	<script type="text/javascript">
		var friendListVersion = <%= this.ver %>;
		var selfId = <%= this.selfid %>;
		var isTempLogin = false;
		NewsFeed.startStream();
		//NewsFeed.fillStatusBox();
		$("#headerright").gradient({
			from: 'BABABA',
			to: '000000',
			direction: 'vertical'
		});
		if (Modernizr.geolocation)
			$("span#GeoLoc").show();
	</script>
	<ul id="feed" style="list-style-type: none">
	</ul>
</asp:Content>