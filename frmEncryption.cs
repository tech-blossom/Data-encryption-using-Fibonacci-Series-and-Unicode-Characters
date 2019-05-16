using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace SplashScreenThreaded
{
    public partial class frmEncryption : Form
    {
        public static string ImgPath = string.Empty;
        public static int FiboNo = 0;
        public frmEncryption()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtText.Text = File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string channel = string.Empty;
            int keySize = FiboNo;
            if (checkValidation() == false)
            {
                return;
            }
            //char[] charArr = txtText.Text.ToCharArray();
            //foreach (char c in charArr)
            //{
            //   byte[] asciiBytes = Encoding.ASCII.GetBytes(c.ToString());
            //}
            byte[] asciiBytes = Encoding.ASCII.GetBytes(txtText.Text);
            byte[] UnicodeBytes = new byte[asciiBytes.Length];
           
            List<Helper> objColor = new List<Helper>();
            int index = 0;
            foreach (byte byt in asciiBytes)
            {
                UnicodeBytes[index++] = (byte)((int)byt + (int)keySize);

                string Color = string.Empty;
                if (rdbC.Checked == true)
                {
                    objColor.Add(new Helper { R = (int)(byt) + keySize, G = (int)nudY.Value, B = (int)nudM.Value });
                    channel = "R";
                }
                else if (rdbM.Checked == true)
                {
                    objColor.Add(new Helper {  R= (int)nudC.Value, G = (int)nudY.Value, B = (int)(byt) + keySize });
                    channel = "B";
                }
                else if (rdbY.Checked == true)
                {
                    objColor.Add(new Helper {  R = (int)nudC.Value, G = (int)(byt) + keySize, B = (int)nudM.Value });
                    channel = "G";
                }

            }
            txtUnicode.Text = Encoding.Unicode.GetString(UnicodeBytes);
            int blockSize = (int)nudBlockSize.Value;
            int width = 800, height = 600;
            int i = 0;
            Bitmap bmp = new Bitmap(width, height);
            Random rand = new Random();
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    //for (int i = 0; i < objColor.Count; i++)
                    //{
                    if (i < objColor.Count)
                    {
                        
                        int r = objColor[i].R;
                        int g = objColor[i].G;
                        int b = objColor[i].B;
                        // bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                        Color argb = Color.FromArgb(r, g, b);
                        using (Graphics gr = Graphics.FromImage(bmp))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(r, g, b)))
                            {
                                gr.FillRectangle(brush, x, z, blockSize, blockSize);
                            }
                        }
                        i++;
                    }
                    //}

                    x = x + blockSize;
                }
                z = z + blockSize;
            }
            //pictureBox1.Image = bmp;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save Image Files";
            saveFileDialog1.DefaultExt = "jpg";
            saveFileDialog1.Filter = "Images|*.png;*.jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(saveFileDialog1.FileName);
                bmp.Save(saveFileDialog1.FileName);
                ImgPath = saveFileDialog1.FileName;
            }

            string Key = string.Format("BlockSize={0}|Channel={1}|KeySize={2}", blockSize, channel, keySize);
            Helper objHelp = new Helper();
            txtKey.Text = objHelp.EncryptText(Key);
            btnsendMail.Enabled = true;
        }
        private bool checkValidation()
        {
            if (string.IsNullOrEmpty(txtText.Text))
            {
                MessageBox.Show("Please enter some text");
                return false;
            }
            if (rdbC.Checked == false && rdbM.Checked == false && rdbY.Checked == false)
            {
                MessageBox.Show("Please select Color Channel");
                return false;
            }

            return true;
        }

        private void rdbRed_Click(object sender, EventArgs e)
        {
            rdbM.Checked = rdbY.Checked = false;
            nudM.Enabled = nudY.Enabled = true;
            if (rdbC.Checked == false)
                rdbC.Checked = true;
            nudC.Enabled = false;
        }

        private void rdbBlue_Click(object sender, EventArgs e)
        {
            rdbC.Checked = rdbY.Checked = false;
            nudC.Enabled = nudY.Enabled = true;
            if (rdbM.Checked == false)
                rdbM.Checked = true;
            nudM.Enabled = false;
        }

        private void rdbGreen_Click(object sender, EventArgs e)
        {
            rdbY.Checked = rdbM.Checked = false;
            nudC.Enabled = nudM.Enabled = true;
            if (rdbY.Checked == false)
                rdbY.Checked = true;
            nudY.Enabled = false;
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            int x = 800 / (int)nudBlockSize.Value;
            int y = 600 / (int)nudBlockSize.Value;
            int z = x * y;
            lblCounter.Text = "Total Text: " + z + ". Remaining: " + (z - txtText.Text.Length);
        }

        private void btnsendMail_Click(object sender, EventArgs e)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("catch.me.if.you.misbehave@gmail.com");

                mail.To.Add(txtEmail.Text);
                mail.Subject = "Color Substitutation Cryptography";
                mail.Body = "Your Decryption Key= " + txtKey.Text;

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(ImgPath);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("catch.me.if.you.misbehave@gmail.com", "C@tchMeif123");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("mail Send");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtText.Text = txtKey.Text = txtEmail.Text = string.Empty;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void lblCounter_Click(object sender, EventArgs e)
        {

        }

        private void frmEncryption_Load(object sender, EventArgs e)
        {
            var rand = new Random();
            FiboNo = Helper.Fibonaci[rand.Next(Helper.Fibonaci.Length)];
        }
    }
}
