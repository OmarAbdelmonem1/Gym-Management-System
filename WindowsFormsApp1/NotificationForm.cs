using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class NotificationForm : Form
    {
        private Timer timer;
        
          
        private Timer animationTimer;
        private int targetHeight;
        public event EventHandler NotificationClosed;


        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(12, 12);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(412, 114);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Notification Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMessage.Click += new System.EventHandler(this.lblMessage_Click);
            // 
            // NotificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(436, 135);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationForm";
            this.Opacity = 0.7;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
        public NotificationForm(string message)
        {
            
            InitializeComponent();
            lblMessage.Text = message;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = true;
            //this.BackColor = Color.White;
            //this.Opacity = 0;
            //Console.WriteLine(lblMessage.Text);

            //// Set initial size and target size for animation
            //this.Size = new Size(300, 0);
            //targetHeight = 100;

            // Timer to close the notification after a few seconds
            timer = new Timer();
            timer.Interval = 4000; // 5 seconds
            timer.Tick += Timer_Tick;

            // Timer for animation
            animationTimer = new Timer();
            animationTimer.Interval = 10; // Animation interval
            animationTimer.Tick += AnimationTimer_Tick;

          

            this.Load += NotificationForm_Load;
        }
        
        public void NotificationForm_Load(object sender, EventArgs e)
        {
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (this.Height < targetHeight)
            {
                this.Height += 10;
                this.Opacity += 0.1;
            }
            else
            {
                animationTimer.Stop();
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw rounded corners
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 20, 20, 180, 90);
            path.AddArc(this.Width - 20, 0, 20, 20, 270, 90);
            path.AddArc(this.Width - 20, this.Height - 20, 20, 20, 0, 90);
            path.AddArc(0, this.Height - 20, 20, 20, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);

            // Draw gradient background with the specified color
            Color startColor = Color.FromArgb(192, 80, 12); // Start color
            Color endColor = Color.FromArgb(192, 80, 12);   // End color (same as start for solid color)
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, startColor, endColor, 90F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }






        public void ShowNotification()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                screen.Width / 2 - this.Width / 2,
                screen.Height / 2 - this.Height / 2
            );
            this.Show();
        }
        
        private void lblMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
