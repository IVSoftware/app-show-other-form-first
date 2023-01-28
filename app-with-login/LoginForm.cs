using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app_with_login
{
    public partial class LoginForm : Form
    {

        public LoginForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            textBoxUid.Text = "Admin";
            textBoxUid.TextChanged += onOnyTextChanged;
            textBoxPswd.TextChanged += onOnyTextChanged;
            buttonLogin.Enabled = false;
            buttonLogin.Click += validateCredentials;
            textBoxPswd.KeyDown += (sender, e) =>
            {
                if(e.KeyData.Equals(Keys.Enter))
                {
                    e.Handled = e.SuppressKeyPress= true;
                    validateCredentials(textBoxPswd, EventArgs.Empty);
                }
            };
        }

        private async void validateCredentials(object sender, EventArgs e)
        {
            try
            {
                // Looks good to me!
                UseWaitCursor = true;
                Cursor= Cursors.WaitCursor;
                await Task.Delay(5000);
                DialogResult = DialogResult.OK;
            }
            finally
            {
                UseWaitCursor = false;
                Cursor = Cursors.Default;
            }
        }

        private void onOnyTextChanged(object sender, EventArgs e)
        {
            buttonLogin.Enabled = !(
                (textBoxUid.Text.Length == 0) || 
                (textBoxPswd.Text.Length == 0)
            );
        }
        public string UserName => textBoxUid.Text;
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                textBoxPswd.Clear();
                textBoxPswd.PlaceholderText = "********";
            }
        }
    }
}
