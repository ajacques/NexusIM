var MessageFeed = function ()
{
	MessageFeed.initializeBase(this);
	this._timeout = 0;
	this._userContext = null;
	this._succeeded = null;
	this._failed = null;
}
MessageFeed.prototype = {
	_get_path: function ()
	{
		var p = this.get_path();
		if (p) return p;
		else return MessageFeed._staticInstance.get_path();
	},
	GetStatusUpdatesSince: function (messagesSince, succeededCallback, failedCallback, userContext)
	{
		/// <param name="messagesSince" type="Date">System.DateTime</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetStatusUpdatesSince', false, { messagesSince: messagesSince }, succeededCallback, failedCallback, userContext);
	},
	GetStatusUpdatesBetween: function (startdate, enddate, succeededCallback, failedCallback, userContext)
	{
		/// <param name="startdate" type="Date">System.DateTime</param>
		/// <param name="enddate" type="Date">System.DateTime</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetStatusUpdatesBetween', false, { startdate: startdate, enddate: enddate }, succeededCallback, failedCallback, userContext);
	},
	GetStatusUpdatesSinceLongPoll: function (messagesSince, succeededCallback, failedCallback, userContext)
	{
		/// <param name="messagesSince" type="Date">System.DateTime</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetStatusUpdatesSinceLongPoll', false, { messagesSince: messagesSince }, succeededCallback, failedCallback, userContext);
	},
	test: function (messagesSince, succeededCallback, failedCallback, userContext)
	{
		/// <param name="messagesSince" type="Date">System.DateTime</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'test', false, { messagesSince: messagesSince }, succeededCallback, failedCallback, userContext);
	},
	GetArticleComments: function (articleType, articleId, succeededCallback, failedCallback, userContext)
	{
		/// <param name="articleType" type="String">System.String</param>
		/// <param name="articleId" type="Number">System.Int32</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetArticleComments', false, { articleType: articleType, articleId: articleId }, succeededCallback, failedCallback, userContext);
	},
	GetStatusUpdatesForUserPage: function (targetid, succeededCallback, failedCallback, userContext)
	{
		/// <param name="targetid" type="Number">System.Int32</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetStatusUpdatesForUserPage', false, { targetid: targetid }, succeededCallback, failedCallback, userContext);
	},
	GetFriends: function (succeededCallback, failedCallback, userContext)
	{
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetFriends', false, {}, succeededCallback, failedCallback, userContext);
	},
	GetUserDetails: function (profileid, succeededCallback, failedCallback, userContext)
	{
		/// <param name="profileid" type="Number">System.Int32</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetUserDetails', false, { profileid: profileid }, succeededCallback, failedCallback, userContext);
	},
	PostStatusMessage: function (messageBody, position, succeededCallback, failedCallback, userContext)
	{
		/// <param name="messageBody" type="String">System.String</param>
		/// <param name="position" type="GeoLocation">NexusWeb.Services.DataContracts.GeoLocation</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'PostStatusMessage', false, { messageBody: messageBody, position: position }, succeededCallback, failedCallback, userContext);
	},
	GetUserImage: function (userid, size, succeededCallback, failedCallback, userContext)
	{
		/// <param name="userid" type="Number">System.Int32</param>
		/// <param name="size" type="NexusWeb.Services.PhotoSize">NexusWeb.Services.PhotoSize</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetUserImage', false, { userid: userid, size: size }, succeededCallback, failedCallback, userContext);
	},
	GetUserMap: function (userid, succeededCallback, failedCallback, userContext)
	{
		/// <param name="userid" type="Number">System.Int32</param>
		/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
		/// <param name="userContext" optional="true" mayBeNull="true"></param>
		return this._invoke(this._get_path(), 'GetUserMap', false, { userid: userid }, succeededCallback, failedCallback, userContext);
	} 
}
MessageFeed.registerClass('MessageFeed', Sys.Net.WebServiceProxy);
MessageFeed._staticInstance = new MessageFeed();
MessageFeed.set_path = function (value)
{
	MessageFeed._staticInstance.set_path(value);
}
MessageFeed.get_path = function ()
{
	/// <value type="String" mayBeNull="true">The service url.</value>
	return MessageFeed._staticInstance.get_path();
}
MessageFeed.set_timeout = function (value)
{
	MessageFeed._staticInstance.set_timeout(value);
}
MessageFeed.get_timeout = function ()
{
	/// <value type="Number">The service timeout.</value>
	return MessageFeed._staticInstance.get_timeout();
}
MessageFeed.set_defaultUserContext = function (value)
{
	MessageFeed._staticInstance.set_defaultUserContext(value);
}
MessageFeed.get_defaultUserContext = function ()
{
	/// <value mayBeNull="true">The service default user context.</value>
	return MessageFeed._staticInstance.get_defaultUserContext();
}
MessageFeed.set_defaultSucceededCallback = function (value)
{
	MessageFeed._staticInstance.set_defaultSucceededCallback(value);
}
MessageFeed.get_defaultSucceededCallback = function ()
{
	/// <value type="Function" mayBeNull="true">The service default succeeded callback.</value>
	return MessageFeed._staticInstance.get_defaultSucceededCallback();
}
MessageFeed.set_defaultFailedCallback = function (value)
{
	MessageFeed._staticInstance.set_defaultFailedCallback(value);
}
MessageFeed.get_defaultFailedCallback = function ()
{
	/// <value type="Function" mayBeNull="true">The service default failed callback.</value>
	return MessageFeed._staticInstance.get_defaultFailedCallback();
}
MessageFeed.set_enableJsonp = function (value) { MessageFeed._staticInstance.set_enableJsonp(value); }
MessageFeed.get_enableJsonp = function ()
{
	/// <value type="Boolean">Specifies whether the service supports JSONP for cross domain calling.</value>
	return MessageFeed._staticInstance.get_enableJsonp();
}
MessageFeed.set_jsonpCallbackParameter = function (value) { MessageFeed._staticInstance.set_jsonpCallbackParameter(value); }
MessageFeed.get_jsonpCallbackParameter = function ()
{
	/// <value type="String">Specifies the parameter name that contains the callback function name for a JSONP request.</value>
	return MessageFeed._staticInstance.get_jsonpCallbackParameter();
}
MessageFeed.set_path("http://dev.nexus-im.com/Services/MessageFeed.svc");
MessageFeed.GetStatusUpdatesSince = function (messagesSince, onSuccess, onFailed, userContext)
{
	/// <param name="messagesSince" type="Date">System.DateTime</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetStatusUpdatesSince(messagesSince, onSuccess, onFailed, userContext);
}
MessageFeed.GetStatusUpdatesBetween = function (startdate, enddate, onSuccess, onFailed, userContext)
{
	/// <param name="startdate" type="Date">System.DateTime</param>
	/// <param name="enddate" type="Date">System.DateTime</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetStatusUpdatesBetween(startdate, enddate, onSuccess, onFailed, userContext);
}
MessageFeed.GetStatusUpdatesSinceLongPoll = function (messagesSince, onSuccess, onFailed, userContext)
{
	/// <param name="messagesSince" type="Date">System.DateTime</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetStatusUpdatesSinceLongPoll(messagesSince, onSuccess, onFailed, userContext);
}
MessageFeed.test = function (messagesSince, onSuccess, onFailed, userContext)
{
	/// <param name="messagesSince" type="Date">System.DateTime</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.test(messagesSince, onSuccess, onFailed, userContext);
}
MessageFeed.GetArticleComments = function (articleType, articleId, onSuccess, onFailed, userContext)
{
	/// <param name="articleType" type="String">System.String</param>
	/// <param name="articleId" type="Number">System.Int32</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetArticleComments(articleType, articleId, onSuccess, onFailed, userContext);
}
MessageFeed.GetStatusUpdatesForUserPage = function (targetid, onSuccess, onFailed, userContext)
{
	/// <param name="targetid" type="Number">System.Int32</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetStatusUpdatesForUserPage(targetid, onSuccess, onFailed, userContext);
}
MessageFeed.GetFriends = function (onSuccess, onFailed, userContext)
{
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetFriends(onSuccess, onFailed, userContext);
}
MessageFeed.GetUserDetails = function (profileid, onSuccess, onFailed, userContext)
{
	/// <param name="profileid" type="Number">System.Int32</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetUserDetails(profileid, onSuccess, onFailed, userContext);
}
MessageFeed.PostStatusMessage = function (messageBody, position, onSuccess, onFailed, userContext)
{
	/// <param name="messageBody" type="String">System.String</param>
	/// <param name="position" type="GeoLocation">NexusWeb.Services.DataContracts.GeoLocation</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.PostStatusMessage(messageBody, position, onSuccess, onFailed, userContext);
}
MessageFeed.GetUserImage = function (userid, size, onSuccess, onFailed, userContext)
{
	/// <param name="userid" type="Number">System.Int32</param>
	/// <param name="size" type="NexusWeb.Services.PhotoSize">NexusWeb.Services.PhotoSize</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetUserImage(userid, size, onSuccess, onFailed, userContext);
}
MessageFeed.GetUserMap = function (userid, onSuccess, onFailed, userContext)
{
	/// <param name="userid" type="Number">System.Int32</param>
	/// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
	/// <param name="userContext" optional="true" mayBeNull="true"></param>
	MessageFeed._staticInstance.GetUserMap(userid, onSuccess, onFailed, userContext);
}
var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
if (typeof (ClientArticleUpdate) === 'undefined')
{
	var ClientArticleUpdate = gtc("ClientArticleUpdate");
	ClientArticleUpdate.registerClass('ClientArticleUpdate');
}
if (typeof (ClientArticleComment) === 'undefined')
{
	var ClientArticleComment = gtc("ClientArticleComment");
	ClientArticleComment.registerClass('ClientArticleComment');
}
if (typeof (ClientStatusUpdate) === 'undefined')
{
	var ClientStatusUpdate = gtc("ClientStatusUpdate");
	ClientStatusUpdate.registerClass('ClientStatusUpdate');
}
if (typeof (GeoLocation) === 'undefined')
{
	var GeoLocation = gtc("GeoLocation");
	GeoLocation.registerClass('GeoLocation');
}
Type.registerNamespace('NexusWeb.Services.DataContracts');
if (typeof (NexusWeb.Services.DataContracts.GeoCity) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoCity = gtc("GeoCity:http://schemas.datacontract.org/2004/07/NexusWeb.Services.DataContracts");
	NexusWeb.Services.DataContracts.GeoCity.registerClass('NexusWeb.Services.DataContracts.GeoCity');
}
if (typeof (NexusWeb.Services.DataContracts.GeoLevel1) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoLevel1 = gtc("GeoLevel1:http://schemas.datacontract.org/2004/07/NexusWeb.Services.DataContracts");
	NexusWeb.Services.DataContracts.GeoLevel1.registerClass('NexusWeb.Services.DataContracts.GeoLevel1');
}
if (typeof (NexusWeb.Services.DataContracts.GeoLevel2) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoLevel2 = gtc("GeoLevel2:http://schemas.datacontract.org/2004/07/NexusWeb.Services.DataContracts");
	NexusWeb.Services.DataContracts.GeoLevel2.registerClass('NexusWeb.Services.DataContracts.GeoLevel2');
}
if (typeof (NexusWeb.Services.DataContracts.GeoCountry) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoCountry = gtc("GeoCountry:http://schemas.datacontract.org/2004/07/NexusWeb.Services.DataContracts");
	NexusWeb.Services.DataContracts.GeoCountry.registerClass('NexusWeb.Services.DataContracts.GeoCountry');
}
if (typeof (UserDetails) === 'undefined')
{
	var UserDetails = gtc("UserDetails");
	UserDetails.registerClass('UserDetails');
}
Type.registerNamespace('System.IO');
if (typeof (System.IO.Stream) === 'undefined')
{
	System.IO.Stream = gtc("Stream:http://schemas.datacontract.org/2004/07/System.IO");
	System.IO.Stream.registerClass('System.IO.Stream');
}
Type.registerNamespace('System');
if (typeof (System.MarshalByRefObject) === 'undefined')
{
	System.MarshalByRefObject = gtc("MarshalByRefObject:http://schemas.datacontract.org/2004/07/System");
	System.MarshalByRefObject.registerClass('System.MarshalByRefObject');
}
if (typeof (NexusWeb.Services.DataContracts.GeoLevel1Type) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoLevel1Type = function () { throw Error.invalidOperation(); }
	NexusWeb.Services.DataContracts.GeoLevel1Type.prototype = { Unknown: 0, Other: 1, Province: 2, Region: 3, District: 4, State: 5 }
	NexusWeb.Services.DataContracts.GeoLevel1Type.registerEnum('NexusWeb.Services.DataContracts.GeoLevel1Type', true);
}
if (typeof (NexusWeb.Services.DataContracts.GeoLevel2Type) === 'undefined')
{
	NexusWeb.Services.DataContracts.GeoLevel2Type = function () { throw Error.invalidOperation(); }
	NexusWeb.Services.DataContracts.GeoLevel2Type.prototype = { Unknown: 0, Other: 1, County: 2, Census_Division: 3, Parish: 4, Water_Body: 5, Borough: 6 }
	NexusWeb.Services.DataContracts.GeoLevel2Type.registerEnum('NexusWeb.Services.DataContracts.GeoLevel2Type', true);
}
Type.registerNamespace('NexusWeb.Services');
if (typeof (NexusWeb.Services.PhotoSize) === 'undefined')
{
	NexusWeb.Services.PhotoSize = function () { throw Error.invalidOperation(); }
	NexusWeb.Services.PhotoSize.prototype = { Small: 32, Medium: 64, Large: 128 }
	NexusWeb.Services.PhotoSize.registerEnum('NexusWeb.Services.PhotoSize', true);
}

MessageFeed.