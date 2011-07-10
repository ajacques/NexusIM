using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for TLSCertificateDetails.xaml
	/// </summary>
	public partial class TLSCertificateDetails : Window
	{
		public TLSCertificateDetails()
		{
			InitializeComponent();
		}

		public void PopulateControls(X509Certificate2 certificate)
		{
			StringReader commonname = new StringReader(certificate.SubjectName.Decode(X500DistinguishedNameFlags.UseNewLines));

			// ex. C=US\r\nS=Michigan\r\nL=Marquette\r\nO=Adren Software\r\nOU=IRC\r\nCN=pub.nexus-im.com

			commonname.ReadLine();
			commonname.ReadLine();
			commonname.ReadLine();
			string organization = commonname.ReadLine();
			Organization.Text = organization.Substring(2);

			Unit.Text = commonname.ReadLine().Substring(3);
			CommonName.Text = commonname.ReadLine().Substring(3);

			IssueDate.Text = certificate.NotBefore.ToString(CultureInfo.InstalledUICulture);
			ExpireDate.Text = certificate.NotAfter.ToString(CultureInfo.InstalledUICulture);
		}
	}
}