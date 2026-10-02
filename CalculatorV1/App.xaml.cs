using Octokit;

namespace CalculatorV1
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage());

#if WINDOWS
                        const int desiredWidth = 450;
                        const int desiredHeight = 560;
            
                        window.Width = desiredWidth;
                        window.Height = desiredHeight;
                        window.MaximumHeight = desiredHeight;
                        window.MaximumWidth = desiredWidth;
                        window.MinimumHeight = desiredHeight;
                        window.MinimumWidth = desiredWidth;
            
#endif
            return window;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await CheckForUpdates();
        }

        public async Task CheckForUpdates()
        {
            var github = new GitHubClient(new ProductHeaderValue("CalculatorV1Unpackaged"));
            var latestRelease = await github.Repository.Release.GetLatest("m07liu-1", "CalculatorV1Unpackaged");
            var latestVersion = latestRelease.TagName;
            var currentVersion = AppInfo.VersionString;

            if (latestVersion != currentVersion)
            {
                var updateMessage = $"A new version ({latestVersion}) is available. You are currently using version {currentVersion}.";
                var asset = latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".exe"));
                if (asset != null)
                {
                    await Launcher.Default.OpenAsync(asset.BrowserDownloadUrl);
                }
            }
        }
    }
}
