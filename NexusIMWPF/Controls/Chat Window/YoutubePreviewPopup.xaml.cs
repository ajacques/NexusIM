using System;
using System.Globalization;
using System.IO;
using System.Linq;
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

		public void LoadVideoMetadata(string videoId)
		{
			mVideoId = videoId;
			
			DataCache cache = DataCache.Create(mConnectionString);
			VideoMetadata metadata = cache.VideoMetadata.FirstOrDefault(vm => vm.VideoId == videoId);

			Uri uri;
			AsyncCallback callback;
			if (metadata == null)
			{
				LoadingHint.Visibility = Visibility.Visible;
				uri = new Uri(String.Format(CultureInfo.InvariantCulture, mVideoStatsUrl, videoId));
				callback = new AsyncCallback(OnVideoStatsDl);
			} else {
				PopulateUIControls(metadata);
				uri = new Uri(String.Format(CultureInfo.InvariantCulture, mVideoEphemeralStatsUrl, videoId));
				callback = new AsyncCallback(OnTempStatsDl);
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.BeginGetResponse(callback, request);
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

			ProcessVideoStats(document.DocumentElement, videoData);
			ProcessTempStats(document.DocumentElement, videoData);
			
			Dispatcher.BeginInvoke(new PopulateDelegate(PopulateUIControls), videoData);

			HttpWebRequest thumbrequest = (HttpWebRequest)WebRequest.Create(String.Format(CultureInfo.InvariantCulture, mThumbailUrl, mVideoId));
			thumbrequest.BeginGetResponse(new AsyncCallback(OnThumbnailDownload), new object[] { videoData, thumbrequest });
		}
		private void OnTempStatsDl(IAsyncResult e)
		{
			HttpWebRequest request = (HttpWebRequest)e.AsyncState;
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);

			Stream responseStream = response.GetResponseStream();

			XmlDocument document = new XmlDocument();
			document.Load(responseStream);

			VideoMetadata videoData = new VideoMetadata();
			ProcessTempStats(document.DocumentElement, videoData);

			Dispatcher.BeginInvoke(new PopulateDelegate(PopulateTempControls), videoData);
		}
		private void OnThumbnailDownload(IAsyncResult e)
		{
			object[] arrArray = (object[])e.AsyncState;
			VideoMetadata videoData = (VideoMetadata)arrArray[0];
			HttpWebRequest request = (HttpWebRequest)arrArray[1];
			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(e);
			
			Stream thumbstream = response.GetResponseStream();
			byte[] thumbdata = new byte[response.ContentLength];

			thumbstream.Read(thumbdata, 0, thumbdata.Length);
			
			videoData.Thumbnail = thumbdata;
			Dispatcher.BeginInvoke(new GenericEvent(() => Thumbnail.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(videoData.Thumbnail) ));

			DataCache cache = DataCache.Create(mConnectionString);
			cache.VideoMetadata.InsertOnSubmit(videoData);
			cache.SubmitChanges();
		}

		private static void ProcessVideoStats(XmlElement root, VideoMetadata videoData)
		{
			XmlElement detailElem = root["media:group"];

			videoData.Title = root["title"].InnerText;
			videoData.Author = root["author"]["name"].InnerText;
			videoData.Description = detailElem["media:description"].InnerText;
			
			if (videoData.Description.Length > 350)
				videoData.Description = videoData.Description.Substring(0, 350); // Only use the first 350 characters due to DataCache.sdf schema sizing and UI sizes

			string duration = detailElem["yt:duration"].GetAttribute("seconds");
			try {
				videoData.Duration = Int32.Parse(duration, NumberStyles.None, CultureInfo.InvariantCulture);
			} catch (FormatException) {
				videoData.Duration = 0;
			}
		}
		private static void ProcessTempStats(XmlElement root, VideoMetadata videoData)
		{
			try {
				videoData.Views = Int64.Parse(root["yt:statistics"].GetAttribute("viewCount"), NumberStyles.None, CultureInfo.InvariantCulture);
			} catch (FormatException) {
				videoData.Views = -1;
			}

			try {
				XmlElement rating = root["yt:rating"];
				videoData.Likes = Int64.Parse(rating.GetAttribute("numLikes"), NumberStyles.None, CultureInfo.InvariantCulture);
				videoData.Dislikes = Int64.Parse(rating.GetAttribute("numDislikes"), NumberStyles.None, CultureInfo.InvariantCulture);
			} catch (FormatException) {}
		}
		/// <summary>
		/// Takes the input video data and updates the UI to show the video metadata
		/// </summary>
		/// <remarks>
		/// This method must be called from the UI Thread
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">You didn't call the method from the UI thread.</exception>
		/// <param name="videoData">Input data describing the video</param>
		private void PopulateUIControls(VideoMetadata videoData)
		{
			TimeSpan duration = TimeSpan.FromSeconds(videoData.Duration);
			if (duration.Hours > 1)
				Duration.Text = duration.ToString("h\\:mm\\:ss", CultureInfo.InstalledUICulture);
			else
				Duration.Text = duration.ToString("m\\:ss", CultureInfo.InstalledUICulture);

			if (videoData.Thumbnail != null)
				Thumbnail.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(videoData.Thumbnail);
			
			TitleBlock.Text = videoData.Title;
			UploaderBlock.Text = videoData.Author;
			DescriptionBlock.Text = videoData.Description;

			LoadingHint.Visibility = Visibility.Collapsed;

			PopulateTempControls(videoData);
		}
		/// <remarks>
		/// This method must be called from the UI Thread
		/// </remarks>
		private void PopulateTempControls(VideoMetadata videoData)
		{
			double totalRates = videoData.Likes + videoData.Dislikes;

			if (totalRates != 0)
			{
				LikesBar.Width = new GridLength(videoData.Likes / totalRates, GridUnitType.Star);
				DislikesBar.Width = new GridLength(videoData.Dislikes / totalRates, GridUnitType.Star);
			}

			ViewsBlock.Text = videoData.Views.ToString("N0", CultureInfo.InstalledUICulture);
		}

		private delegate void PopulateDelegate(VideoMetadata videoData);

		private const string mVideoStatsUrl = "http://gdata.youtube.com/feeds/api/videos/{0}?v=2&fields=title,author,media:group(media:description,yt:duration),yt:statistics,yt:rating";
		private const string mVideoEphemeralStatsUrl = "http://gdata.youtube.com/feeds/api/videos/{0}?v=2&fields=yt:statistics,yt:rating";
		private const string mThumbailUrl = "http://i.ytimg.com/vi/{0}/default.jpg";
		private string mVideoId;
		private const string mConnectionString = "Data Source=DataCache.sdf;Persist Security Info=False;";
	}
}