using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Dental_Clinic.PL
{
    public partial class Appointment : Form
    {
        BL.Cls_Appointment appointment = new BL.Cls_Appointment();
        BL.Cls_Patient Paient = new BL.Cls_Patient();
        BL.Cls_Doctors Doctor = new BL.Cls_Doctors();
        bool Enable_BtnAdd = true;

        public Appointment()
        {
            InitializeComponent();
           Fill_ComPaient();
           Fill_Names_Doctors();
            FillDGV();
        }
        public void Fill_ComPaient()
        {
            
            Com_Paient.DataSource = Paient.Get_All_Patient();
            Com_Paient.DisplayMember = "Name";
            Com_Paient.ValueMember = "ID_P";
        }
        private void Fill_Names_Doctors()
        {
            Com_Doctors.DataSource = Doctor.Get_All_Doctors();
            Com_Doctors.DisplayMember = "Name_D";
            Com_Doctors.ValueMember = "Id_D";
        }
        private void FillDGV()
        {   

            DGV_Appointment.DataSource = appointment.Get_All_Appointment();

            DGV_Appointment.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Appointment.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Appointment.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV_Appointment.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            foreach (DataGridViewColumn column in DGV_Appointment.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            try{

            Boolean Status; 
            if (RBtn_Wait.Checked)
            {
               Status=true;
            }
            else
            {
                
               Status = false;
            }

            if (Com_Paient.Text == "اختيار" || Com_Doctors.Text == "اختيار" || Com_Type.Text == "اختيار"||string.IsNullOrEmpty(Com_Paient.Text))
            {
                MessageBox.Show("لا يمكن ترك بيانات الاسم فارغة ", "تحذير", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                return;
            }

            dt = appointment.Check_Appintment(Convert.ToInt32(Com_Paient.SelectedValue));
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show(" . تمت إضافة حجز لهذا المريض مسبقا  \n . قم بالمحاولة مرة اخرى بعد أن تتم معاينته ", "خطأ", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                return;
            }
          

            appointment.Add_Appointment(Com_Type.Text, Status, DTP_history.Value,  DTP_time.Value.TimeOfDay , Convert.ToInt32( Com_Paient.SelectedValue),Convert.ToInt32( Com_Doctors.SelectedValue));

            MessageBox.Show("تمت عملية الإضافة بنجاح", "عملية الإضافة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FillDGV();
            Btn_New_Click(sender, e);
        }
            catch(Exception ex){ }
        }

        private void Btn_New_Click(object sender, EventArgs e)
        {
            Com_Paient.DropDownStyle = ComboBoxStyle.DropDown;
            Btn_Add.Enabled = true;
            Btn_Update.Enabled = false;

            Com_Paient.ForeColor = Color.Gray;
            Com_Doctors.ForeColor = Color.Gray;
            Com_Paient.Text  = "اختيار";
            Com_Paient.Focus();

            Com_Doctors.Text = "اختيار";
            Com_Type.Text = "اختيار";


            DTP_time.Value = Convert.ToDateTime(DateTime.Now);
            DTP_history.MinDate = Convert.ToDateTime(DateTime.Now.ToString("dd / MM / yyyy"));

            DTP_history.Value = Convert.ToDateTime(DateTime.Now);
     
            RBtn_Wait.Checked = true;
            RBtn_Finished.Checked = false;
            DGV_Appointment.ClearSelection();

        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (DGV_Appointment.SelectedRows.Count > 0)
            if (MessageBox.Show(" تأكيد عملية الحذف ؟ ", "عملية الحذف ", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                appointment.Delete_Appointment(Int32.Parse(this.DGV_Appointment.CurrentRow.Cells[0].Value.ToString()));
                FillDGV();
                Btn_New_Click(sender, e);
            }
        }

        private void Btn_Update_Click(object sender, EventArgs e)
        {
            Boolean Status;
            if (RBtn_Wait.Checked)
            {
                Status = true;
            }
            else
            {

                Status = false;
            }

            if (Com_Paient.Text == "اختيار" || Com_Doctors.Text == "اختيار" || Com_Type.Text == "اختيار"||string.IsNullOrEmpty( Com_Paient.Text))
            {
                MessageBox.Show("لا يمكن ترك بيانات الاسم فارغة ", "تحذير", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                return;
            }
            DataTable dt = new DataTable();
            dt = appointment.Check_Appintment(Convert.ToInt32(Com_Paient.SelectedValue));
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show(" . تمت إضافة حجز لهذا المريض مسبقا  \n . قم بالمحاولة مرة اخرى بعد أن تتم معاينته ", "خطأ", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                return;
            }

            if (Status == true && DTP_history.Value < Convert.ToDateTime(DateTime.Now.ToString("dd / MM / yyyy")))
            {
                MessageBox.Show("        . الرجاء التحقق من خانة اختيار التاريخ  \n ! لأنه تم اختيار تاريخ أصغر من تاريخ اليوم  ", "اختيار التاريخ  ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                appointment.Update_Appointmet(Int32.Parse(DGV_Appointment.CurrentRow.Cells[0].Value.ToString()), Com_Type.Text, Status, DTP_history.Value, DTP_time.Value.TimeOfDay, Convert.ToInt32( Com_Paient.SelectedValue),Convert.ToInt32( Com_Doctors.SelectedValue));
                MessageBox.Show("تمت عملية التعديل بنجاح", "عملية التعديل ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            FillDGV();

            if (Enable_BtnAdd == false)
            {
                Btn_Add.Enabled = true;
            }
            Btn_Update.Enabled = false;

            Btn_New_Click(sender, e);
        }
            catch (Exception ex)
            {
            }


        }

        private void DGV_Appointment_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                if (DGV_Appointment.SelectedRows.Count == 0)
                    return;

                Com_Type.Text = this.DGV_Appointment.CurrentRow.Cells[3].Value.ToString();
                Com_Paient.Text = this.DGV_Appointment.CurrentRow.Cells[2].Value.ToString();

                bool Status = Convert.ToBoolean(this.DGV_Appointment.CurrentRow.Cells[4].Value.ToString());

                if (Status == true)
                {
                    RBtn_Wait.Checked = true;
                }
                else
                {
                    RBtn_Finished.Checked = true;
                }


                DTP_history.Value = Convert.ToDateTime(this.DGV_Appointment.CurrentRow.Cells[6].Value.ToString());
                DTP_time.Value = Convert.ToDateTime(this.DGV_Appointment.CurrentRow.Cells[7].Value.ToString());
                Com_Doctors.Text = this.DGV_Appointment.CurrentRow.Cells[5].Value.ToString();

                DGV_Appointment.ClearSelection();
                Btn_Update.Enabled = true;
                Enable_BtnAdd = false;
                Btn_Add.Enabled = false;

                Com_Doctors.ForeColor = Color.Black;
                Com_Paient.ForeColor = Color.Black;    
            }
            catch (Exception ex) { }
        }

        private void RText_Search_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt= appointment.Search_Appointment(RText_Search.Text);
            DGV_Appointment.DataSource = dt;
            DGV_Appointment.ClearSelection();

        }

        private void RadBtn_Day_CheckedChanged(object sender, EventArgs e)
        {        
                DataTable dt = new DataTable();
                dt = appointment.Search_BYDAy(DateTime.Now);
                DGV_Appointment.DataSource = dt;
                DGV_Appointment.ClearSelection();
        }

        private void RadBtn_History_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = appointment.Search_ByHistory(DTP1.Value, DTP2.Value);
            DGV_Appointment.DataSource = dt;
            DGV_Appointment.ClearSelection();

        }

        private void ChkBox_Appintment_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = appointment.Search_ByBooking(ChkBox_Appintment.Checked);
            DGV_Appointment.DataSource = dt;

            DGV_Appointment.ClearSelection();

        }

        private void Appointment_Load(object sender, EventArgs e)
        {

            Com_Paient.ResetText();
            Com_Paient.ForeColor = Color.Gray;
            Com_Paient.Text = "اختيار";
            Com_Paient.Focus();

            Com_Doctors.ResetText();
            Com_Doctors.ForeColor = Color.Gray;
            Com_Doctors.Text = "اختيار";
            DGV_Appointment.ClearSelection();

            DTP_history.MinDate = Convert.ToDateTime(DateTime.Now.ToString("dd / MM / yyyy"));

            timer1.Start();

        }

        private void Com_Paient_SelectedIndexChanged(object sender, EventArgs e)
        {
            Com_Paient.ForeColor = Color.Black;
            label8.Focus();

        }

        private void Com_Doctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            Com_Doctors.ForeColor = Color.Black;
            label7.Focus();
        }

        private void Appointment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Btn_New_Click(sender, e);
            }
        }

        private void DTP1_ValueChanged(object sender, EventArgs e)
        {
            if (RadBtn_History.Checked == true)
            {
                DataTable dt = new DataTable();
                dt = appointment.Search_ByHistory(DTP1.Value, DTP2.Value);
                DGV_Appointment.DataSource = dt;

                DGV_Appointment.ClearSelection();

            }
        }
        private void Com_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            label12.Focus();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Com_Doctors_Enter(object sender, EventArgs e)
        {
            Com_Paient.SelectionLength = 0;
        }

        private void Btn_Add_Enter(object sender, EventArgs e)
        {
            Com_Paient.SelectionLength = 0;
        }

        private void Com_Paient_KeyPress(object sender, KeyPressEventArgs e)
        {
            Com_Paient.DroppedDown = false;
           
        }

        private void Com_Doctors_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void DTP_history_ValueChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = appointment.Check_Booking();
                if (dt.Rows.Count > 0)
                    foreach (DataRow t in dt.Rows)
                    {

                        foreach (DataGridViewRow row in DGV_Appointment.Rows)
                        {
                            if (Convert.ToInt32(row.Cells[0].Value) == Convert.ToInt32(t[0]))
                            {
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                            }
                        }
                    }

            }
            catch (Exception ex) { }
        }

        private void Com_Paient_DropDownClosed(object sender, EventArgs e)
        {
            Com_Paient.DropDownStyle = ComboBoxStyle.DropDownList;

            Com_Paient.ForeColor = Color.Black;
            label1.Focus();
        }

        private void Panel_Hover_MouseEnter(object sender, EventArgs e)
        {
            Com_Paient.Focus();

        }

        private void Panel_Hover_Enter(object sender, EventArgs e)
        {

        }

        private void DGV_Appointment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

 
       
    }
}
