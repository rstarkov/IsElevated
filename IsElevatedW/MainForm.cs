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

            if (ProcessIntegrity.GetProcessElevated())
            {
                lblElevated.Text = $"Elevated: YES";
                lblElevated.ForeColor = Color.Maroon;
            }
            else
            {
                lblElevated.Text = $"Elevated: NO";
                lblElevated.ForeColor = Color.Green;
            }

            switch (ProcessIntegrity.GetCurrentProcessIntegrity())
            {
                case ProcessIntegrity.Level.System:
                    lblIntegrity.Text = "Integrity: SYSTEM";
                    lblIntegrity.ForeColor = Color.Crimson;
                    break;
                case ProcessIntegrity.Level.High:
                    lblIntegrity.Text = "Integrity: HIGH";
                    lblIntegrity.ForeColor = Color.Maroon;
                    break;
                case ProcessIntegrity.Level.Medium:
                    lblIntegrity.Text = "Integrity: MEDIUM";
                    lblIntegrity.ForeColor = Color.Green;
                    break;
                case ProcessIntegrity.Level.Low:
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
