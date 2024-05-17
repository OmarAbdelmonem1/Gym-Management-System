using System;
using System.Data;
using System.Windows.Forms;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.views
{
    public partial class SubscriptionDetailsForm : Form
    {
        private Label lblSubscriptionId;
        private Label lblType;
        private Label lblStartDate;
        private Label lblEndDate;
        private Label lblTotalPrice;

        public SubscriptionDetailsForm(Subscription subscription)
        {
            InitializeComponent();

            // Display subscription details in labels or textboxes
            lblSubscriptionId.Text = subscription.Id.ToString();
            lblType.Text = subscription.Name;
            lblStartDate.Text = subscription.StartDate.ToShortDateString();
            lblEndDate.Text = subscription.EndDate.ToShortDateString();
            lblTotalPrice.Text = subscription.TotalPrice.ToString("C");
        }

        private void InitializeComponent()
        {
            this.lblSubscriptionId = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSubscriptionId
            // 
            this.lblSubscriptionId.AutoSize = true;
            this.lblSubscriptionId.Location = new System.Drawing.Point(0, 0);
            this.lblSubscriptionId.Name = "lblSubscriptionId";
            this.lblSubscriptionId.Size = new System.Drawing.Size(42, 17);
            this.lblSubscriptionId.TabIndex = 0;
            this.lblSubscriptionId.Text = "label1";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(205, 140);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(42, 17);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "label2";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(282, 140);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(42, 17);
            this.lblStartDate.TabIndex = 2;
            this.lblStartDate.Text = "label3";
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(456, 163);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(42, 17);
            this.lblEndDate.TabIndex = 3;
            this.lblEndDate.Text = "label4";
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.AutoSize = true;
            this.lblTotalPrice.Location = new System.Drawing.Point(450, 72);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(42, 17);
            this.lblTotalPrice.TabIndex = 4;
            this.lblTotalPrice.Text = "label5";
            // 
            // SubscriptionDetailsForm
            // 
            this.ClientSize = new System.Drawing.Size(856, 468);
            this.Controls.Add(this.lblTotalPrice);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblSubscriptionId);
            this.Name = "SubscriptionDetailsForm";
            this.Load += new System.EventHandler(this.SubscriptionDetailsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void SubscriptionDetailsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
