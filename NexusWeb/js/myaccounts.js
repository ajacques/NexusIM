Type.registerNamespace("MyAccounts");

MyAccounts.deleteAccount = function(id)
{
	openpopup("Delete Account", "popups/deleteaccount.aspx?id=" + id);
}

MyAccounts.editAccount = function(id)
{
	openpopup("Delete Account", "popups/editaccount.aspx?id=" + id);
}

MyAccounts.popupAccountEdit = function(id, username)
{
	$("tr[id$=account1]").children("td[class=accusername]").html(username);
}

MyAccounts.saveChanges = function()
{
	var states = $("input[id*=enableState]");
	states = states.filter(function(i, ndex)
	{
		var prevstate = ndex.parentElement.attributes["startstate"].nodeValue === "false" ? false : true;
		return (prevstate !== ndex.checked);
	});

	if (states.length == 0)
		return;

	var upload = new Array();

	states.each(function()
	{
		upload.push({ Key: this.parentElement.attributes["accountid"].nodeValue, Value: this.checked });
	});

	$("img#saveimg").css("display", "inline");

	var service = new AccountService();
	service.ChangeAccountEnabledStatus(upload, function (results, states)
	{
		states.each(function ()
		{
			this.parentElement.attributes["startstate"].nodeValue = this.parentElement.attributes["startstate"].nodeValue === "true" ? false : true; // same as !nodeValue
		});
		$("img#saveimg").css("display", "none");
		$("span#savedtext").css("display", "inline").delay(2000).fadeOut(1000);
	}, function () { }, states);
}

MyAccounts.revertChanges = function()
{
	var states = $("input[id*=enableState]");
	states = states.filter(function (i, ndex)
	{
		var prevstate = ndex.parentElement.attributes["startstate"].nodeValue === "false" ? false : true; // same as !nodeValue
		return (prevstate !== ndex.checked);
	}).each(function()
	{
		this.checked = this.parentElement.attributes["startstate"].nodeValue;
	});
}

MyAccounts.setupPage = function ()
{
	$("#ctl00_body_accounttable").tableDnD();
}