namespace kafka_sufficiently_advanced_technology
{
    public partial class App : Application
    {
        private readonly MainPage _mainPage;

        public App(MainPage mainPage)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark;
            _mainPage = mainPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_mainPage)
            {
                Title         = "Kafka SAT",
                MinimumWidth  = 900,
                MinimumHeight = 600,
                Width         = 1280,
                Height        = 800,
            };
        }
    }
}
