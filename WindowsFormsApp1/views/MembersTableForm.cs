﻿using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.views
{
    public partial class MembersTableForm : Form
    {
        private MemberController memberController;
        private SubscriptionController subscriptionController;
        private DataTable originalDataTable;
        public MembersTableForm()
        {
            InitializeComponent();
            memberController = new MemberController();
            subscriptionController = new SubscriptionController();
            LoadMembers();
        }

        private void LoadMembers()
        {
            originalDataTable = memberController.GetAllMembers();
            dataGridView1.DataSource = originalDataTable;
            AddButtonsToDataGridView();
        }

        private void AddButtonsToDataGridView()
        {
            // Add Edit and Delete buttons
            AddButtonColumn("Edit", "Edit");
            AddButtonColumn("Delete", "Delete");

            // Style the existing subscriptions_id column as clickable links
            DataGridViewColumn subscriptionsIdColumn = dataGridView1.Columns["subscriptions_id"];
            if (subscriptionsIdColumn != null)
            {
                subscriptionsIdColumn.DefaultCellStyle.NullValue = "Subscription Details";
                subscriptionsIdColumn.DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
                subscriptionsIdColumn.DefaultCellStyle.Font = new System.Drawing.Font(dataGridView1.DefaultCellStyle.Font, System.Drawing.FontStyle.Underline);
            }

            // Subscribe to CellContentClick event
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Create a filtered view of the original data table
                DataView dataView = originalDataTable.DefaultView;

                // Build a filter expression to match any column containing the search term
                StringBuilder filterExpression = new StringBuilder();
                bool firstColumn = true;

                foreach (DataColumn column in originalDataTable.Columns)
                {
                    if (!firstColumn)
                    {
                        filterExpression.Append(" OR ");
                    }

                    filterExpression.Append($"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchTerm}%'");
                    firstColumn = false;
                }

                // Apply the filter to the DataView
                dataView.RowFilter = filterExpression.ToString();

                // Update the DataGridView to display the filtered data
                dataGridView1.DataSource = dataView.ToTable(); // Convert DataView back to DataTable
            }
            else
            {
                // If search term is empty, reset the DataGridView to show the original data
                dataGridView1.DataSource = originalDataTable;
            }
        }
    }
}