using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

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
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mVideoStatsUrl + videoId);
			request.BeginGetResponse(new AsyncCallback(OnVideoStatsDl), request);
		}

		private void OnVideoStatsDl(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			
			Stream responseStream = response.GetResponseStream();

			XmlDocument document = new XmlDocument();
			document.Load(responseStream);
			VideoMetadata videoData = new VideoMetadata();

			videoData.Title = document.DocumentElement["title"].InnerText;
			videoData.Author = document.DocumentElement["author"]["name"].InnerText;
			videoData.Description = document.DocumentElement["content"].InnerText;
			videoData.Thumbnail = (ImageSource)new ImageSourceConverter().ConvertFrom(string.Format(mThumbailUrl, mVideoId));

			try	{
				string duration = document.DocumentElement["media:group"]["yt:duration"].GetAttribute("seconds");
				videoData.Duration = TimeSpan.FromSeconds(Convert.ToInt32(duration));
			} catch {
				videoData.Duration = TimeSpan.Zero;
			}
			
			Dispatcher.BeginInvoke(new PopulateDelegate(PopulateUIControls), videoData);
		}

		private void PopulateUIControls(VideoMetadata videoData)
		{
			if (videoData.Duration.Hours > 1)
				Duration.Text = videoData.Duration.ToString("h\\:mm\\:ss");
			else
				Duration.Text = videoData.Duration.ToString("m\\:ss");

			Thumbnail.Source = videoData.Thumbnail;
			TitleBlock.Text = videoData.Title;
			UploaderBlock.Text = videoData.Author;
			DescriptionBlock.Text = videoData.Description;

			LoadingHint.Visibility = Visibility.Collapsed;
		}

		private struct VideoMetadata
		{
			public string Title;
			public string Author;
			public string Description;
			public TimeSpan Duration;
			public ImageSource Thumbnail;
		}

		private delegate void PopulateDelegate(VideoMetadata videoData);

		private const string mVideoStatsUrl = "http://gdata.youtube.com/feeds/api/videos/";
		private const string mThumbailUrl = "http://i.ytimg.com/vi/{0}/default.jpg";
		private string mVideoId;
	}
}