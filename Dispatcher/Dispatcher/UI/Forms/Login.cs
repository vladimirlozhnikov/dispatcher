using System;
using System.Windows.Forms;

namespace Dispatcher.UI.Forms
{
    public partial class Login : Form
    {
        public string Phone { get; set; }
        public string Promo { get; set; }

        public Login()
        {
            InitializeComponent();
        }

        private void LoginButtonClick(object sender, EventArgs e)
        {
            Phone = phoneTextBox.Text;
            Promo = promoTextBox.Text;

            Close();
        }
    }
}
