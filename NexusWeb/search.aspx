<%@ Page Title="NexusIM - Search Results" Language="C#" MasterPageFile="~/centered.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="NexusWeb.Pages.SearchPage" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript" src="js/search.js"></script>
	<style type="text/css">
		#Loading
		{
			color: Blue;
			font-style: italic;
			font-size: 12pt;
			text-align: center;
			width: 100%;
			background-color: White;
			-moz-border-radius: 5px;
		}
		.SearchResult
		{
			border: solid 1px #ccc;
			width: 100%;
			border-collapse: collapse;
		}
	</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<h4>Parameters</h4>
	<input id="search" type="text" style="margin: 5px;" />
	<select>
		<option>Any</option>
		<option>Marquette, MI</option>
	</select>
	<input type="button" Value="Search" onclick="Search.BeginSearch();" />
	<h4 id="ResultPane" style="border-bottom: solid 1px #c0c0c0; display: none; margin-bottom: 10px; margin-top: 20px;">Results</h4>
	<div id="NoResults" style="color: Blue; font-style: italic; font-size: 12pt; text-align: center; display: none;">No results found.</div>
	<ul id="SearchResults" style="list-style: none"></ul>
	<div id="Loading" style="display: none">Please Wait<br /><img src="images/ajax-loader.gif" style="margin-top: 10px" /></div>
</asp:Content>