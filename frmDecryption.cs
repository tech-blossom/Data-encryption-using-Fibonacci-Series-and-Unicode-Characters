using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SplashScreenThreaded
{
    public partial class frmDecryption : Form
    {
        string FilePath = string.Empty;
        public frmDecryption()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (lblName.Text != "." && txtKey.Text != "")
            {
                Helper objHelper = new Helper();
                List<Color> pixelColor = new List<Color>();
                string DecryptKey = string.Empty;
                string channel = string.Empty;
                int blockSize = 0;
                int keySize = 0;
                string Result = string.Empty;
                DecryptKey = objHelper.DecryptText(txtKey.Text);
                string[] arr = DecryptKey.Split('|');
                if (arr.Length == 3)
                {
                    if (arr[0].Length > 0)
                    {
                        string[] arrBlock = arr[0].ToString().Split('=');
                        blockSize = Convert.ToInt32(arrBlock[1]);
                    }
                    if (arr[1].Length > 0)
                    {
                        string[] arrChannel = arr[1].ToString().Split('=');
                        channel = arrChannel[1];
                    }
                    if (arr[2].Length > 0)
                    {
                        string[] arrKeySize = arr[2].ToString().Split('=');
                        keySize = Convert.ToInt32(arrKeySize[1]);
                    }
                    Bitmap myBitmap = new Bitmap(FilePath);
                    for (int x = 0; x < myBitmap.Height; x++)
                    {
                        for (int z = 0; z < myBitmap.Width; z++)
                        {
                            pixelColor.Add(myBitmap.GetPixel(z, x));
                            z = z + blockSize;
                        }
                        x = x + blockSize;
                    }
                    List<byte> objBytes = new List<byte>();
                    foreach (Color item in pixelColor)
                    {
                        if (channel.Equals("R") && item.R > 0)
                        {
                            objBytes.Add((byte)(item.R - keySize));
                        }
                        else if (channel.Equals("B") && item.B > 0)
                        {
                            objBytes.Add((byte)(item.B - keySize));
                        }
                        else if (channel.Equals("G") && item.G > 0)
                        {
                            objBytes.Add((byte)(item.G - keySize));
                        }
                    }
                    byte[] objbty = (byte[])objBytes.ToArray();
                    Result = System.Text.Encoding.ASCII.GetString(objbty);
                    txtResult.Text = Result;

                }
                else
                {
                    MessageBox.Show("Enter key is not valid");
                    txtKey.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Please select image file and enter Key");
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog1.FileName;
                lblName.Text = openFileDialog1.SafeFileName;
            }
        }
    }
}
