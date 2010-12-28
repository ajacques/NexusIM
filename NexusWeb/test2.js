INewsFeedPush = function () {
	this._socket = null;
	this._path = '';
	this._callbackEvents = new Sys.EventHandlerList();
	this._events = new Sys.EventHandlerList();
}
INewsFeedPush.prototype = {
	get_Path: function () {
		return this._path
	},
	set_Path: function (path) {
		this._path = path;
	},
	Open: function () {
		this._socket = new WebSocket(this._path);
		this._socket.onopen = this._socket_onopen;
		this._socket.onmessage = this._message_onmessage;
	},
	_socket_open: function () {
		var delegate = this._events.getHandler('Open');
		if (delegate) delegate(this, Sys.EventArgs.Empty);
	},
	_socket_onmessage: function (message) {
		var output = Sys.Serialization.JavaScriptSerializer.deserialize(message);
		var delegate = this._callbackEvents.getHandler(output._targetinvoke);
		if (delegate) delegate(this, output._d);
	},
	_invoke: function (target, parameters) {
		var packet = { __targetinvoke: target, _d: parameters };
		var output = Sys.Serialization.JavaScriptSerializer.serialize(packet);
		this._socket.send(output);
	},
	add_OnOpen: function (callback) {
		this._events.addHandler("OnOpen", callback);
	},
	remove_OnOpen: function (callback) {
		this._events.addHandler("OnOpen", callback);
	},
	PostStatusMessage: function (messageBody) {
		var params = [];
		for (var i = 1; i < arguments.length; i++) params.append(arguments[i]);
		this._invoke('PostStatusMessage', params);
	}
}
INewsFeedPush.PostStatusMessage = function (messageBody) {
	INewsFeedPush._staticInstance.PostStatusMessage(messageBody);
}
INewsFeedPush.Open = function () {
	INewsFeedPush._staticInstance.Open();
}
INewsFeedPush.get_Path = function () {
	return INewsFeedPush._staticInstance.get_Path();
}
INewsFeedPush.set_Path = function (value) {
	INewsFeedPush._staticInstance.set_Path(value);
}
INewsFeedPush.add_OnOpen = function (callback) {
	INewsFeedPush._staticInstance.add_OnOpen(callback);
}
INewsFeedPush.remove_OnOpen = function (callback) {
	INewsFeedPush._staticInstance.remove_OnOpen(callback);
}
INewsFeedPush.registerClass('INewsFeedPush');
INewsFeedPush._staticInstance = new INewsFeedPush();
INewsFeedPush.set_Path('ws://dev.nexus-im.com/newsfeed');