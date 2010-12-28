function validateUsername()
{
	var username = $("input#ctl00_body_username");   // This is stupid! why must ASP.NET put these stupid strings on the front of all my Ids?
	var service = new ValidationFunctions();
	service.UsernameInUse(username, function(result)
	{
		if (result == false)
		{
			$("img[id*=usernameValidationResult]").attr("src", "images/accept.png").attr("title", "Good job! This username is available");
		} else {
			$("img[id*=usernameValidationResult]").attr("src", "images/delete.png").attr("title", "Sorry, this username has been already taken");
		}

		$("img[id*=usernameValidationResult]").css("display", "block");
	}, null, null);
}

function validateAll()
{
	return true;
}