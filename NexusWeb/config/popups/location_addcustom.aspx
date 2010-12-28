<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="location_addcustom.aspx.cs" Inherits="NexusWeb.SubPages.LocationAddCustom" %>
<form id="customlocationform" runat="server">
<div id="customlocationpopup">
	<table>
		<tr><td>Display Name:</td><td><asp:TextBox runat="Server" ID="displayname"/></td></tr>
		<tr><td>Service Type:</td><td>
			<asp:dropdownlist runat="server" ID="servicetype">
				<asp:ListItem Value="GoogleLatitude">Google Latitude</asp:ListItem>
			</asp:dropdownlist></td>
		</tr>
		<tr><td>Identifer:</td><td><asp:TextBox runat="Server" ID="identifier" /></td></tr>
	</table>
</div>
</form>
<script type="text/javascript">
function submit()
{
	$.ajax({
		url: "location_addcustom.aspx",
		type: "POST",
		data: ({
			displayname: $("input#displayname").val(),
			servicetype: $("select#servicetype").val(),
			identifer: $("input#identifier").val()
		}),
		success: function(data)
		{
			if (data == "yes")
			{
				$("div#popupcontent").html("Added!");
				$("div#popup").delay(2000).fadeOut(1000);
			}
		}
	});
}

$("input#popupaccept").bind("click", submit);
</script>