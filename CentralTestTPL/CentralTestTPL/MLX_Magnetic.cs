using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CentralTestTPL
{
    public partial class MLX_Magnetic : Form
    {
        public MLX_Magnetic()
        {
            InitializeComponent();
            fieldTSHAtxtbox1.Tag = fieldTSHAtxtbox2;
            fieldTSHAtxtbox2.Tag = fieldTSHAtxtbox3;
            fieldTSHAtxtbox3.Tag = fieldTSHAtxtbox4;
            fieldTSHAtxtbox4.Tag = fieldPHtxtbox1;
            fieldPHtxtbox1.Tag = fieldPHtxtbox2;
            fieldPHtxtbox2.Tag = fieldPHtxtbox3;
            fieldPHtxtbox3.Tag = fieldPHtxtbox4;
            fieldPHtxtbox4.Tag = fieldPHtxtbox5;
            fieldPHtxtbox5.Tag = fieldPHtxtbox6;
            fieldPHtxtbox6.Tag = fieldPHtxtbox7;
            fieldPHtxtbox7.Tag = fieldPHtxtbox8;
            fieldPHtxtbox8.Tag = null; // Last textbox

            fieldTSHAtxtbox1.KeyPress += FieldPH_KeyPress;
            fieldTSHAtxtbox2.KeyPress += FieldPH_KeyPress;
            fieldTSHAtxtbox3.KeyPress += FieldPH_KeyPress;
            fieldTSHAtxtbox4.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox1.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox2.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox3.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox4.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox5.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox6.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox7.KeyPress += FieldPH_KeyPress;
            fieldPHtxtbox8.KeyPress += FieldPH_KeyPress;
            TSHAtxtbox1.Focus();

        }

        private void ShowError(string message, TextBox txtbox)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtbox.Clear();
            txtbox.Enabled = true;
            txtbox.Focus();
        }

        private void MLX_Magnetic_Load(object sender, EventArgs e)
        {
            var list = new DataAccess().GetNonMagneticDetails();
            if (list.Count <= 0) {
                MessageBox.Show("No Magnetic Details Error Please Call IT", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                TSHAtxtbox1.Focus();
            }
        }

        private void TSHAtxtbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (TSHAtxtbox1.Text != Magnetic.TestHead1) {
                ShowError("Invalid Test Stand Head Assembly.\nPlease Scan again.", TSHAtxtbox1);
            } else {
                TSHAtxtbox1.Enabled = false;
                TSHAtxtbox2.Enabled = true;
                TSHAtxtbox2.Focus();
            }
        }

        private void TSHAtxtbox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (TSHAtxtbox2.Text != Magnetic.TestHead2)
            {
                ShowError("Invalid Test Stand Head Assembly.\nPlease Scan again.", TSHAtxtbox2);
            }
            else
            {
                TSHAtxtbox2.Enabled = false;
                TSHAtxtbox3.Enabled = true;
                TSHAtxtbox3.Focus();
            }
        }

        private void TSHAtxtbox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (TSHAtxtbox3.Text != Magnetic.TestHead3)
            {
                ShowError("Invalid Test Stand Head Assembly.\nPlease Scan again.", TSHAtxtbox3);
            }
            else
            {
                TSHAtxtbox3.Enabled = false;
                TSHAtxtbox4.Enabled = true;
                TSHAtxtbox4.Focus();
            }
        }

        private void TSHAtxtbox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (TSHAtxtbox4.Text != Magnetic.TestHead4)
            {
                ShowError("Invalid Test Stand Head Assembly.\nPlease Scan again.", TSHAtxtbox4);
            }
            else
            {
                TSHAtxtbox4.Enabled = false;
                PHtxtbox1.Enabled = true;
                PHtxtbox1.Focus();
            }
        }

        private void PHtxtbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (PHtxtbox1.Text != Magnetic.PickUpHead)
            {
                ShowError("Invalid Test Stand Head Assembly.\nPlease Scan again.", PHtxtbox1);
            }
            else
            {
                PHtxtbox1.Enabled = false;
                fieldTSHAtxtbox1.Enabled = true;
                fieldTSHAtxtbox1.Focus();
            }
        }




        //private void fieldTSHAtxtbox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldTSHAtxtbox2_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldTSHAtxtbox3_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldTSHAtxtbox4_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox2_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox3_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox4_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox5_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox6_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox7_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar != (char)Keys.Enter) return;

        //}

        //private void fieldPHtxtbox8_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    TextBox txt = (TextBox)sender;

        //    if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Back)
        //        return;

        //    if (e.KeyChar == '.')
        //    {
        //        if (txt.Text.Length == 0 || txt.Text.Contains("."))
        //            e.Handled = true;

        //        return;
        //    }

        //    if (!char.IsDigit(e.KeyChar))
        //    {
        //        e.Handled = true;
        //        return;
        //    }

        //    decimal value;
        //    if (decimal.TryParse(fieldPHtxtbox8.Text, out value) && value > 0.100m)
        //    {
        //        MessageBox.Show("Magnetic field measurements Failed, Check the Handler non-magnetic parts");
        //        fieldPHtxtbox8.Focus();
        //        fieldPHtxtbox8.SelectAll();
        //        e.Handled = true;
        //        return;
        //    }
        //}

        private void FieldPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            // Allow Enter and Backspace
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (e.KeyChar == (char)Keys.Enter)
            {
                decimal value;
                if (decimal.TryParse(txt.Text, out value) && value >= 0.100m || decimal.TryParse(txt.Text, out value) && value == 0)
                {
                    MessageBox.Show("Magnetic field measurements Failed, Check the Handler non-magnetic parts");
                    txt.Clear();
                }
                else
                {
                    txt.Enabled = false;

                    TextBox next = txt.Tag as TextBox;
                    if (next != null)
                    {
                        next.Enabled = true;
                        next.Focus();
                    }
                    else
                    {
                        bool success = new DataAccess().InsertNonMagneticDetails(TSHAtxtbox1.Text,
                                                                                 TSHAtxtbox2.Text,
                                                                                 TSHAtxtbox3.Text,
                                                                                 TSHAtxtbox4.Text,
                                                                                 PHtxtbox1.Text,
                                                                                 fieldTSHAtxtbox1.Text,
                                                                                 fieldTSHAtxtbox2.Text,
                                                                                 fieldTSHAtxtbox3.Text,
                                                                                 fieldTSHAtxtbox4.Text,
                                                                                 fieldPHtxtbox1.Text,
                                                                                 fieldPHtxtbox2.Text,
                                                                                 fieldPHtxtbox3.Text,
                                                                                 fieldPHtxtbox4.Text,
                                                                                 fieldPHtxtbox5.Text,
                                                                                 fieldPHtxtbox6.Text,
                                                                                 fieldPHtxtbox7.Text,
                                                                                 fieldPHtxtbox8.Text);
                        if (success) {
                            this.Hide();
                        }
                        else {
                            MessageBox.Show("Failed to insert logs.\nPlease Try Again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //new DataAccess().insertMasterLogs("Failed to insert logs. Application will not launch. ", LotInfo.LotAlias, LotInfo.Device, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());

                        }
                    }
                }

                e.Handled = true;
                return;
            }

            // Allow only one decimal point and not as the first character
            if (e.KeyChar == '.')
            {
                if (txt.Text.Length == 0 || txt.Text.Contains("."))
                    e.Handled = true;

                return;
            }
            // Allow digits only
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Validate the value after the current key is pressed
            //string newText = txt.Text.Insert(txt.SelectionStart, e.KeyChar.ToString());

            //decimal value;
            //if (decimal.TryParse(newText, out value) && value > 0.100m)
            //{
            //    MessageBox.Show("Magnetic field measurements Failed, Check the Handler non-magnetic parts");
            //    txt.Clear();
            //    txt.Focus();
            //    e.Handled = true;
            //}
            //else
            //{
            //    txt.Enabled = false;

            //    TextBox next = txt.Tag as TextBox;
            //    if (next != null)
            //    {
            //        next.Enabled = true;
            //        next.Focus();
            //    }
            //}

        }

    }
}
