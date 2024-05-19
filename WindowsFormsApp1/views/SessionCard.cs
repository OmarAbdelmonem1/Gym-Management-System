using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.views
{
    public partial class SessionCard : UserControl
    {
        private Session session;

        public SessionCard()
        {
            InitializeComponent();
        }

        public SessionCard(Session session)
        {
            this.session = session;
        }

        public void DisplaySession(Session session)
        {
            // Populate the controls with session details
            labelSessionName.Text = session.Name;
            labelCoachName.Text = session.Coach.Name;
            labelTime.Text = $"{session.StartTime} - {session.EndTime}";
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
