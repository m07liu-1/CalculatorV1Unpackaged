namespace CalculatorV1
{
    public partial class App : Application {
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
    }
}
