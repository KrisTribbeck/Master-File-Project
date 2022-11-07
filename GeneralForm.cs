﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;

namespace MasterFileProject
{
    public partial class GeneralForm : Form
    {
        public GeneralForm()
        {
            InitializeComponent();
        }
        // 4.1.	Create a Dictionary data structure with a TKey of type integer
        // and a TValue of type string, name the new structure “MasterFile”.
        public static Dictionary<int, string> MasterFile = new Dictionary<int, string>();
        // 4.2.	Create a method that will read the data from the .csv file
        // into the Dictionary data structure when the form loads.
        #region Form Load
        private void GeneralForm_Load(object sender, EventArgs e)
        {
            ReadStaffDetailsFromFile();
        }
        #endregion
        #region Read from cvs file
        private void ReadStaffDetailsFromFile()
        {
            var filePath = @"MalinStaffNamesV2.csv";

            if (File.Exists(filePath))
            {
                string[] staffData = File.ReadAllLines(filePath);
                foreach (var staff in staffData)
                {
                    var splitData = staff.Split(',');
                    if (splitData.Length > 0)
                    {
                        var staffID = splitData[0];
                        var staffName = splitData[1];
                        MasterFile.Add(int.Parse(staffID), staffName);
                    }
                }
                DisplayData(readOnlyListbox);
            }
            else
            {
                MessageBox.Show("File did not load.");
            }
        }
        #endregion
        // 4.3.	Create a method to display the Dictionary data into a non-selectable display only listbox (ie read only).
        // 4.8.	Create a method for the filtered and selectable listbox which will populate the two textboxes when a staff record is selected.
        #region Display Data
        private void DisplayData(ListBox listbox)
        {
            listbox.Items.Clear();
            foreach (var staff in MasterFile)
            {
                listbox.Items.Add(staff.Key + "\t" + staff.Value);
            }
        }
        #endregion
        // 4.4.	Create a method to filter the Staff Name data from the Dictionary into a second filtered and selectable listbox.
        // This method must use a textbox input and update as each character is entered. The listbox must reflect the filtered data in real time.
        #region Filter by Name
        private void FilterStaffName()
        {
            foreach (var staff in MasterFile)
            {
                if (!string.IsNullOrEmpty(staffNameTextbox.Text) && staff.Value.Contains(staffNameTextbox.Text))
                {
                    filteredListbox.Items.Add(staff.Key + "\t" + staff.Value);
                }
            }
        }
        private void staffNameTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            filteredListbox.Items.Clear();
            FilterStaffName();
        }
        #endregion
        // 4.5.	Create a method to filter the Staff ID data from the Dictionary into the second filtered and selectable list box.
        // This method must use a textbox input and update as each number is entered. The listbox must reflect the filtered data in real time.
        #region Filter by ID
        private void FilterStaffId()
        {
            foreach (var staff in MasterFile)
            {
                if (staff.Key.ToString().Contains(staffIdTextbox.Text) && string.IsNullOrEmpty(staffNameTextbox.Text))
                {
                    filteredListbox.Items.Add(staff.Key + "\t" + staff.Value);
                    // staffNameTextbox.TabStop = false;
                }
            }
        }
        private void staffIdTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            filteredListbox.Items.Clear();
            FilterStaffId();
        }
        #endregion
        // 4.6.	Create a method for the Staff Name textbox which will clear the contents
        // and place the focus into the Staff Name textbox. Utilise a keyboard shortcut.
        // 4.7.	Create a method for the Staff ID textbox which will clear the contents and place the focus into the textbox. Utilise a keyboard shortcut.
        // 4.9 Create modified logic to open the Admin Form to Create a new user when the Staff ID 77
        // and the Staff Name is empty. Read the appropriate criteria in the Admin Form for further information.
        #region General Form 
        private void GeneralForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Keyboard shortcut for Staff ID Textbox.
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.R)
            {
                ClearTextbox(staffIdTextbox);
            }
            // Keyboard shortcut for Staff Name Textbox.
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.D)
            {
                ClearTextbox(staffNameTextbox);
            }
            // 4.9.	Create a method that will open the Admin Form when the Alt + A keys are pressed. 
            // Ensure the General Form sends the currently selected Staff ID and Staff Name to the Admin Form
            // for Update and Delete purposes and is opened as modal.
            // Create modified logic to open the Admin Form to Create a new user when the Staff ID 77
            // and the Staff Name is empty. Read the appropriate criteria in the Admin Form for further information.
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.A)
            {
                // This is for edit and delete.
                if (!string.IsNullOrEmpty(staffIdTextbox.Text) && !string.IsNullOrEmpty(staffNameTextbox.Text))
                {
                    AdminForm adminForm = new AdminForm(staffIdTextbox.Text, staffNameTextbox.Text);
                    adminForm.ShowDialog();
                }
                // This is for create method.
                else if (!string.IsNullOrEmpty(staffIdTextbox.Text) && string.IsNullOrEmpty(staffNameTextbox.Text))
                {
                    if (ValidID(staffIdTextbox.Text))
                    {
                        AdminForm adminFrom = new AdminForm(staffIdTextbox.Text);
                        adminFrom.ShowDialog();
                    }
                    else
                    {
                        AdminForm adminFrom = new AdminForm(GenerateID());
                        adminFrom.ShowDialog();
                    }
                }
            }
            // Close General Form.
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Q)
            {
                this.Close();
            }
        }
        #endregion
        // 5.3.	Create a method that will create a new Staff ID and input the staff name from the related text box.
        // The Staff ID must be unique starting with 77xxxxxxx while the staff name may be duplicated. The new staff
        // member must be added to the Dictionary data structure.
        #region Generate Random ID
        private string GenerateID()
        {
            Random random = new Random();
            int digits = random.Next(9999999);
            string ukDigit = "77";
            string phoneNumber = string.Concat(ukDigit, digits);
            return phoneNumber;
        }
        #endregion
        // 4.9 Create modified logic to open the Admin Form to Create a new user when the Staff ID 77
        // and the Staff Name is empty. Read the appropriate criteria in the Admin Form for further information.
        #region Filtered Listbox Method
        // If user selects a staff from listbox and presses enter, the results are displayed 
        // in the Staff ID textbox and the Staff Name textbox.
        private void filteredListbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string curItem = filteredListbox.SelectedItem.ToString();
                var staffArray = curItem.Split('\t');
                staffIdTextbox.Text = staffArray[0];    
                staffNameTextbox.Text = staffArray[1];
            }
        }
        #endregion
        #region Utility Methods
        private void ClearTextbox(TextBox textbox)
        {
            textbox.Clear();
            textbox.Focus();
        }
        #endregion
        #region Check duplicates
        private bool ValidID(string checkThisID)
        {
            foreach (var staff in MasterFile)
            {
                if (!staff.Key.ToString().Contains(checkThisID) && staffIdTextbox.Text.StartsWith("77"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}


