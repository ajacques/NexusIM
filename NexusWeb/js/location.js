Type.registerNamespace("LocationConfig");

LocationConfig.requestperm = function()
{
	openpopup("Request Permission", "popups/location_requestperm.aspx");
}

LocationConfig.addcustom = function ()
{
	openpopup("Add Custom", "popups/location_addcustom.aspx");
	
}

LocationConfig.enableLocation = function ()
{
	AccountService.SetLocationShareState(true, function(data)
	{
		$("div.errormessage").fadeOut(1000);
	});
}

LocationConfig.deleteFriend = function(rowId)
{
	
}