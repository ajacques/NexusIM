Type.registerNamespace("MyAccount");

MyAccount.toggleLocationState = function ()
{
	var state = $("input#locationstate").val() === "true" ? false : true;

	console.info("Updating LocationState (" + $("input#locationstate").val() + " --> " + state + ")");

	AccountService.SetLocationShareState(state, function (data)
	{
		if (state == true)
		{
			$("div[id*=locationstatus]").css("color", "green").text(Strings.LocationStatusEnabled);
			$("a[id$=locationtogglehref]").text(Strings.DisableText);
		} else {
			$("div[id*=locationstatus]").css("color", "red").text(Strings.LocationStatusDisabled);
			$("a[id$=locationtogglehref]").text(Strings.EnableText);
		}
	}, function (data)
	{
		alert(data);
	});
	$("a[id$=locationtogglehref]").val("" + state);
	$("inpup[id$=locationstate]").val(state);
}

MyAccount.statusChange = function()
{
	var state = $("select#accountState").val();
	AccountService.ChangeAllUsersProtocolStatuses(state);
}