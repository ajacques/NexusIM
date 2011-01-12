<%@ Page Title="News Feed" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="newsfeed.aspx.cs" Inherits="NexusWeb.Pages.NewsFeed" %>
<%@ OutputCache Duration="100" Location="Client" VaryByParam="None" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="css/NewsFeed.css" type="text/css" rel="Stylesheet" />
	<script type="text/javascript" src="js/modernizer.js"></script>
	<script type="text/javascript" src="js/newsfeed.js"></script>
	<script type="text/javascript" src="js/LocationBuilder.js"></script>
	<script type="text/javascript" src="js/jStorage.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightcolcontent" runat="server">
	<div class="colheadline">Search</div>
	<form action="search.aspx">
		<input id="SearchBox" type="text" style="margin: 10px; -moz-box-shadow: 0 0 15px rgba(0, 168, 50, 0.5); width: 200px; border: solid 1px rgba(0, 168, 50, 1)" onkeyup="NewsFeed.searchBarKeyUp(event);" onblur="$('#SearchBoxSuggest').hide();" />
	</form>
	<div id="SearchBoxSuggest" class="SuggestionBox" style="display: none; margin-left: 10px; width: 189px">
		<ul id="SearchSuggestList"></ul>
	</div>
	<div class="colheadline">Requests</div>
	<div id="RequestNoneAlert" style="font-style: italic; font-size: 9pt; text-align: center;">None</div>
	<ul id="requestList">
	</ul>
	<div class="colheadline">Tasks</div>
	<ul>
		<li><a href="config/myaccount.aspx">Account Settings</a></li>
	</ul>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div id="UserHoverCard" onmouseover="cancelHoverBoxHide = true;" onmouseout="cancelHoverBoxHide = false;NewsFeed.onUserLinkHoverOut();" style="display: none">
		<div id="UserHoverCardContent" style="position: absolute"></div>
	</div>
	<div id="StatusUpdateContainer" onclick="cancelStatusMsgHide = true;" style="width: 540px; border-bottom: solid 1px #eeeeee; ">
		<label for="statusupdate" style="font-family: 'Lucida Grande',sans-serif; margin-bottom: 10px; font-size: 20px;">How are you today?</label>
		<textarea ID="statusupdate" onkeyup="return NewsFeed.statusKeyPress(event);"></textarea> <!-- onfocus="NewsFeed.clearStatusBox();" onblur="NewsFeed.fillStatusBox();" -->
		<div id="StatusUpdateSuggestions" class="SuggestionBox" style="display: none;">
			<ul id="susuggest"></ul>
		</div>
		<div id="PostSpan" style="height: 25px;">
			<span id="GeoLoc" style="display: none">
				<a id="GeoLocLink" href="javascript:NewsFeed.enableGeoLocation();" style="font-size: 8pt;">Add Location</a>
				<span id="GeoLocStatus" style="font-size: 8pt"></span>
				<a id="GeoLocDisableLink" href="javascript:NewsFeed.disableGeoLocation();" style="font-size: 8pt; display: none;">×</a>
			</span>
			<input id="PostButton" type="button" value="Post" onclick="NewsFeed.postStatusMessage();" style="float: right; margin-right: 5px; font-family: 'helvetica'" />
		</div>
	</div>
	<script type="text/javascript">
		var friendListVersion = <%= this.ver %>;
		var selfId = <%= this.selfid %>;
		var Self = new Object();
		Self.FirstName = "<%= this.user.firstname %>";
		Self.LastName = "<%= this.user.lastname %>";
		Self.UserId = <%= this.user.id %>;
		var isTempLogin = false;
	</script>
	<ul id="feed" style="list-style-type: none">
	</ul>
</asp:Content>