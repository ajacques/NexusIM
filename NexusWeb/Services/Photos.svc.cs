using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.IO;
using System.Drawing;
using NexusWeb.Services.DataContracts;
using System.Collections.Generic;
using System.Linq;
using NexusWeb.Databases;

namespace NexusWeb.Services
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PhotoService
	{
		public PhotoService()
		{
			db = new userdbDataContext();
		}

		public void UploadPhoto(Stream uploadstream)
		{
			Image bitmap = Image.FromStream(uploadstream);

		}

		[OperationContract]
		public AlbumDetails test()
		{
			return new AlbumDetails();
		}

		[OperationContract]
		public IEnumerable<AlbumDetails> GetUserAlbums(int userid)
		{
			throw new NotImplementedException();
		}

		private userdbDataContext db;
	}
}