<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="locationmap.aspx.cs" Inherits="NexusWeb.Pages.LocationMap" %>
<%@ OutputCache Duration="86400" Location="ServerAndClient" VaryByParam="None" %><!DOCTYPE html>
<html>
<head>
	<title>Contact Location Map</title>
	<link rel="Stylesheet" href="css/main.css" />
	<script type="text/javascript" src="js/Silverlight.supportedUserAgent.js"></script>
	<script type="text/javascript" src="js/Silverlight.debug.js"></script>
	<script type="text/javascript" src="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.js"></script>
	<script type="text/javascript">
	function onSLError(sender, args)
	{
		// Display error message.
		alert('ERROR');
	}
	function onSLLoad(sender, args)
	{
		$("#silverlightHost").height("auto");
	}
	function loadMapControl()
	{
		var height = (innerHeight - 33) + "px"
		Silverlight.createObjectEx({
			source: "/ClientBin/SilverlightContactMap.xap",
			parentElement: document.getElementById('silverlightHost'),
			id: "slPlugin",
			properties: {
				width: "100%",
				height: height,
				background: "white",
				alt: " ",
				version: "4.0.50917"
			},
			events: { onError: onSLError, onLoad: onSLLoad }
		});
		$("div#silverlightHost").height(height);
	}
	</script>
	<style type="text/css">
		#TopBar
		{
			padding: 5px;
			background-color: Black;
			vertical-align: middle;
			height: 20px;
		}
		#TopBar a
		{
			color: white;
		}
		#TopBar a:hover
		{
			text-decoration: underline;
		}
	</style>
</head>
<body style="margin: 0; height: 100%">
	<div id="TopBar"><a href="newsfeed.aspx">News Feed</a></div>
	<div id="slError" style="display: none; margin-left: auto; margin-right: auto; text-align: center; padding-top: 20%; height: 10px">
		<div id="slErrorDetail"></div>
		<a id="slInstallLink" href="http://go.microsoft.com/fwlink/?LinkId=149156" target="_blank">Install Silverlight now</a>
	</div>
	<div id="silverlightHost" style="height: 0px">
		<script type="text/javascript">
			function logger(data)
			{
				console.log(data);
			}
			if (!Silverlight.isInstalled("4"))
			{
				$("div#slError").show();
				if (navigator.platform == "Win64")
				{
					$("div#slErrorDetail").html("Silverlight does not currently support running in 64-bit browsers.<br />Please try again using a 32-bit browser.");
					$("a#slInstallLink").html("Install anyway");
				}
			} else {
				loadMapControl();
			}
		</script>
	</div>
</body>
</html>