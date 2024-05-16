using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.views
{
    public partial class MembersTableForm : Form
    {
        private MemberController memberController;
        private SubscriptionController subscriptionController;
        private DataTable originalMembersTable; // Store the original members data

        public MembersTableForm()
        {
            InitializeComponent();
            memberController = new MemberController();
            subscriptionController = new SubscriptionController();
            LoadMembers();
        }

        private void LoadMembers()
        {
            originalMembersTable = memberController.GetAllMembers(); // Store the original members data
            dataGridView1.DataSource = originalMembersTable;
            AddButtonsToDataGridView();
        }

        private void AddButtonsToDataGridView()
        {
            // Add Edit and Delete buttons
            AddButtonColumn("Edit", "Edit");
            AddButtonColumn("Delete", "Delete");
          
        }

        private void AddButtonColumn(string name, string headerText)
        {
            if (!dataGridView1.Columns.Contains(name))
            {
                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.Name = name;
                buttonColumn.HeaderText = headerText;
                buttonColumn.Text = headerText;
                buttonColumn.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonColumn);
            }
        }

      
        

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

                if (columnName == "Edit")
                {
                    dataGridView1.BeginEdit(true);
                }
                else if (columnName == "Delete")
                {
                    int memberId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["MemberId"].Value);
                    DeleteMember(memberId);
                }
                else if (columnName == "subscriptions_id")
                {
                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells["subscriptions_id"];
                    if (cell.Value != null && cell.Value != DBNull.Value)
                    {
                        int subscriptionId = Convert.ToInt32(cell.Value);
                        ShowSubscriptionDetails(subscriptionId);
                    }
                    else
                    {
                        MessageBox.Show("Invalid subscription ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowSubscriptionDetails(int subscriptionId)
        {
            try
            {
                Subscription subscription = subscriptionController.GetSubscriptionDetails(subscriptionId);

                if (subscription != null)
                {
                    SubscriptionDetailsForm subscriptionForm = new SubscriptionDetailsForm(subscription);
                    subscriptionForm.ShowDialog(); // Display as modal dialog
                }
                else
                {
                    MessageBox.Show($"Subscription with ID {subscriptionId} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteMember(int memberId)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this member?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                memberController.DeleteMember(memberId);
                LoadMembers(); // Reload members after deletion
            }
        }
        

    }
}
