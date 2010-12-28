/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.debug.js" />
/// <reference path="http://dev.nexus-im.com/Services/MessageFeed.svc/jsdebug" />
/// <reference path="http://dev.nexus-im.com/locales/strings.en-US.js" />
/// <reference path="http://code.jquery.com/jquery-1.4.1-vsdoc.js" />

Type.registerNamespace("NewsFeed");

var Friends = [];
var userHoverOpen = false;
var cancelHoverBoxHide = false;
var cancelHoverBoxShow = false;
var mapsetup = false;
var mapobj = null;
var userHoverDelay = 500;
var Self = null;
var lastStreamRequest = null;
var cancelStatusMsgBoxHide = false;
var lastGeoLoc = null;
var lastGeoResolve = null;
var waitingMsgBody = "";

NewsFeed.startStream = function ()
{
	MessageFeed.GetUserDetails(selfId, function(data)
	{
		Self = data;
	});
	MessageFeed.GetFriends(function (data)
	{
		for (var i = 0; i < data.length; i++)
		{
			var obj = data[i];
			var key = obj["Prefix"] + obj["UserId"];
			Friends[key] = obj;
			if (!isTempLogin)
				$.jStorage.set(key, obj);
		}
		
		NewsFeed.downloadStatus();
	});
}

NewsFeed.setupMap = function ()
{
	if (mapsetup)
		return;

	
	mapsetup = true;
}

NewsFeed.UpdateTimestamps = function()
{
	var now = (new Date()).getTime();

	$(".ArticleTimestamp").each(function(item)
	{
		var iDate = this.getAttribute("timestamp");

		var seconds = (now - iDate) / 1000;
		if (seconds < 60)
		{
			this.innerHTML = Strings.PluralSecondsAgo.format(Math.ceil(seconds));
			return;
		}
		var minutes = seconds / 60;
		if (minutes < 60)
		{
			this.innerHTML = Strings.PluralMinutesAgo.format(Math.ceil(minutes));
			return;
		}
		var hours = minutes / 60;
		if (hours < 24)
		{
			this.innerHTML = Strings.PluralHoursAgo.format(Math.ceil(hours));
			return;
		}
		var days = hours / 24;
		if (days < 7)
		{
			this.innerHTML = Strings.PluralDaysAgo.format(Math.ceil(days));
			return;
		}
		var weeks = days / 7;
		this.innerHTML = Strings.PluralWeeksAgo.format(Math.ceil(weeks));
	});
}

NewsFeed.onArticlesDownload = function (content)
{
	lastStreamRequest = new Date();
	for (var i = 0; i < content.length; i++)
	{
		var obj = content[i];
		$("ul#feed").append(NewsFeed.handleAddArticleMessage(obj));
	}
	NewsFeed.UpdateTimestamps();
	NewsFeed.ArticleLongPoll();
}

NewsFeed.handleAddArticleMessage = function(obj)
{
	var time = obj["TimeStamp"];

	// DOM generation
	var li = document.createElement("li");
	li.setAttribute("aid", obj["ArticleId"]);
	li.setAttribute("fid", obj["UserPrefix"] + obj["UserId"]);

	var table = document.createElement("table");
	var tr1 = document.createElement("tr");
	var tr2 = document.createElement("tr");
	var tr3 = document.createElement("tr");

	table.setAttribute("class", "ArticleTable");

	// CommentAttachNode
	var cmdtd = document.createElement("td");
	var cmdtable = document.createElement("table");
	cmdtd.appendChild(cmdtable);
	cmdtd.setAttribute("colspan", "2");
	tr3.appendChild(cmdtd);

	cmdtable.setAttribute("class", "CommentAttachNode");

	var imgtd = document.createElement("td");
	imgtd.setAttribute("class", "ArticleMainImageContainer");
	tr1.appendChild(imgtd);
	var contactimg = document.createElement("img");

	contactimg.setAttribute("src", "/Services/MessageFeed.svc/GetUserImage?userid=" + obj["UserId"] + "&size=32");
	imgtd.appendChild(contactimg);
	imgtd.setAttribute("rowspan", "2");

	// Command Bar
	var cmdtable = document.createElement("table");

	var datetd = document.createElement("td");
	datetd.setAttribute("timestamp", time.getTime());
	datetd.setAttribute("class", "ArticleTimestamp");

	var cmdtd = document.createElement("td");
	var commenthref = document.createElement("a");

	cmdtable.setAttribute("class", "ArticleCommandBar");

	commenthref.innerHTML = Strings.CommentText;

	cmdtd.appendChild(commenthref);
	cmdtable.appendChild(datetd);
	cmdtable.appendChild(cmdtd);

	tr2.appendChild(cmdtable);

	var msgtd = document.createElement("td");
	msgtd.setAttribute("class", "ArticleBody");
	tr1.appendChild(msgtd);

	table.appendChild(tr1);
	table.appendChild(tr2);
	table.appendChild(tr3);
	li.appendChild(table);

	var userhref = document.createElement("a");
	var friendobj = null;
	if (obj["UserId"] == selfId)
		friendobj = Self;
	else
		friendobj = Friends[obj["UserPrefix"] + obj["UserId"]];

	if (friendobj == null)
		return; // Bad idea

	userhref.setAttribute("href", "user.aspx?userid=" + obj["UserId"]);
	userhref.setAttribute("class", "ArticleUserHref");
	userhref.innerHTML = friendobj.FirstName + " " + friendobj.LastName;
	userhref.setAttribute("onmouseover", "javascript:NewsFeed.onUserLinkHover(" + obj["UserId"] + ");");
	userhref.setAttribute("onmouseout", "javascript:NewsFeed.onUserLinkHoverOut();");

	if (obj.__type == "ClientStatusUpdate")
	{
		var messagebody = document.createElement("span");
		li.setAttribute("atype", "status");

		messagebody.setAttribute("class", "statusbody");
		messagebody.innerHTML = obj["MessageBody"];

		msgtd.appendChild(userhref);
		msgtd.appendChild(messagebody);
		commenthref.setAttribute("href", "javascript:NewsFeed.startArticleComment('status', " + obj["ArticleId"] + ");");
	}
	return li;
}

