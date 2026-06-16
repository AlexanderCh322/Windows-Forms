using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WindowsFormsApp1.DataAccess; 

namespace WindowsFormsApp1.Forms
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        void LoadUsers()
        {
            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT user_id, login, password, role, is_blocked FROM Users", conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                usersDataGridView.DataSource = table;
            }
        }

        private void addButton_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text)) return;
            if (roleComboBox.SelectedIndex == -1) return;

            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE login = @login", conn);
                cmd.Parameters.AddWithValue("@login", loginTextBox.Text);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    MessageBox.Show("Пользователь с указанным логином уже существует");
                    return;
                }

                cmd.CommandText = "INSERT INTO Users (login, password, role) VALUES (@login, @password, @role)";
                cmd.Parameters.AddWithValue("@password", passwordTextBox.Text);
                cmd.Parameters.AddWithValue("@role", roleComboBox.Text);
                cmd.ExecuteNonQuery();
            }
            LoadUsers();
        }

        private void editButton_Click_1(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0) return;
            string login = usersDataGridView.SelectedRows[0].Cells["login"].Value.ToString();

            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET password = @password, role = @role WHERE login = @login", conn);
                cmd.Parameters.AddWithValue("@password", passwordTextBox.Text);
                cmd.Parameters.AddWithValue("@role", roleComboBox.Text);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.ExecuteNonQuery();
            }
            LoadUsers();
        }

        private void unlockButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0) return;
            string login = usersDataGridView.SelectedRows[0].Cells["login"].Value.ToString();

            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET is_blocked = 0, failed_attempts = 0 WHERE login = @login", conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.ExecuteNonQuery();
            }
            LoadUsers();
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void AdminForm_Load(object sender, EventArgs e)
        {

        }
    }
}