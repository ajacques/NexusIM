function openpopup(title, contenturl)
{
	$.ajax({
		url: contenturl,
		success: function(data)
		{
			$("div#popupcontent").html(data);
			$("div#popup").css("display", "block");
			$("div#popuptitle").html(title);
		}
	});
	$("input[custom*=custom]").detach();
	$("div.popuptitle").corner("top")
	$("div.popupinner").corner()
}

function cancelPopup()
{
	$("div#popup").fadeOut("slow", function()
	{
		$("div#popuptitle").html("");
		$("div#popupcontent").html("");
		$("input#popupaccept").unbind("click");
	});
	//$("div#popup").css("display", "none");
}