/// <summary>
/// Prepares the hover window and starts the wait timer.
/// </summary>
NewsFeed.onUserLinkHover = function (articleid)
{
	if (userHoverOpen)
	{
		cancelHoverBoxHide = true;
		return;
	}

	userHoverOpen = true;

	$("UserHoverCardContent").html(""); // Clear any existing cards

	var position = {};
	var friend;

	if (articleid == -1) // Custom positioning for the giant main picture
	{
		var ps = NewsFeed.getAbsolutePosition("img#MyDisplayImageByUpdatebox");
		position = { y: 70, x: ps.x -= 520 };

		friend = Friends["nx" + selfId];
	} else {
		var li = $("li[aid=" + articleid + "]", $("ul#feed"));
		friend = Friends[li.attr("fid")];
		var articleuserhref = $("a.articleuserhref", li);
		position = NewsFeed.getAbsolutePosition(articleuserhref);
	}

	// Begin Construction of Popup body
	var title = document.createElement("div");
	title.setAttribute("style", "background-color: #000000; width: 100%; color: #ffffff");
	title.innerHTML = friend.FirstName + " " + friend.LastName;

	if (friend.LocationAllowed)
	{
		
	}
	// End Construction

	$("#UserHoverCard").css("top", position.y + "px");
	$("#UserHoverCardContent").append(title);

	setTimeout("javascript:NewsFeed.doUserHoverShow();", userHoverDelay);
}

NewsFeed.doUserHoverShow = function ()
{
	if (cancelHoverBoxShow)
		return;

	$("#UserHoverCard").show();
}

NewsFeed.onUserLinkHoverOut = function ()
{
	cancelHoverBoxShow = true;
	setTimeout("javascript:NewsFeed.doUserHoverHide();", userHoverDelay);
}

NewsFeed.doUserHoverHide = function ()
{
	if (!cancelHoverBoxHide)
	{
		$("#UserHoverCard").hide();
		$("#UserHoverCardContent").html("");
		userHoverOpen = false;
	}
	cancelHoverBoxHide = false;
}

NewsFeed.ArticleLongPoll = function ()
{
	//MessageFeed.GetStatusUpdatesSinceLongPoll(lastStreamRequest);
}

NewsFeed.downloadStatus = function ()
{
	MessageFeed.GetStatusUpdatesSince(new Date(1), NewsFeed.onArticlesDownload, function (data)
	{
		alert(data["_message"]);
	});
}

NewsFeed.clearStatusBox = function ()
{
	var box = $("#statusupdate");
	var button = $("#PostSpan");  //$("#PostButton");
	if (box.val() == Strings.StatusUpdateBoxTip)
		box.removeClass("faded").val("");

	if (box.height() <= 30)
		box.animate({ height: "30px" }, 100);

	button.animate({ opacity: "1", height: "25px" }, 200);
}

NewsFeed.fillStatusBox = function ()
{
	var box = $("#statusupdate");
	var button = $("#PostSpan");  //$("#PostButton");
	if (box.val() == Strings.StatusUpdateBoxTip || box.val() == "")
	{
		box.addClass("faded").val(Strings.StatusUpdateBoxTip);
		button.animate({ opacity: "0", height: "0" }, 200);
		box.animate({ height: "25px" }, 200);
	}
}

