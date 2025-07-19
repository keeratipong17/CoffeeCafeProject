﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMain : Form
    {
        //ตัวแปรเก็บราคาเมนู
        float[] menuPrice = new float[10];

        //ตัวแปรเก็บรหัสสมาฃิก
        int memberId = 0;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            FrmMenu frmMenu = new FrmMenu();
            frmMenu.ShowDialog();
            resetFrom();
        }

        private void btMember_Click(object sender, EventArgs e)
        {
            FrmMember frmMember = new FrmMember();
            frmMember.ShowDialog();
        }
        //เมธอต resetFrom
        private void resetFrom()
        {
            //se ค่า memberId เป็น 0
            memberId = 0;

            //ให้rdMemberNo rdMemberTes ไม่ถูกเลือก
            rdMenberNo.Checked = false;
            rdMemberYes.Checked = false;

            //ให้ tbMemerPhone ว่างและใช้งานไม่ได้
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;

            //ให้ tbMemberName เป็นข้อความ (ชื่อสมาชิก)
            tbMemberName.Text = "(ชื่อสมาชิก)";

            //ให้ tbMenberScore เป็น 0
            lbMemberScore.Text = "0";

            //ให้ lbOrderPay เป็๋น 0.00 
            lbOrderPay.Text = "0.00";

            //เคลียร์ lvorderMenu
            lvOrderMenu.Items.Clear();
            lvOrderMenu.Columns.Clear();
            lvOrderMenu.FullRowSelect = true;
            lvOrderMenu.View = View.Details;
            lvOrderMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
            lvOrderMenu.Columns.Add("ราคา", 80, HorizontalAlignment.Left);

            //ดึงข้อมูลเมนูมาจาก Datbase แสดงที่หน้าจอ และเก็บไว้ใช้กับตอนที่ผู้ใช้เลือกสั่งเมนู
            //สร้าง Connection ไปยังฐานข้อมูล
            using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
            {
                try
                {
                    //เปิดการเชื่อมต่อไปยังฐานข้อมูล
                    sqlConnection.Open();

                    //สร้างคำสั่ง SQL ในที่นี้คือ ดึงข้อมูลทั้งหมดจากตาราง menu_tb
                    string strSQL = "SELECT menuName, menuPrice, menuImage FROM menu_tb";

                    //จัดการให้ SQL ทำงาน
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        //เอาข้อมูลที่ได้จาก strSQL เป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน DataTable
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        //สร้างตัวแปรอาจถึง picturBox และ Botton ที่จะเอารูปและชื่อเมนูไปแสดง
                        PictureBox[] pbMenuImage = { pbMenu1, pbMenu2, pbMenu3, pbMenu4, pbMenu5, pbMenu6, pbMenu7, pbMenu8, pbMenu9, pbMenu10 };
                        Button[] btMenuName = { btMenu1, btMenu2, btMenu3, btMenu4, btMenu5, btMenu6, btMenu7, btMenu8, btMenu9, btMenu10 };

                        //เคลียร์ pbMenuImage และ btMenuName ก่อนที่จะใส่ลงไปใหม่
                        for (int i = 0; i < 10; i++)
                        {
                            pbMenuImage[i].Image = Properties.Resources.menu;
                            btMenuName[i].Text = "เพิ่มเมนู";
                        }

                        //วนลูปเอาข้อมูลที่อยู่ใน dataTable กำหนดให้กับ pbMenuImage, btMenuName, menPrice
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            btMenuName[i].Text = dataTable.Rows[i]["menuName"].ToString();
                            menuPrice[i] = float.Parse(dataTable.Rows[i]["menuPrice"].ToString());
                            //เอารูปไปกำหนดให้กับ pbMenuImage
                            if (dataTable.Rows[i]["menuImage"] != DBNull.Value)
                            {
                                //กรณีมีรูปต้องแปลงจาก Binary ให้เป็นรูป
                                byte[] imgByte = (byte[])dataTable.Rows[i]["menuImage"];
                                using (var ms = new System.IO.MemoryStream(imgByte))
                                {
                                    pbMenuImage[i].Image = System.Drawing.Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                //กรณีไม่มีรูป
                                pbMenuImage[i].Image = Properties.Resources.menu;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            resetFrom();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            resetFrom();
        }

        private void rdMenberNo_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
            memberId = 0;
        }

        private void rdMemberYes_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = true;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
        }

        private void tbMemberPhone_KeyUp(object sender, KeyEventArgs e)
        {//ตรวจสอบว่าปุ่มที่กดแล้วปล่อยใช้ปุ่ม Enter หรือไม่
            //ถ้าไม่ใช้ก็ไม่ต้องทำอะไร แต่ถ้าใช้ให้เอา เบอร์โทรไปค้นใน Database
            //แล้วชื่อกับแต้มมาโชว์ ส่วนรหัสเอาไว้ใช้บันทึกลง Database
            if (e.KeyCode == Keys.Enter)
            {
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open();
 
                        string strSQL = "SELECT memberId, memberName, memberScore FROM member_tb WHERE memberPhone=@memberPhone";
 
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
 
                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                //เอาข้อมูลที่ได้จาก strSQL เป็นก้อนใน dataAdapter มาทำให้เป็นตารางโดยใส่ไว้ใน DataTable
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable);
 
                                if (dataTable.Rows.Count == 1)
                                {
                                    tbMemberName.Text = dataTable.Rows[0]["memberName"].ToString();
                                    lbMemberScore.Text = dataTable.Rows[0]["memberScore"].ToString();
                                    memberId = int.Parse(dataTable.Rows[0]["memberId"].ToString());
                                }
                                else
                                {
                                    MessageBox.Show("เบอร์โทรนี้ไม่มีในระบบ กรุณาลองใหม่..!");
                                }
                            }
 
                        }
 
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);
                    }
                }
            }
        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu1.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu1.Text);
                item.SubItems.Add(menuPrice[0].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[0]).ToString();
            }
        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu2.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu2.Text);
                item.SubItems.Add(menuPrice[1].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[1]).ToString();
            }
        }

        private void btMenu3_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu3.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu3.Text);
                item.SubItems.Add(menuPrice[2].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[2]).ToString();
            }
        }

        private void btMenu4_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu4.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu4.Text);
                item.SubItems.Add(menuPrice[3].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString();
            }
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu5.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu5.Text);
                item.SubItems.Add(menuPrice[4].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[4]).ToString();
            }
        }

        private void btMenu6_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu6.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu6.Text);
                item.SubItems.Add(menuPrice[5].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[5]).ToString();
            }
        }

        private void btMenu7_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu7.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu7.Text);
                item.SubItems.Add(menuPrice[6].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[6]).ToString();
            }
        }

        private void btMenu8_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu8.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu8.Text);
                item.SubItems.Add(menuPrice[7].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[7]).ToString();
            }
        }

        private void btMenu9_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu9.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu9.Text);
                item.SubItems.Add(menuPrice[8].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[8]).ToString();
            }
        }

        private void btMenu10_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่าชื่อบนปุ่มเป็นคำว่า Menu หรือไม่ หากใช่ไม่ต้องทำอะไร 
            //หากไม่ใช่ ให้เอาชื่อเมนู กับราคาไปใส่ใน lvOrderMenu แล้วบวกแต้มเพิ่ม และบวกรวมเป็นเงินเพิ่ม
            if (btMenu10.Text != "Menu")
            {
                //เอาชื่อ ราคาใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu10.Text);
                item.SubItems.Add(menuPrice[9].ToString());
                lvOrderMenu.Items.Add(item);

                //บวกแต้มเพิ่ม ต้องตรวจสอบก่อนว่าเป็นสมาชิกไหม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                //บวกราคาเพิ่ม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[9]).ToString();
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            //ตรวจสอบว่ามีรายการสั่งซื้อหรือไม่

            if (lbOrderPay.Text == "0.00")

            {

                MessageBox.Show("กรุณาเลือกเมนูที่ต้องการสั่งซื้อก่อน..!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            else if (rdMemberYes.Checked != true && rdMenberNo.Checked != true)

            {

                MessageBox.Show("กรุณาเลือกสถานะสมาชิกก่อน..!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            else if (rdMemberYes.Checked == true && tbMemberName.Text == "(ชื่อสมาชิก)")

            {

                MessageBox.Show("กรุณาค้นหาสมาชิกก่อน..!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            else

            {

                //ผ่านตรวจสอบมาได้ต้องทำ 3 อย่าง

                //1. บันทึก order_th (INSERT INTO...)

                //2. บันทึก order_detail_tb (INSERT INTO...)

                //3. บันทึกแก้ไขแต้มคะแนนของสมาชิก member_tb กรณีสมาชิก (UPDATE...SET...)

                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))

                {

                    try

                    {

                        sqlConnection.Open();

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        //บันทึกลง Order_tb 

                        string strSQLOrder = "INSERT INTO order_tb (memberId, orderPay, createAt, updateAt) "

                                             + "VALUES (@memberId, @orderPay, @createAt, @updateAt); "

                                             + "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        //ตัวแปรเก็บ orderId

                        int orderId;

                        using (SqlCommand sqlCommand = new SqlCommand(strSQLOrder, sqlConnection, sqlTransaction))

                        {

                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;

                            sqlCommand.Parameters.Add("@orderPay", SqlDbType.Float).Value = float.Parse(lbOrderPay.Text);

                            sqlCommand.Parameters.Add("@createAt", SqlDbType.Date).Value = DateTime.Now;

                            sqlCommand.Parameters.Add("@updateAt", SqlDbType.Date).Value = DateTime.Now;


                            orderId = (int)sqlCommand.ExecuteScalar();

                        }

                        //บันทึกลง Order_detail_tb

                        foreach (ListViewItem item in lvOrderMenu.Items)

                        {

                            string strSQLOrderDetail = "INSERT INTO order_detail_tb (orderdetailId, menuName, menuPrice) "

                                                       + "VALUES (@orderdetailId, @menuName, @menuPrice)";

                            using (SqlCommand sqlCommand = new SqlCommand(strSQLOrderDetail, sqlConnection, sqlTransaction))

                            {

                                sqlCommand.Parameters.Add("@orderdetailId", SqlDbType.Int).Value = orderId;

                                sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 100).Value = item.SubItems[0].Text;

                                sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(item.SubItems[1].Text);

                                sqlCommand.ExecuteNonQuery();

                            }

                        }


                        //แก้ไข memberScore ที่ member_tb  

                        if (rdMemberYes.Checked == true)

                        {

                            string strEditScore = "UPDATE member_tb SET memberScore=@memberScore WHERE memberId=@memberId";

                            using (SqlCommand sqlCommand = new SqlCommand(strEditScore, sqlConnection, sqlTransaction))

                            {

                                sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = int.Parse(lbMemberScore.Text);

                                sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;

                                sqlCommand.ExecuteNonQuery();

                            }

                        }


                        //----------------------

                        sqlTransaction.Commit();

                        MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        resetFrom();

                    }

                    catch (SqlException ex)

                    {

                        MessageBox.Show("พบข้อผิดพลาด กรุณาลองใหม่หรือติดต่อ IT : " + ex.Message);

                    }

                }

            }

        }

        private void lvOrderMenu_ItemActivate(object sender, EventArgs e)
        {
            //ดับเบิ้ลคลิกที่รายการใน lvOrderMenu เพื่อลบรายการนั้นออก
            //เมื่อรายการถูกลบแล้วให้ลบแต้มสมาฃิก 1 แต้ม และจำนวนเงินก็ต้องลดลงด้วย
            if (lvOrderMenu.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvOrderMenu.SelectedItems[0];
                if (selectedItem != null)
                {
                    //ลบรายการที่เลือกออกจาก lvOrderMenu
                    lvOrderMenu.Items.Remove(selectedItem);

                    //ลบแต้มสมาชิก 1 แต้ม
                    if (tbMemberName.Text != "(ชื่อสมาชิก)")
                    {
                        lbMemberScore.Text = (int.Parse(lbMemberScore.Text) - 1).ToString();
                    }

                    //ลบราคาออกจาก lbOrderPay
                    lbOrderPay.Text = (float.Parse(lbOrderPay.Text) - float.Parse(selectedItem.SubItems[1].Text)).ToString();
                }
            }
        }
    }
}
