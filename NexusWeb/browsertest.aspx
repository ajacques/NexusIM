<%@ Page Language="C#" %>
<html>
<head>
	<style>
	p { font-family:Tahoma, Sans-Serif; font-size:10pt; }
	td { font-family:Tahoma, Sans-Serif; font-size:8pt; padding-left:10; }
	tr { background-color:#F7F7F7; }
	.title { color:#396B9C; font-weight:bold; }
	.header { background-color:#CCDDEE; }
	</style>
</head>
<body>

	<p class="title">Browser Version:</p>
	
	<table width="90%" border="0" align="center"  cellpadding="2" cellspacing="2">
		<tr class="header">
			<td width="30%">Property</td>
			<td>Value</td>
		</tr>
		<tr>
			<td>Browser </td>
			<td> <%= Request.Browser.Browser %></td>
		</tr>
		<tr>
			<td>MajorVersion </td>
			<td> <%= Request.Browser.MajorVersion %></td>
		</tr>
		<tr>
			<td>MinorVersion </td>
			<td> <%= Request.Browser.MinorVersion %></td>
		</tr>
		<tr>
			<td>Platform </td>
			<td> <%= Request.Browser.Platform %></td>
		</tr>
		<tr>
			<td>Type </td>
			<td> <%= Request.Browser.Type %></td>
		</tr>
		<tr>
			<td>Version </td>
			<td> <%= Request.Browser.Version %></td>
		</tr>
		<tr>
			<td>Beta </td>
			<td> <%= Request.Browser.Beta %></td>
		</tr>
		<tr>
			<td>CLR Version</td>
			<td> <%= Request.Browser.ClrVersion %></td>
		</tr>
	</table>
	<p class="title">Device Specs:</p>
	<table width="90%" border="0" align="center"  cellpadding="2" cellspacing="2">
		<tr class="header">
			<td width="30%">Property</td>
			<td>Value</td>
		</tr>
		<% IDictionaryEnumerator enumerator = Request.Browser.Capabilities.GetEnumerator();
		while (enumerator.MoveNext())	
		{ %>
		<tr>
			<td><% Response.Write(enumerator.Key); %></td>
			<td><% Response.Write(enumerator.Value); %></td>
		</tr>
		<% } %>
	</table>

	<p class="title">Server Variables:</p>
	<table width="90%" border="0" align="center"  cellpadding="2" cellspacing="2">
	<% foreach (var variable in Request.ServerVariables)
	{%>
		<tr>
			<td><%= variable.ToString() %></td>
			<td><%= Request.ServerVariables[variable.ToString()] %></td>
		</tr>
	<% } %>
	</table>


</body>
</html>