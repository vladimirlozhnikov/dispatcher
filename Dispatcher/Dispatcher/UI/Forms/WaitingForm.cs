using System.Windows.Forms;

namespace Dispatcher.UI.Forms
{
    public partial class WaitingForm : Form
    {
        public string WaitingText
        {
            get
            {
                return waitingLabel.Text;
            }
            set
            {
                waitingLabel.Text = value;
            }
        }
        public WaitingForm()
        {
            InitializeComponent();
        }
    }
}
