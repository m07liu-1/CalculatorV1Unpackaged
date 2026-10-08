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
            await LaunchUpdate();
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

        public async Task ScriptUpdate()
        {
            // Removed early return check - downloads update on every check, not just if file exists
            try
            {
                var github = new GitHubClient(new ProductHeaderValue("CalculatorV1Unpackaged"));
                var latestRelease = await github.Repository.Release.GetLatest("m07liu-1", "CalculatorV1Unpackaged");
                var latestVersion = latestRelease.TagName;
                var currentVersion = AppInfo.VersionString;

                if (latestVersion != currentVersion)
                {
                    var asset = latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".exe"));
                    if (asset != null)
                    {
                        string tempFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "update.exe");
                        
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.UserAgent.ParseAdd("CalculatorV1");
                            
                            var response = await client.GetAsync(asset.BrowserDownloadUrl);
                            response.EnsureSuccessStatusCode();
                            
                            var data = await response.Content.ReadAsByteArrayAsync();
                            await File.WriteAllBytesAsync(tempFilePath, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        }

        public async Task LaunchUpdate()
        {
            var update = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "update.exe");
            if (!File.Exists(update))
            {
                return;
            }   
            try
            {
                
                // Get the executable name dynamically instead of hardcoding
                string executableName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "CalculatorV1.exe");
                string batPath = Path.Combine(Path.GetTempPath(), "update_launcher.bat");
                string targetExePath = Environment.ProcessPath;
                
                string batContent = $@"@echo off
timeout /t 2 /nobreak
taskkill /f /im {executableName} 2>nul
timeout /t 1 /nobreak
copy /y ""{update}"" ""{targetExePath}""
start """" ""{targetExePath}""
del ""{update}""
";

                await File.WriteAllTextAsync(batPath, batContent);

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                var process = System.Diagnostics.Process.Start(psi);
                if (process != null)
                {
                    // Increased delay to allow batch script to acquire file locks
                    await Task.Delay(1500);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to start update process");
                    return;
                }
                
                App.Current?.Quit();
            }
            catch (Exception ex)
            {
                // Log the error instead of silently failing
                System.Diagnostics.Debug.WriteLine($"Update launch failed: {ex.Message}");
            }
        }
    }
}