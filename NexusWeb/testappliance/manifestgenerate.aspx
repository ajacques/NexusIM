<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Security" %>

<%
	string setupkey = Request["setupkey"];
	string url = "http://localhost/testappliance/NexusCoreTestAppliance.application?setupkey=" + setupkey;
	//string args = String.Format("-u \"{0}\" -pu \"{1}\" -cf \"{2}\" -pwd \"qwerty\" -i false -t \"{3}\"", "NexusCoreTestAppliance.application", url, "key.pfx", "test.application");
    string args = String.Format("-u \"{0}\" -pu \"{1}\" -i false -t \"{2}\"", "NexusCoreTestAppliance.application", url, "test.application");
	ProcessStartInfo info = new ProcessStartInfo();
	info.FileName = Server.MapPath("mage.exe");
	info.CreateNoWindow = true;
	info.UseShellExecute = false;
	info.Arguments = args;
	info.RedirectStandardOutput = true;
	info.RedirectStandardError = true;
	info.WorkingDirectory = Server.MapPath(".");
	Process mage = new Process();
	mage.StartInfo = info;
	mage.Start();
	StreamWriter wr = new StreamWriter(Response.OutputStream);
	wr.WriteLine(mage.StandardOutput.ReadLine());
	wr.WriteLine(mage.StandardOutput.ReadLine());
	wr.Flush();
	mage.WaitForExit(1000);
%>