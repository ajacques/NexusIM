/// <reference path="http://code.jquery.com/jquery-1.4.1-vsdoc.js" />
/// <reference path="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.js" />
/// <reference path="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0" />
/// <reference path="https://github.com/ded/script.js/blob/master/dist/script.js" />

var mapObj;

function getCircle(loc, radius)
{
	var R = 6371;
	var lat = (loc.Latitude * Math.PI) / 180;
	var lon = (loc.Longitude * Math.PI) / 180;
	var d = parseFloat(radius) / R;
	var locs = new Array();
	for (x = 0; x <= 360; x++)
	{
		var p2 = new Microsoft.Maps.Location(0, 0);
		brng = x * Math.PI / 180;
		p2.Latitude = Math.asin(Math.sin(lat) * Math.cos(d) + Math.cos(lat) * Math.sin(d) * Math.cos(brng));
		p2.Longitude = ((lon + Math.atan2(Math.sin(brng) * Math.sin(d) * Math.cos(lat), Math.cos(d) - Math.sin(lat) * Math.sin(p2.Latitude))) * 180) / Math.PI;
		p2.Latitude = (p2.Latitude * 180) / Math.PI;
		locs.push(p2);
	}
	return new Microsoft.Maps.Polygon(locs);
}

function ContactPoint(data)
{
	this.lat = data.Latitude;
	this.lon = data.Longitude;

	this.getLocation = function()
	{
		alert('hi');
		return new Microsoft.Maps.Location(lat, lon);
	}
}

function pinClick(point)
{
	
}

function dlComplete(data, textStatus, jgXHR)
{
	for (var location in data)
	{
		var point = data[location];
		var location = new Microsoft.Maps.Location(point.Latitude, point.Longitude);
		var pin = new Microsoft.Maps.Pushpin(location, {text: point.ServiceType });

		var pinInfobox = new Microsoft.Maps.Infobox(pin.getLocation(), 
				{title: 'My Pushpin', 
				 description: 'This pushpin is located at (0,0).', 
				 visible: false, 
				 offset: new Microsoft.Maps.Point(0,15)});

		//var radi = getCircle(point, 1000);
		Microsoft.Maps.Events.addHandler(pin, 'mouseover', pinClick);

		mapObj.entities.push(pin);
		mapObj.entities.push(pinInfobox);
		//mapObj.entities.push(radi);
	}
}

function login()
{
	$.ajax({
		url: "http://dev.nexus-im.com/Services/ContactMap.svc/All",
		dataType: 'json',
		crossDomain: true,
		success: dlComplete
	});
}

function loadJsMap()
{
	mapObj = new Microsoft.Maps.Map(document.getElementById("mapHost"), {credentials: "AsS2Xi9W1iyJWYJv67X3CnPtgqpQrITIGqlBqKBuSkRh0benHS3vkAaC2V-s_2nP"});
	recomputeHeight();

	login();
}

$script('http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0', function()
{
	window.setTimeout("loadJsMap();", 200);
});