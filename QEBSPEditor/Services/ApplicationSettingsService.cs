namespace QEBSPEditor.Services
{
    public class ApplicationSettingsService
    {
        public event EventHandler? Changed;

        private bool _testingMode = false;
        public bool TestingMode { get => _testingMode; set { _testingMode = value; Changed?.Invoke(this,EventArgs.Empty); } }



    }
}