NewsFeed.statusKeyPress = function ()
{
	if (window.event && window.event.keyCode == 13)
	{
		var box = $("#statusupdate");

		return false;
	}
}

NewsFeed.onPostStatusBoxLoseFocus = function (target)
{
	if (target.value == "")
	{
		var toptr = target.parentNode.parentNode;
		toptr.parentNode.removeChild(toptr);
	}
}

NewsFeed.startArticleComment = function (articleType, articleId)
{
	var context = $("li[aid='" + articleId + "']|[atype='" + articleType + "']"); // Get the article root element
	var node = $("table.CommentAttachNode", context);
	var tr1 = document.createElement("tr");
	var td1 = document.createElement("td");

	var cmtarea = document.createElement("textarea");

	cmtarea.setAttribute("onblur", "javascript:NewsFeed.onPostStatusBoxLoseFocus(this);");

	td1.appendChild(cmtarea);
	td1.setAttribute("colspan", "2");
	tr1.setAttribute("class", "ArticlePostCommentTr");
	tr1.setAttribute("id", "NewCommentTr" + articleId + "-" + articleType);

	tr1.appendChild(td1);
	node.append(tr1);
	cmtarea.focus();
}

NewsFeed.getAbsolutePosition = function(theObj)
{
	x = y = 0;
	h = theObj.offsetHeight;
	w = theObj.offsetWidth;
	while (theObj)
	{
		x += theObj.offsetLeft;
		y += theObj.offsetTop;
		theObj = theObj.offsetParent;
	}
	return {height: h, width: w, x: x, y: y}
}

NewsFeed.enableGeoLocation = function()
{
	$("a#GeoLocLink").hide();
	$("a#GeoLocDisableLink").show();
	$("span#GeoLocStatus").html("Acquiring");
	navigator.geolocation.getCurrentPosition(function (pos)
	{
		$("span#GeoLocStatus").html("Resolving");
		lastGeoLoc = pos.coords;

		GeoService.LatLngToCity(lastGeoLoc.latitude, lastGeoLoc.longitude,
		function(data)
		{
			lastGeoResolve = data;

			if (Sys.Debug.isDebug)
				Sys.Debug.trace(GetSimpleString(data));

			$("span#GeoLocStatus").html(GetSimpleString(data));
		});
	}, function(err)
	{
		var img = document.creatElement("img");
		img.setAttribute("src", "images/exclamation.png");
		img.setAttribute("alt", err.message);
		$("span#GeoLocStatus").html("Failed").append(img);
	});
}

NewsFeed.disableGeoLocation = function()
{
	$("a#GeoLocLink").show();
	$("a#GeoLocDisableLink").hide();
	$("span#GeoLocStatus").html("");
	lastGeoLoc = null;
	lastGeoResolve = null;
}

NewsFeed.postStatusMessage = function()
{
	if ($("#statusupdate").val() === "")
		return;
	$("textarea#statusupdate").attr("disabled", "disabled");
	$("input#PostButton").attr("disabled", "disabled").val("Posting"); // Prevent the user from accidentally posting the message twice

	waitingMsgBody = $("textarea#statusupdate").val();

	var geoloc = null;

	if (lastGeoLoc != null)
	{
		geoloc = new GeoLocation(); // Convert the coordinates to the server format
		geoloc.Latitude = lastGeoLoc.latitude;
		geoloc.Longitude = lastGeoLoc.longitude;
		geoloc.Accuracy = lastGeoLoc.accuracy;
		geoloc.Altitude = lastGeoLoc.altitude;
	}

	MessageFeed.PostStatusMessage(waitingMsgBody, geoloc,
	function(result) // Success Callback
	{
		$("textarea#statusupdate").attr("disabled", "").val("");
		$("input#PostButton").attr("disabled", "").val("Post"); // Re-enable the box

		var obj = new ClientStatusUpdate();
		obj.__type = "ClientStatusUpdate";
		obj.ArticleId = result;
		obj.Comments = null;
		obj.TimeStamp = new Date();
		obj.UserId = Self.UserId;
		obj.UserPrefix = "nx";
		obj.MessageBody = waitingMsgBody;
		
		if (lastGeoLoc != null)
			obj.GeoTag = lastGeoLoc;

		var li = NewsFeed.handleAddArticleMessage(obj);
		$(li).hide();
		$("ul#feed").prepend(li);
		$(li).slideDown();
	});
}

$("body").ready(function()
{
	NewsFeed.startStream();
	//NewsFeed.fillStatusBox();
	$("#headerright").gradient({
		from: 'BABABA',
		to: '000000',
		direction: 'vertical'
	});
	if (Modernizr.geolocation)
		$("span#GeoLoc").show();
});