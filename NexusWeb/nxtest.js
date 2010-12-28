/// <reference path="Websocket.doc.js" />
/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.debug.js" />

var NewsFeedPush = function ()
{
	this._socket = null;
	this._path = "";
	this._callbackEvents = new Sys.EventHandlerList();
	this._events = new Sys.EventHandlerList();
}

NewsFeedPush.prototype = {
	get_Path: function ()
	{
		return this._path;
	},

	set_Path: function (value)
	{
		this._path = value;
	},

	Open: function ()
	{
		this._socket = new WebSocket(this.get_Path());
		this._socket.onmessage = this._message_parser;
		this._socket.onopen = this._socket_open;
	},

	_socket_open: function ()
	{
		var delegate = this._events.getHandler("Open");

		if (delegate)
			delegate(this, Sys.EventArgs.Empty);
	},

	add_OnOpen: function (callback)
	{
		this._events.addHandler("Open", callback);
	},

	remove_OnOpen: function (callback)
	{
		this._events.removeHandler("Open", callback);
	},

	_message_parser: function (message)
	{
		var output = Sys.Serialization.JavaScriptSerializer.deserialize(message);
		var target = output.__targetinvoke;

		var delegate = this._callbackEvents.getHandler(target);
		if (delegate)
			delegate(this, output._d);
	},

	_invoke: function (target, parameters)
	{
		var packet = { __targetinvoke: target, _d: parameters };
		var output = Sys.Serialization.JavaScriptSerializer.serialize(packet);
		
		this._socket.send(output);
	},

	// Begin Custom Functions Generated from WCF Metadata
	PostStatusMessage: function (message, callback)
	{
		this._invoke("PostStatusMessage", { messageBody: message });
	},

	// Metadata Callbacks
	add_OnStatusMessage: function (callback)
	{
		this._callbackEvents.addHandler("OnStatusMessage", callback);
	},

	remove_OnStatusMessage: function (callback)
	{
		this._callbackEvents.removeHandler("OnStatusMessage", callback);
	}
}

NewsFeedPush.set_path = function (value)
{
	NewsFeedPush._staticInstance.set_path(value);
}

NewsFeedPush.Open = function ()
{
	NewsFeedPush._staticInstance.Open();
}

NewsFeedPush.PostStatusMessage = function (message)
{
	NewsFeedPush._staticInstance.PostStatusMessage(message);
}

NewsFeedPush.registerClass('NewsFeedPush');
NewsFeedPush._staticInstance = new NewsFeedPush();
//NewsFeedPush.set_path('ws://dev.nexus-im.com/newsfeed');
NewsFeedPush.set_Path("ws://dev.nexus-im.com:8181/newsfeed");