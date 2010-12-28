Type.registerNamespace("MyDevices");

MyDevices.explodeDevice = function (id)
{
	if ($("tr[id^=explodeDevice" + id + "]").length >= 1)
	{
		$("tr[id^=explodeDevice]").detach();
		return;
	}

	var connected = $("tr[id$=device" + id + "]").attr("connected") === "false" ? false : true;

	$("tr[id^=explodeDevice]").detach();
	var elem = document.createElement("tr");
	elem.setAttribute("id", "explodeDevice" + id);
	var majorcell = document.createElement("td");
	elem.appendChild(majorcell);

	// explodeDevice[id]
	//  |- table
	//      |- row1
	//         |- topcell1
	//         |- topcell2
	//            |- Disconnect/Reconnect
	//      |- row2
	//         |- cell0 - Spacer
	//         |- cell1 - Reinstall Link
	//         |- cell2 - Remove

	var table = document.createElement("table");
	var row1 = document.createElement("tr");
	var row2 = document.createElement("tr");

	majorcell.setAttribute("colspan", "2");
	majorcell.appendChild(table);

	// Row 1
	var topcell1 = document.createElement("td");
	var topcell2 = document.createElement("td");

	var disconnecthref = document.createElement("a");

	if (connected)
		disconnecthref.setAttribute("href", "javascript:MyDevices.disconnectDevice(" + id + ");");
	disconnecthref.textContent = "Disconnect";

	row1.appendChild(document.createElement("td"));
	topcell2.appendChild(disconnecthref);
	row1.appendChild(topcell1);
	row1.appendChild(topcell2);
	table.appendChild(row1);

	// Row 2
	var cell0 = document.createElement("td");
	var cell1 = document.createElement("td");
	var cell2 = document.createElement("td");
	cell1.setAttribute("width", "70");
	cell2.setAttribute("width", "70");
	row2.appendChild(cell0);
	row2.appendChild(cell1);
	row2.appendChild(cell2);
	table.appendChild(row2);

	// Cell 1
	var reinstallhref = document.createElement("a");
	reinstallhref.textContent = "Reinstall";
	cell1.appendChild(reinstallhref);

	// Cell 2
	var deletehref = document.createElement("a");
	deletehref.setAttribute("href", "javascript:MyDevices.removeDevice(" + id + ")");
	deletehref.textContent = "Remove";
	cell2.appendChild(deletehref);

	table.width = "100%";
	table.height = "100%"

	// Now we append it
	$("tr[id$=device" + id + "]").after(elem);
}

MyDevices.addDevice = function()
{
	openpopup("Add Device", "popups/newdevice.aspx");
}

MyDevices.disconnectDevice = function(id)
{
	var service = new DeviceService();
	service.DisconnectDevice(id);
}

MyDevices.removeDevice = function(id)
{
	var service = new DeviceService();
	service.DeleteDevice(id);
}

MyDevices.setupPage = function ()
{
	$("tr[id^=device]").hover(function (object)
	{
		if ($(this).data("selected") != "true")
			$(this).css("background-color", "#EEEEEE");
	}, function (object)
	{
		if ($(this).data("selected") != "true")
			$(this).css("background-color", "#ffffff");
	}).click(function ()
	{
		$("tr[id^=device]").each(function (obj)
		{
			$(this).css("background-color", "#ffffff");
		});
		$(this).css("background-color", "#D2E4FB").data("selected", "true");
	});
}