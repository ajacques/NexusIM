/// <reference path="Websocket.doc.js" />

Type.registerNamespace("NxPush");

if (typeof (NxPush.StreamStateChange) === 'undefined')
{
	NxPush.StreamStateChange = function () { throw Error.invalidOperation(); }
	NxPush.StreamStateChange.prototype = { NotLoggedIn: 0, Ready: 1, Streaming: 2 }
	NxPush.StreamStateChange.registerEnum('NxPush.StreamStateChange', true);
}

var websocket = null;
var start = function ()
{
	var inc = document.getElementById('incoming');
	inc.innerHTML += "WebSocket Support Detected<br />Connecting... Please Wait<br/>";

	// create a new websocket and connect
	websocket = new WebSocket('ws://dev.nexus-im.com:8181/newsfeed');

	websocket.onmessage = function (evt)
	{
		var message = Sys.Serialization.JavaScriptSerializer.deserialize(evt.data, true);
		if (message.__type = "StreamStateChange")
		{
			inc.innerHTML += "New Stream State: ";

			if (message.State == NxPush.StreamStateChange.NotLoggedIn)
				inc.innerHTML += "NotLoggedIn<br />";
			else if (message.State == NxPush.StreamStateChange.Ready)
			{
				

				inc.innerHTML += "Ready<br />";
			} else if (message.State == NxPush.StreamSTateChange.Streaming)
				inc.innerHTML += "Streaming<br />";
		}
	};

	websocket.onopen = function ()
	{
		inc.innerHTML += 'Connection Established... Authenticating...<br />';

		var ticket = new Object();
		ticket.__type = "Authentication";
		ticket.ticket = "tY0cm6a9GmsXXBRyx8GIAlIsAJmhRT";

		websocket.send(Sys.Serialization.JavaScriptSerializer.serialize(ticket));
	};

	websocket.onclose = function ()
	{
		inc.innerHTML += 'Connection Lost<br />';
	}
}