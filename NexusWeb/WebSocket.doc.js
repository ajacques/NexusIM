var WebSocket = function(uri)
{
	/// <summary>Opens a new WebSocket to the designated Uri</summary>
	/// <param name="uri">Uri scheme must be either ws:// or wss://</param>

	this.readyState = 0;
	this.bufferedAmount = 0.0;
	this.url = "";
};
WebSocket.prototype =
{
	send: function(message)
	{
		/// <summary>Transmits the message to the server</summary>
	},

	close: function()
	{
		/// <summary>Disconnects from the server.</summary>
	},

	onopen: function(event)
	{
		/// <summary>Triggered after the socket has connected to the server</summary>
	},
	
	onmessage: function(event)
	{
		/// <summary>Triggered after the socket receives a message from the server</summary>
	}
};

WebSocket.CONNECTING = 0;
WebSocket.OPEN = 1;
WebSocket.CLOSING = 2;
WebSocket.CLOSED = 3;