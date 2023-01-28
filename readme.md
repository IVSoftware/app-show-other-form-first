Reading your question carefully I ask myself _why would you want to do this?_ And sure! I can think of at least one scenario where the MainForm and the form you want to show _first_ are different. So, for purposes of demonstration, consider an "app that requires a login" as an example.

**App with Login**

[![screenshot][1]][1]

In **Program.cs** you'll still want to call `Application.Run(new MainForm())` even though in this case, it's the `LoginForm` that should present first. But here's a simple trick that ensures window handles will be created in the correct sequence (this can be adapted for your actual use case).

***
**Prevent MainForm from becoming Visible**

Override `SetVisibleCore` so that the `MainForm` can't be visible unless some condition is met, e.g. `IsLoggedIn`.

    public partial class MainForm : Form
    {
        .
        .
        .
        protected override void SetVisibleCore(bool value) =>
            base.SetVisibleCore(value && IsLoggedIn);
    }
***
**Ensure `MainForm.Handle` is created "anyway"**

In the CTor force the creation of the main form `Handle`. This ordinarily happens _later_ in the course of making the main form visible, but we're not going to _allow_ that. The main form `Handle` is needed _now_ and needs to be first in line.

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
***
This way, we can have it both ways and exec a login flow first but _without_ the login form trying to become the main window.

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

***
**Bind MainForm Visible to IsLoggedIn**

Since logins can come and go, just bind the visibility of `MainForm` to changes of the login state.

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
            Text = $"Welcome {_loginForm.UserName}";
            Visible = true;
        }
        else execLoginFlow();
    }


  [1]: https://i.stack.imgur.com/SMRpp.png