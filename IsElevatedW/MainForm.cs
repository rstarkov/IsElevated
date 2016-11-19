using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;

namespace IsElevated
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                lblAdmin.Text = $"Administrator: YES";
                lblAdmin.ForeColor = Color.Maroon;
            }
            else
            {
                lblAdmin.Text = $"Administrator: NO";
                lblAdmin.ForeColor = Color.Green;
            }

            if (ProcessSecurity.GetProcessElevated())
            {
                lblElevated.Text = $"Elevated: YES";
                lblElevated.ForeColor = Color.Maroon;
            }
            else
            {
                lblElevated.Text = $"Elevated: NO";
                lblElevated.ForeColor = Color.Green;
            }

            switch (ProcessSecurity.GetCurrentProcessIntegrity())
            {
                case ProcessSecurity.Integrity.System:
                    lblIntegrity.Text = "Integrity: SYSTEM";
                    lblIntegrity.ForeColor = Color.Crimson;
                    break;
                case ProcessSecurity.Integrity.High:
                    lblIntegrity.Text = "Integrity: HIGH";
                    lblIntegrity.ForeColor = Color.Maroon;
                    break;
                case ProcessSecurity.Integrity.Medium:
                    lblIntegrity.Text = "Integrity: MEDIUM";
                    lblIntegrity.ForeColor = Color.Green;
                    break;
                case ProcessSecurity.Integrity.Low:
                    lblIntegrity.Text = "Integrity: LOW";
                    lblIntegrity.ForeColor = Color.LimeGreen;
                    break;
                default:
                    lblIntegrity.Text = "Integrity: ?";
                    lblIntegrity.ForeColor = Color.Black;
                    break;
            }
        }
    }
}
