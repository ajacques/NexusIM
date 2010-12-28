<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="NexusWeb.Pages.MainPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Buddy List</title>
    <script type="text/javascript">
    	function slerror(sender, args)
    	{
    		var appSource = "";
    		if (sender != null && sender != 0)
    		{
    			appSource = sender.getHost().Source;
    		}

    		var errorType = args.ErrorType;
    		var iErrorCode = args.ErrorCode;

    		if (errorType == "ImageError" || errorType == "MediaError")
    		{
    			return;
    		}

    		var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

    		errMsg += "Code: " + iErrorCode + "    \n";
    		errMsg += "Category: " + errorType + "       \n";
    		errMsg += "Message: " + args.ErrorMessage + "     \n";

    		if (errorType == "ParserError")
    		{
    			errMsg += "File: " + args.xamlFile + "     \n";
    			errMsg += "Line: " + args.lineNumber + "     \n";
    			errMsg += "Position: " + args.charPosition + "     \n";
    		}
    		else if (errorType == "RuntimeError")
    		{
    			if (args.lineNumber != 0)
    			{
    				errMsg += "Line: " + args.lineNumber + "     \n";
    				errMsg += "Position: " + args.charPosition + "     \n";
    			}
    			errMsg += "MethodName: " + args.methodName + "     \n";
    		}

    		throw new Error(errMsg);
    	}
    </script>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager runat="Server" ID="scriptmgr" EnablePartialRendering="true">
		<Scripts>
			<asp:ScriptReference Path="~/js/Silverlight.js" />
			<asp:ScriptReference Path="~/js/Silverlight.supportedUserAgent.js" />
			<asp:ScriptReference Path="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.js" />
			<asp:ScriptReference Name="MicrosoftAjax.js" Path="~/js/scripts/MicrosoftAjax.js" />
		</Scripts>
	</asp:ScriptManager>
		<div id="silverlightHost">
		<script type="text/javascript">
			Silverlight.createObject(
				"../ClientBin/SilverIM.xap",
				silverlightHost,
				"slPlugin",
				{
					width: "100%",
					height: "100%",
					background: "white",
					version: "3.0.4624.0"
				},
				{
					onError: slerror
				}
			);
		</script>
	</div>
	</form>
</body>
</html>