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
		window.location.href = "newsfeed.aspx";
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

	var password = $("input#txtPassword").val();

	$("#logininprogress").fadeIn('normal');
	AccountService.Login(username, password, function (data, data2)
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

LoginPage.TogglePasswordVisibility = function ()
{
	var pwd = $("input#txtRegisterPassword");
}

LoginPage.UpdatePassword = function ()
{
	var pwd = $("input#txtRegisterPassword");
	var meter = $("div#registerPasswordStrength");

	meter.css("width", ((pwd.length / 8.0) * 250) + "px");
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

function daysInMonth(month, year)
{
	var m = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
	if (month != 2)
		return m[month - 1];
	if (year%4 != 0)
		return m[1];
	if (year%100 == 0 && year%400 != 0)
		return m[1];
	return m[1] + 1;
}

LoginPage.DobChange = function()
{
	var txtMonth = parseInt($("#dobMonth").val());
	var txtDay = parseInt($("#dobDay").val());
	var txtYear = parseInt($("#dobYear").val());

	if (txtYear == NaN)
		return;

	var today = new Date();

	if (txtDay <= 0 || txtYear <= 1900 || txtYear > today.getFullYear())
	{
		$("#dobSpan").addClass("badinput");
		return;
	}

	var maxDay = daysInMonth(txtMonth, txtYear);

	if (txtDay > maxDay)
	{
		$("#dobSpan").addClass("badinput");
		return;
	}

	$("#dobSpan").removeClass("badinput");
}