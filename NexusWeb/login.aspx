<%@ Page Language="C#" Title="NexusIM - Login" AutoEventWireup="true" MasterPageFile="centered.master"  CodeBehind="login.aspx.cs" Inherits="NexusWeb.Pages.Login" %>
<%@ OutputCache Duration="5000" Location="Any" VaryByParam="None" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.badinput
		{
			-moz-transition-delay: 0s, 0s;
			-moz-transition-duration: 0.2s, 0.2s;
			-moz-transition-property: border, -moz-box-shadow;
			-moz-transition-timing-function: linear, linear;
			-moz-box-shadow: 0 0 20px rgba(255, 85, 66, 0.85);
			-webkit-box-shadow: 0 0 20px rgba(255, 85, 66, 0.85);
			border: 2px solid rgba(255, 128, 66, 0.75);
			outline-color: -moz-use-text-color;
			outline-style: none;
			outline-width: thick;
		}
		.validatable
		{
			padding: 2px;
		}
		* 
		{
			font-family: 'Segoe UI', Tahoma, Verdana, Arial;
		}		
		.mainheader
		{
			font-size: 20pt;			
		}		
		label
		{
			margin-bottom: 0px;
			padding-top: 10px;
		}
		.errorhint
		{
			 font-size: 10pt;
			 color: #E00500;
			 display: none;
		}
		#dobMonth
		{
			padding-left: 7px;
		}
		
		#dobMonth option
		{
			padding-left: 7px;
		}
		
		button
		{
			padding: 5px;
		}
		
		input[type='text'], input[type='password']
		{
			-webkit-border-radius: 2px;
		}
	</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<table style="width: 100%">
	<tr valign="top">
		<td style="width: 50%">
			<span class="mainheader">new account</span><br />
			<div style="height: 25pt"></div>
			<label for="txtFirstName">First</label><label for="txtLastName" style="margin-left: 80px">Last</label><br />
			<input type="text" id="txtFirstName" placeholder="John" style="width: 100px" />
			<input type="text" id="txtLastName" placeholder="Smith" style="width: 100px" /><br />
			<label for="txtEmail">Email</label>&nbsp;<span id="registerEmailInUse" class="errorhint">Already in-use</span><br />
			<input type="text" class="validatable" id="txtEmail" placeholder="user@example.com" style="width: 210px" onblur="LoginPage.ValidateEmail();" /><br />
			<label for="txtRegisterPassword" style="width: 210px">Password</label><br />
			<input type="password" class="validatable" id="txtRegisterPassword" style="width: 210px" onkeyup="LoginPage.UpdatePassword();" /><br />
			<div id="registerPasswordStrength" style="height: 5px; margin: 0; padding: 0; width: 50px; background-color: Green;"></div>
			<input type="checkbox" id="showPwdChars" name="showPwdChars" onchange="LoginPage.TogglePasswordVisibility();" /><label for="showPwdChars" style="font-size: 10pt; margin-left: 5px;">Show characters</label><br />
			<label for="gender">Gender</label><br />
			<span id="gender">
				<input type="radio" name="gender" id="genderMale" value="2" /><label for="genderMale" style="margin-left: 2px">Male</label> 
				<input type="radio" name="gender" id="genderFemale" value="3" style="margin-left: 15px" /><label for="genderFemale" style="margin-left: 2px">Female</label><br />
			</span>
			<label>Birthday</label>&nbsp;<span id="ageTooYoung" class="errorhint">You must be at least 16 years old to register.</span><br />
			<span id="dobSpan" style="padding-bottom: 2px">
				<select id="dobMonth">
					<option value="1">January</option>
					<option value="2">February</option>
					<option value="3">March</option>
					<option value="4">April</option>
					<option value="5">May</option>
					<option value="6">June</option>
					<option value="7">July</option>
					<option value="8">August</option>
					<option value="9">September</option>
					<option value="10">October</option>
					<option value="11">November</option>
					<option value="12">December</option>
				</select>
				<input type="text" id="dobDay" maxlength="2" style="width: 20px; padding-left: 5px" placeholder="01" onkeyup="LoginPage.DobChange();" />
				<input type="text" id="dobYear" maxlength="4" style="width: 35px; padding-left: 4px;" placeholder="1989" onkeyup="LoginPage.DobChange();" />
			</span><br /><br />
			<input type="button" value="Register" onclick="LoginPage.DoRegisterAccount();" />&nbsp;<span id="registerInProgress" style="font-size: 10pt; color: #3333CC; display: none">Please Wait...</span>
		</td>
		<td style="width: 50%">
			<span class="mainheader">sign in</span>
			<div style="height: 25pt"><span id="autherror" class="errorhint">Failed to login. Please check your username and password.</span></div>
			<label for="txtUsername">Username</label><br />
			<input type="text" class="validatable" id="txtUsername" placeholder="user@example.com" style="width: 200px" /><br />
			<label for="txtPassword">Password</label><br />
			<input type="password" class="validatable" id="txtPassword" style="width: 200px" />&nbsp;<br />
			<a href="forgotpassword.aspx" style="font-size: 10pt; margin-bottom: 10px">Forgot your password?</a>
			<div style="height: 12pt"></div>
			<input type="checkbox" id="rememberMe" /><label for="rememberMe" style="font-size: 10pt; padding-left: 5px">Remember me</label><br /><br />
			<input id="loginButton" type="button" onclick="LoginPage.DoLogin();return false;" Value="Login" style="" />&nbsp;<span id="logininprogress" style="font-size: 10pt; color: #3333CC; display: none">Please Wait...</span>
		</td>
	</tr>
	</table>
</asp:Content>