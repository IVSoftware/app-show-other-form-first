using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app_with_login
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // A handle is needed before calling BeginInvoke, so get one.
            _ = Handle;
            // Call BeginInvoke on the new handle so as not to block the CTor.
            BeginInvoke(new Action(()=> execLoginFlow()));
            // Ensure final disposal of login form when app closes. Failure
            // to properly dispose of window may cause an exit hang.
            Disposed += (sender, e) => _loginForm.Dispose();
            // Provide a means of logging out once the main form shows.
            buttonSignOut.Click += (sender, e) => IsLoggedIn = false;
        }
        private LoginForm _loginForm = new LoginForm();

        private void execLoginFlow()
        {
            Visible = false;
            while (!IsLoggedIn)
            {
                _loginForm.StartPosition = FormStartPosition.CenterScreen;
                if (DialogResult.Cancel == _loginForm.ShowDialog(this))
                {
                    switch (MessageBox.Show(
                        this,
                        "Invalid Credentials",
                        "Error",
                        buttons: MessageBoxButtons.RetryCancel))
                    {
                        case DialogResult.Cancel: Application.Exit(); return;
                        case DialogResult.Retry: break;
                    }
                }
                else
                {
                    IsLoggedIn = true;
                }
            }
        }
        protected override void SetVisibleCore(bool value) =>
            base.SetVisibleCore(value && IsLoggedIn);

        bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (!Equals(_isLoggedIn, value))
                {
                    _isLoggedIn = value;
                    onIsLoggedInChanged();
                }
            }
        }

        private void onIsLoggedInChanged()
        {
            if (IsLoggedIn)
            {
                WindowState = FormWindowState.Maximized;
                Text = $"Main Form - AuthUser: {_loginForm.UserName}";
                Visible = true;
            }
            else execLoginFlow();
        }
    }
}
