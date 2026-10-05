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
            await ScriptUpdate();
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

        public async Task ScriptUpdate() {
            var github = new GitHubClient(new ProductHeaderValue("CalculatorV1Unpackaged"));
            var latestRelease = await github.Repository.Release.GetLatest("m07liu-1", "CalculatorV1Unpackaged");
            var latestVersion = latestRelease.TagName;
            var currentVersion = AppInfo.VersionString;
            if (latestVersion != currentVersion)
            {
                var asset = latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".exe"));
                string tempFilePath = Path.Combine(FileSystem.Current.CacheDirectory, "update.exe");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("CalculatorV1Unpackaged");
                    if (asset != null)
                    {
                        var data = await client.GetByteArrayAsync(asset.BrowserDownloadUrl);
                        await File.WriteAllBytesAsync(tempFilePath, data);
                        string batchPath = Path.Combine(FileSystem.Current.CacheDirectory, "update.bat");
                        string currentExePath = Environment.ProcessPath;

                        string batchContent = $@"
                        @echo off
                        timeout /t 2 /nobreak > nul
                        copy /y ""{tempFilePath}"" ""{currentExePath}""
                        start """" ""{currentExePath}""
                        del ""%~f0""
                        ";

                        await File.WriteAllTextAsync(batchPath, batchContent);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = batchPath,
                            UseShellExecute = true,
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        });
                        Microsoft.Maui.Controls.Application.Current?.Quit();
                    }
                }

            }
        }
    }
}
