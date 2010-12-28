function submit()
{
	var devicetype = $("input[id^=devicetype]").filter("input[checked=true]").val();
	var devicename = $("input[id$=devicename]").val();
	DeviceService.CreateDeviceDownloadLink(devicetype, devicename, function(data)
	{
		document.location = data;
		cancelPopup();
	});
}

$("input#popupaccept").val("Install").bind("click", submit);