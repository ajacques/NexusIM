Type.registerNamespace("NewAccount");

NewAccount.chooseAccType = function()
{
	var result = $("input[id^=networktype]").filter("input[checked=true]").valueOf();
	switch (result)
	{
		case "facebook":
			{
				$("div[id=facebook]").css("display", "block");
			}
	}
}

NewAccount.facebookConnect = function()
{
	document.location = SocialNetworks.GetFacebookConnectUrl();
}

NewAccount.setup = function()
{
	$("input#popupaccept").attr("onclick", "javascript:NewAccount.chooseAccType();");
}