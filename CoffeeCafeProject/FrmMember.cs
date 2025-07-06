using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMember : Form
    {
        public FrmMember()
        {
            InitializeComponent();
        }

        private void getAllMemberToListView()
        {
            //กำหนด Connect String เพื่อติดต่อฐานข้อมูล
            string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string strSQL = "SELECT memberId, memberPhone,memberName FROM member_tb";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQL เป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน DataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //ตั้งค่า ListView
                        lvShowAllMember.Items.Clear();
                        lvShowAllMember.Columns.Clear();
                        lvShowAllMember.FullRowSelect = true;
                        lvShowAllMember.View = View.Details;

                        lvShowAllMember.Columns.Add("รหัสสมาชิก", 100, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("เบอร์โทร", 150, HorizontalAlignment.Left);
                        lvShowAllMember.Columns.Add("ชื่อ", 100, HorizontalAlignment.Left);

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(dataRow["memberId"].ToString());
                            //เอารูปใส่ใน Item

                            // เอาแต่ละรายการใส่ใน Item
                            item.SubItems.Add(dataRow["memberPhone"].ToString());
                            item.SubItems.Add(dataRow["memberName"].ToString());

                            //เอาข้อมูลใน Item 
                            lvShowAllMember.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                }
            }
        }

        private void FrmMember_Load(object sender, EventArgs e)
        {
            getAllMemberToListView();
            tbMemberId.Clear();
            tbMemberPhone.Clear();
            tbMemberName.Clear();
        }

        private void tbMemberPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // อนุญาตให้กดปุ่มควบคุม เช่น Backspace
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // อนุญาตเฉพาะตัวเลข 0-9
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }

            // ถ้าไม่ใช่ตัวเลขหรือปุ่มควบคุม → ไม่อนุญาตให้พิมพ์
            e.Handled = true;
        }

        private void tbMemberName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // อนุญาตปุ่มควบคุม เช่น Backspace, Delete
            if (char.IsControl(e.KeyChar))
                return;

            // อนุญาตเว้นวรรค
            if (e.KeyChar == ' ')
                return;

            // อนุญาตตัวอักษรไทย + วรรณยุกต์ (ช่วง Unicode 0E00 - 0E7F)
            if (e.KeyChar >= 0x0E00 && e.KeyChar <= 0x0E7F)
                return;

            // อนุญาตตัวอักษรภาษาอังกฤษ A-Z, a-z
            if (char.IsLetter(e.KeyChar))
                return;

            // ถ้าไม่ผ่านเงื่อนไขใด ๆ → ไม่อนุญาต
            e.Handled = true;
        }

        //สร้างเมธอดแสดงข้อความเตือน
        private void ShowWarningMSG(string msg)
        {

            MessageBox.Show(msg, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            getAllMemberToListView();
            tbMemberId.Clear();
            tbMemberPhone.Clear();
            tbMemberName.Clear();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบเมนูหรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //ลบออกจาก database
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction(); //ใช้กับ Insert/update/delete

                        //คำสั่ง SQL
                        String strSql = "DELETE FROM member_tb WHERE memberId=@memberId";

                        //กำหนดค่าให้กับ SQL Parameter และสั่งให้คำสั่ง SQL ทำงาน
                        using (SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);

                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();


                            MessageBox.Show("ลบเรียบร้อย", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //อัปเดจ ListView และเคลียหน้าจอ
                            getAllMemberToListView();
                            tbMemberId.Clear();
                            tbMemberPhone.Clear();
                            tbMemberName.Clear();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                    }
                }
            }
        }

        private void lvShowAllMember_ItemActivate(object sender, EventArgs e)
        {
            //เอาข้อมูลของรายการที่เลือกไปแสดงที่หน้าจอ 
            tbMemberId.Text = lvShowAllMember.SelectedItems[0].SubItems[0].Text;
            tbMemberPhone.Text = lvShowAllMember.SelectedItems[0].SubItems[1].Text;
            tbMemberName.Text = lvShowAllMember.SelectedItems[0].SubItems[2].Text;

            btSave.Enabled = false;
            btUpdate.Enabled = true;
            btDelete.Enabled = true;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbMemberName.Text.Length == 0)
            {
                ShowWarningMSG("กรุณากรอกชื่อสมาชิก");
            }
            else if (tbMemberPhone.Text.Length == 0)
            {
                ShowWarningMSG("กรุณากรอกเบอร์โทร");
            }
            else
            {
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();


                        string strSQL = "INSERT INTO member_tb (memberPhone, memberName) " +
                                         "VALUES (@memberPhone, @memberName)";

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;

                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();


                            MessageBox.Show("บันทึกเรียบร้อย", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            getAllMemberToListView();
                            tbMemberId.Clear();
                            tbMemberName.Clear();
                            tbMemberPhone.Clear();
                            btSave.Enabled = true;
                            btUpdate.Enabled = false;
                            btDelete.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);

                    }
                }
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (tbMemberName.Text.Length == 0)
            {
                ShowWarningMSG("กรุณากรอกชื่อสมาชิก");
            }
            else if (tbMemberPhone.Text.Length == 0)
            {
                ShowWarningMSG("กรุณากรอกเบอร์โทร");
            }
            else
            {
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();


                        string strSQL = "UPDATE member_tb SET memberPhone = @memberPhone, memberName = @memberName WHERE memberId = @memberId";


                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;


                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();


                            MessageBox.Show("แก้ไขเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            getAllMemberToListView();
                            tbMemberId.Clear();
                            tbMemberName.Clear();
                            tbMemberPhone.Clear();
                            btSave.Enabled = true;
                            btUpdate.Enabled = false;
                            btDelete.Enabled = false;


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);

                    }
                }
            }
        }

        private void tbMemberName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
