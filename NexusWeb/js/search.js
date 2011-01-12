/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.debug.js" />
/// <reference path="http://dev.nexus-im.com/Services/MessageFeed.svc/jsdebug" />
/// <reference path="http://dev.nexus-im.com/locales/strings.en-US.js" />
/// <reference path="http://dev.nexus-im.com/js/popup.js" />
/// <reference path="http://code.jquery.com/jquery-1.4.1-vsdoc.js" />

Type.registerNamespace("Search");
Search.SearchUrl = "http://dev.nexus-im.com/Services/ArticleFeed.svc/users/?fullname=contains('{0}')";
var userResultControl = document.createElement("li");
var completedControl = false;
var completedGet = false;
var tempResults = null;
var pendingUserRequestId = 0;

Search.StripSlashes = function(date)
{
	return date.replace(/\//, "").replace(/\//, "");
}

Search.BeginSearch = function(isNew)
{
	$("#ResultPane").fadeIn('normal');
	$("#Loading").fadeIn('normal');
	$("#NoResults").hide();
	var terms = escape($("#search").val());

	$.ajax({
		type: 'GET',
		url: Search.SearchUrl.format(terms),
		dataType: 'text',
		beforeSend: function (xhr, settings)
		{
			xhr.setRequestHeader("Accept", "application/json");
			return true;
		},
		success: function(data)
		{
			completedGet = true;

			var result = Sys.Serialization.JavaScriptSerializer.deserialize(data); // jQuery doesn't de-serialize the Date object properly

			if (completedControl)
				Search.OnSearchResults(result);
			else
				tempResults = result;
		}
	});

	window.history.pushState(terms, '', 'search.aspx?query=' + terms);
}

Search.OnSearchResults = function(results)
{
	$("#SearchResults").html("");
	$("#Loading").hide();
	tempResults = results;

	var now = new Date();

	if (results[0] == null)
	{
		$("#NoResults").fadeIn();
		return;
	}

	for (var result in results)
	{
		var user = results[result];
		var li = userResultControl.cloneNode(true);
		$("#FullName", li).html(user.FirstName + " " + user.LastName).attr("href", "user.aspx?userid=" + user.UserId);

		var difference = Math.floor((now.getTime() - user.DateOfBirth.getTime()) / 31536000000);
		$("#UserImage", li).attr("src", "Services/MessageFeed.svc/GetUserImage?userid=" + user.UserId + "&size=64");

		$("#AddAsFriendLink", li).click(function()
		{
			pendingUserRequestId = user.UserId;
			$("div#popuptitle").html("Add Friend");
			$.ajax({
				url: "popups/addfriend.htm",
				success: function(data)
				{
					var html = $("div#popupcontent").html(data).first();
					$("#FullName", html).html(user.FirstName + " " + user.LastName);
					$("#UserImage", html).attr("src", "Services/MessageFeed.svc/GetUserImage?userid=" + user.UserId + "&size=128");
					$("div#popup").css("display", "block");
				}
			});
		});

		$("span#Age", li).html(difference);

		$("#SearchResults").append(li);
	}
	tempResults = null;
}

Search.SendFriendRequest = function()
{
	if (pendingUserRequestId != 0)
	{
		var intro = $("textarea#Introduction").val();
		MessageFeed.SendFriendRequest(pendingUserRequestId, intro);
		Search.CancelPopup();
	}
}

Search.CancelPopup = function()
{
	$("div#popup").fadeOut("normal", function()
	{
		$("div#popuptitle").html("");
		$("div#popupcontent").html("");
		$("input#popupaccept").unbind("click");
	});
	//$("div#popup").css("display", "none");
}

$("body").ready(function()
{
	var regex = new RegExp("[\\?&#]query=([^&#]*)");
	var result = regex.exec(window.location.href);
	$.ajax({
		type: "GET",
		url: "controls/SearchUserResult.html",
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
		Search.BeginSearch(false);
	}
});