/// <reference path="http://code.jquery.com/jquery-1.4.1-vsdoc.js" />
/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.js" />
/// <reference path="http://dev.nexus-im.com/Services/ValidationFunctions.svc/jsdebug" />
/// <reference path="http://dev.nexus-im.com/Services/Accounts.svc/jsdebug" />

Type.registerNamespace("LoginPage");

LoginPage.Redirect = function()
{
	var regex = new RegExp("[\\?&]redirect=([^&#]*)");
	var result = regex.exec(window.location.href);
	if (result == null)
	{
		window.location.href = "/newsfeed.aspx";
	} else {
		window.location.href = result[1];
	}
}

LoginPage.DoLogin = function()
{
	var username = $("input#txtUsername").val();

	if (username == "")
	{
		$("#txtUsername").addClass("badinput");
		return;
	} else
		$("#txtUsername").removeClass("badinput");

	if ($("input#txtPassword").val() == "")
	{
		$("#txtPassword").addClass("badinput");
		return;
	} else
		$("#txtPassword").removeClass("badinput");

	var password = Crypto.SHA256($("input#txtPassword").val());

	$("#logininprogress").fadeIn('normal');
	AccountService.LoginHashedPassword(username, password, function (data, data2)
	{
		// Success! Now go redirect the user
		LoginPage.Redirect();
	}, function (data)
	{
		$("#logininprogress").fadeOut();
		$("#autherror").fadeIn();
		$("#txtUsername").addClass("badinput");
		$("#txtPassword").addClass("badinput");
	});
}

LoginPage.ValidateEmail = function()
{
	ValidationFunctions.UsernameInUse($("#txtEmail").val(), function(result)
	{
		if (result == true)
		{
			$("#registerEmailInUse").fadeIn();
			$("#txtEmail").addClass("badinput");
		} else {
			$("#registerEmailInUse").hide();
			$("#txtEmail").removeClass("badinput");
		}
	});
}

LoginPage.GetBirthday = function()
{
	var month = $("#dobMonth").val();
	var day = $("#dobDay").val();
	var year = $("#dobYear").val();

	return new Date(year, month, day, 0, 0, 0);
}

LoginPage.ValidateBirthday = function()
{
	var now = new Date();

	return (now - LoginPage.GetBirthday()) / 31536000000 > 16;
}

LoginPage.DoRegisterAccount = function()
{
	if (!LoginPage.ValidateBirthday())
	{
		$("#ageTooYoung").fadeIn();
		$("#dobSpan").addClass("badinput");
		return;
	} else {
		$("#ageTooYoung").hide();
		$("#dobSpan").removeClass("badinput");
	}
	$("#registerInProgress").fadeIn();

	AccountService.CreateAccount($("#txtFirstName").val(), $("#txtLastName").val(), $("#txtEmail").val(), $("#txtRegisterPassword").val(), $("#gender input:radio:checked").val(), LoginPage.GetBirthday(),
	function(result)
	{
		LoginPage.Redirect();
	}, function(data)
	{
		alert("something bad happened" + data);
	});
}