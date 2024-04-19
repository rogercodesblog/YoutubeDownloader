using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using static System.Net.WebRequestMethods;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        private string OutputDirectory;
        private string NormalizedTitle;
        YoutubeClient YoutubeClient;
        YoutubeExplode.Videos.Video Video;
        List<YoutubeExplode.Videos.Streams.MuxedStreamInfo> MuxedStreamInfo;
        public Form1()
        {
            InitializeComponent();
            SetOutputDirectory();
            YoutubeClient = new YoutubeClient();
        }

        private void btnGetVideoData_Click(object sender, EventArgs e)
        {
            GetVideoData(txtVideoUrl.Text);
        }

        private void btnDownloadThumbnail_Click(object sender, EventArgs e)
        {

        }

        private void btnDownloadVideo_Click(object sender, EventArgs e)
        {

        }

        //TODO: Add a CSV for saving configurations and a custom output directory
        private void SetOutputDirectory()
        {
            OutputDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private async void GetVideoData(string url)
        {
            btnGetVideoData.Enabled = false;
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("The url is cannot be empty", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Regex pattern provided by Regexr.com
            if (!Regex.IsMatch(url, "^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$"))
            {
                MessageBox.Show("The provided video Url is not valid", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Video = await YoutubeClient.Videos.GetAsync(url);

                NormalizedTitle = string.Join("_", Video.Title.Split(Path.GetInvalidFileNameChars()));

                var streamManifest = await YoutubeClient.Videos.Streams.GetManifestAsync(Video.Id);

                MuxedStreamInfo = streamManifest.GetMuxedStreams().OrderByDescending(video => video.VideoQuality).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error ocurred; \n {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            lblTitle.Text = NormalizedTitle;
            selectThumbnailQuality.DataSource = Video.Thumbnails;
            selectVideoQuality.DataSource = MuxedStreamInfo;
            imgVideoThumbnail.ImageLocation = Video.Thumbnails.FirstOrDefault().Url.Split("?").First();

            lblTitle.Visible = true;
            selectThumbnailQuality.Enabled = true;
            selectVideoQuality.Enabled = true;
            btnDownloadThumbnail.Enabled = true;
            btnDownloadVideo.Enabled = true;
            btnGetVideoData.Enabled = true;
        }


    }
}
