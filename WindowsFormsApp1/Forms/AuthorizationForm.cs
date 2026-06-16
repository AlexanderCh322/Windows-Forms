using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DataAccess; 

namespace WindowsFormsApp1.Forms
{
    public partial class AuthorizationForm : Form
    {
        int[] order = { 0, 1, 2, 3 };
        PictureBox[] pbs;
        PictureBox selectedPb = null;

        public AuthorizationForm()
        {
            InitializeComponent();
            pbs = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };
            foreach (var pb in pbs) pb.Click += PictureBox_Click;
            Shuffle();
        }

        void Shuffle()
        {
            Random rnd = new Random();
            order = order.OrderBy(x => rnd.Next()).ToArray();
            UpdateImages();
        }
        void UpdateImages()
        {
            Image[] images = { Properties.Resources.Part1, Properties.Resources.Part2, Properties.Resources.Part3, Properties.Resources.Part4 };
            for (int i = 0; i < 4; i++) pbs[i].Image = images[order[i]];
        }

        void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPb = sender as PictureBox;
            if (selectedPb == null)
            {
                selectedPb = clickedPb;
                selectedPb.BorderStyle = BorderStyle.Fixed3D;
            }
            else
            {
                int i1 = Array.IndexOf(pbs, selectedPb);
                int i2 = Array.IndexOf(pbs, clickedPb);

                Image tempImg = selectedPb.Image;
                selectedPb.Image = clickedPb.Image;
                clickedPb.Image = tempImg;

                int tempOrder = order[i1];
                order[i1] = order[i2];
                order[i2] = tempOrder;

                selectedPb.BorderStyle = BorderStyle.None;
                selectedPb = null;
            }
        }

        bool IsCaptchaValid()
        {
            return order[0] == 0 && order[1] == 1 && order[2] == 2 && order[3] == 3;
        }

        private void Loginbutton_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LogintextBox.Text) || string.IsNullOrWhiteSpace(PasswordtextBox.Text))
            {
                MessageBox.Show("Поля Логин и Пароль обязательны для заполнения.");
                return;
            }

            if (!IsCaptchaValid())
            {
                MessageBox.Show("Пазл собран неверно!");
                AddFailedAttempt(LogintextBox.Text);
                Shuffle();
                return;
            }

            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT password, role, is_blocked FROM Users WHERE login = @login", conn);
                cmd.Parameters.AddWithValue("@login", LogintextBox.Text);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    bool success = false;
                    string role = "";
                    string errorMsg = "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные";

                    if (reader.Read())
                    {
                        bool isBlocked = Convert.ToBoolean(reader["is_blocked"]);
                        string dbPassword = reader["password"].ToString();
                        role = reader["role"].ToString().Trim();

                        if (isBlocked)
                            errorMsg = "Вы заблокированы. Обратитесь к администратору";
                        else if (dbPassword == PasswordtextBox.Text)
                            success = true;
                    }

                    if (success)
                    {
                        ResetFailedAttempts(LogintextBox.Text);
                        MessageBox.Show("Вы успешно авторизовались");

                        if (role == "Admin" || role == "Администратор")
                            new AdminForm().Show();
                        else
                        {
                            MessageBox.Show("Вы вошли как пользователь. Форма в разработке.");
                            Application.Exit();
                        }
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show(errorMsg);
                        AddFailedAttempt(LogintextBox.Text);
                        Shuffle();
                    }
                }
            }
        }

        void AddFailedAttempt(string login)
        {
            if (string.IsNullOrEmpty(login)) return;
            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET failed_attempts = failed_attempts + 1 WHERE login = @login; UPDATE Users SET is_blocked = 1 WHERE login = @login AND failed_attempts >= 3;", conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.ExecuteNonQuery();
            }
        }

        void ResetFailedAttempts(string login)
        {
            using (SqlConnection conn = DataBaseHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET failed_attempts = 0, is_blocked = 0 WHERE login = @login", conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.ExecuteNonQuery();
            }
        }
        private void AuthorizationForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}