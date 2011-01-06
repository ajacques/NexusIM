<%@ Page Title="NexusIM - Search Results" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="NexusWeb.Pages.SearchPage" %>
<%@ OutputCache Duration="5000" VaryByParam="None" Location="ServerAndClient" VaryByHeader="Cookie" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript" src="js/search.js"></script>
	<script type="text/javascript" src="Services/MessageFeed.svc/jsdebug"></script>
	<style type="text/css">
		.Help
		{
			color: Blue;
			font-style: italic;
			font-size: 12pt;
			text-align: center;
			width: 500px;
			background-color: White;
			-moz-border-radius: 5px;
			position: fixed;
			
		}
		.SearchResult
		{
			border: solid 1px #ccc;
			width: 100%;
			border-collapse: collapse;
		}
		#total
		{
			min-height: 220px;
		}
	</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<h4>Parameters</h4>
	<form onsubmit="Search.BeginSearch();return false">
		<input id="search" type="text" style="margin: 5px;" />
		<select>
			<option>Any</option>
			<option>Marquette, MI</option>
		</select>
		<input type="submit" Value="Search" />
	</form>
	<h4 id="ResultPane" style="border-bottom: solid 1px #c0c0c0; display: none; margin-bottom: 10px; margin-top: 20px;">Results</h4>
	<div id="Loading" class="Help" style="display: none">Please Wait<br /><img src="images/ajax-loader.gif" style="margin-top: 10px" /></div>
	<div id="NoResults" class="Help" style="display: none;">No results found.</div>
	<ul id="SearchResults" style="list-style: none"></ul>
		<div id="popup" class="popupouter" style="display: none">
		<div class="popupmid">
			<div class="popupinner">
				<div id="popuptitle" class="popuptitle"></div>
				<div id="popupcontent" class="popupcontent"></div>
				<div id ="popupbuttoncontainer">
					<span id="popupbuttons" class="popupbuttons">
						<input type="button" value="Send" id="popupaccept" onclick="Search.SendFriendRequest();" />
						<input type="button" value="Cancel" id="popupcancel" onclick="Search.CancelPopup();" />
					</span>
				</div>
			</div>
		</div>
	</div>
</asp:Content>