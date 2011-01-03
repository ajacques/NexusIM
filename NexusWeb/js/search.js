/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.debug.js" />
/// <reference path="http://dev.nexus-im.com/Services/MessageFeed.svc/jsdebug" />
/// <reference path="http://dev.nexus-im.com/locales/strings.en-US.js" />
/// <reference path="http://code.jquery.com/jquery-1.4.1-vsdoc.js" />

Type.registerNamespace("Search");

Search.SearchUrl = "http://dev.nexus-im.com/Services/ArticleFeed.svc/Users()?$filter=substringof('{0}',concat(firstname,concat(' ',lastname)))";
var userResultControl = document.createElement("li");
var completedControl = false;
var completedGet = false;
var tempResults = null;

Search.BeginSearch = function()
{
	$("#ResultPane").fadeIn('normal');
	$("#Loading").fadeIn('normal');
	var terms = $("#search").val();	
	
	MessageFeed.GetFriendsMatching(terms, function(data)
	{
		completedGet = true;
		if (completedControl)
			Search.OnSearchResults(data);
		else
			tempResults = data.d;
	});

	/*$.ajax({
		type: "GET",
		url: Search.SearchUrl.format(terms),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function(data)
		{
			completedGet = true;
			if (completedControl)
				Search.OnSearchResults(data.d);
			else
				tempResults = data.d;
		}
	});*/
}
Search.OnSearchResults = function(results)
{
	$("#SearchResults").html("");
	$("#Loading").hide();

	var now = new Date();

	for (var result in results)
	{
		var user = results[result];
		var li = userResultControl.cloneNode(true);
		$("#FullName", li).html(user.FirstName + " " + user.LastName);
		var difference = Math.ceil((now.getTime() - user.DateOfBirth.getTime()) / 31536000000);

		$("span#Age", li).html(difference);

		$("#SearchResults").append(li);
	}
}

$("body").ready(function()
{
	var regex = new RegExp("[\\?&]query=([^&#]*)");
	var result = regex.exec(window.location.href);

	$.ajax({
		type: "GET",
		url: "SearchUserResult.html",
		success: function(data)
		{
			userResultControl.innerHTML = data;
			completedControl = true;
			if (completedGet && tempResults != null)
			{
				Search.OnSearchResults(tempResults);
				tempResults = null;
			}
		}
	});

	if (result != null)
	{
		$("#search").val(result[1]);
		Search.BeginSearch();
	}
});