<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="demologin.aspx.cs" Inherits="NexusWeb.Pages.DemoLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		Please stand by while we login into your test account and redirect you to your destination.
		<asp:SqlDataSource ID="sessionstate" runat="server" ConnectionString="Data Source=HAL9001\SQLEXPRESS;Initial Catalog=SessionState;User ID=root;Password=bob"></asp:SqlDataSource>
    </div>
    </form>
</body>
</html>