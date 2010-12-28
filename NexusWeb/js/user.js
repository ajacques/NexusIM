Type.registerNamespace("User");

var LocationAllowed = false;
var LocRowId = 0;

User.loadData = function()
{
	MessageFeed.GetUserDetails(userid, function(data)
	{
		$("#laststatus").html(data.LastStatusUpdate.MessageBody);
		if (data.DateOfBirth)
		{
			$("#statCntrDobHead").css("display", "block");
			$("#statCntrBirthday").css("display", "block");
			$("#dateofbirth").html(jQuery.format(data.DateOfBirth, "MMMM d, yyyy"));
		}

		var one_year = 365 * 1000 * 60 * 60 * 24;
		var age = Math.floor((new Date() - data.DateOfBirth) / one_year);

		$("#statsAge").css("display", "block").html(String.Format(Strings.PluralAge, age));

		LocationAllowed = data.LocationAllowed;

		if (data.LocationAllowed)
		{
			$("#curlocStatContainer").css("display", "block");
			LocRowId = data.LocationId;

			CoreService.set_enableJsonp(true);

			User.UpdateLocation();
		}
	});
}

User.UpdateLocation = function()
{
	CoreService.GetLocation(LocRowId, function(data)
	{
		if (data.ReasonCode != null)
		{			
			if (data.ReasonType == "Location")
			{
				if (data.ReasonSubType == "Unavailable")
					$("#curlocation").html("Location Unavailable");
			}
		}
		$("#curlocation").html(data["ReverseGeocode"]);

		setTimeout("User.UpdateLocation();", 1000 * 60 * 10); // Delay: 10 Minutes
	});
}

function _StringFormatStatic()
{
	for (var i = 1; i < arguments.length; i++)
	{
		var exp = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
		arguments[0] = arguments[0].replace(exp, arguments[i]);
	}
	return arguments[0];
}
if (!String.Format)
{
	String.Format = _StringFormatStatic;
}

var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
Type.registerNamespace("NexusCore.Support");
if (typeof(NexusCore.Support.WCFWebPrettyFault) === 'undefined')
{
	NexusCore.Support.WCFWebPrettyFault = gtc("WCFWebPrettyFault:http://schemas.datacontract.org/2004/07/NexusCore.Support")
	NexusCore.Support.WCFWebPrettyFault.registerClass('NexusCore.Support.WCFWebPrettyFault');
}
