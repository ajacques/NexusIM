using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Linq;
using System.Diagnostics;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for YoutubePreviewPopup.xaml
	/// </summary>
	public partial class YoutubePreviewPopup : UserControl
	{
		public YoutubePreviewPopup()
		{
			InitializeComponent();
		}

		public void PopulateUIControls(string videoId)
		{
			mVideoId = videoId;
			
			DataCache cache = DataCache.Create(mConnectionString);
			VideoMetadata metadata = cache.VideoMetadata.FirstOrDefault(vm => vm.VideoId == videoId);

			if (metadata == null)
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mVideoStatsUrl + videoId);
				request.BeginGetResponse(new AsyncCallback(OnVideoStatsDl), request);
			} else
				PopulateUIControls(metadata);
		}

		private void OnVideoStatsDl(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			
			Stream responseStream = response.GetResponseStream();

			XmlDocument document = new XmlDocument();
			document.Load(responseStream);
			VideoMetadata videoData = new VideoMetadata();

			videoData.VideoId = mVideoId;
			videoData.Title = document.DocumentElement["title"].InnerText;
			videoData.Author = document.DocumentElement["author"]["name"].InnerText;
			videoData.Description = document.DocumentElement["content"].InnerText;

			try	{
				string duration = document.DocumentElement["media:group"]["yt:duration"].GetAttribute("seconds");
				videoData.Duration = Convert.ToInt32(duration);
			} catch {
				videoData.Duration = 0;
			}
			
			Dispatcher.BeginInvoke(new PopulateDelegate(PopulateUIControls), videoData);

			HttpWebRequest thumbrequest = (HttpWebRequest)WebRequest.Create(string.Format(mThumbailUrl, mVideoId));
			thumbrequest.BeginGetResponse(new AsyncCallback(OnThumbnailDownload), new object[] { videoData, thumbrequest });
		}
		private void OnThumbnailDownload(IAsyncResult e)
		{
			object[] arrArray = (object[])e.AsyncState;
			VideoMetadata videoData = (VideoMetadata)arrArray[0];
			HttpWebRequest request = (HttpWebRequest)arrArray[1];
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			
			Stream thumbstream = response.GetResponseStream();
			byte[] thumbdata = new byte[response.ContentLength];

			int bytesRead = thumbstream.Read(thumbdata, 0, thumbdata.Length);
			
			videoData.Thumbnail = thumbdata;
			Dispatcher.BeginInvoke(new GenericEvent(() => Thumbnail.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(videoData.Thumbnail) ));

			DataCache cache = DataCache.Create(mConnectionString);
			cache.VideoMetadata.InsertOnSubmit(videoData);
			cache.SubmitChanges();
		}

		private void PopulateUIControls(VideoMetadata videoData)
		{
			TimeSpan duration = TimeSpan.FromSeconds(videoData.Duration);
			if (duration.Hours > 1)
				Duration.Text = duration.ToString("h\\:mm\\:ss");
			else
				Duration.Text = duration.ToString("m\\:ss");

			if (videoData.Thumbnail != null)
				Thumbnail.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(videoData.Thumbnail);
			
			TitleBlock.Text = videoData.Title;
			UploaderBlock.Text = videoData.Author;
			DescriptionBlock.Text = videoData.Description;

			LoadingHint.Visibility = Visibility.Collapsed;
		}

		private delegate void PopulateDelegate(VideoMetadata videoData);

		private const string mVideoStatsUrl = "http://gdata.youtube.com/feeds/api/videos/";
		private const string mThumbailUrl = "http://i.ytimg.com/vi/{0}/default.jpg";
		private string mVideoId;
		private const string mConnectionString = "Data Source=DataCache.sdf;Persist Security Info=False;";
	}
}