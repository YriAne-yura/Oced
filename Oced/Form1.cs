using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oced
{
    public partial class Form1 : Form
    {
        // Khai báo các hàm và hằng số để di chuyển cửa sổ
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
    (
       int nLeftRect,
       int nTopRect,
       int nRightRect,
       int nBottomRect,
       int nWidthEllipse,
       int nHeightEllipse
    );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

        FlowLayoutPanel flowLayoutPanel1; // Thêm dòng này

        public Form1()
        {
            InitializeComponent();
            ApplyRoundedCorners();
            // Thêm sự kiện chuột cho panel_move để di chuyển cửa sổ
            panelmove.MouseDown += PanelMove_MouseDown;
            panelmove.MouseMove += PanelMove_MouseMove;


            // Kiểm tra xem key "Edge" có tồn tại hay không
            string baseKeyPath = "Software\\Policies\\Microsoft\\Edge";
            if (Registry.CurrentUser.OpenSubKey(baseKeyPath) != null)
            {
                // Nếu tồn tại, thay đổi màu của nút và chữ trên nút
                button4.BackColor = Color.Green;
                button4.ForeColor = Color.White;
                button4.Text = "Success";
            }


        }
        private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Khi chuột được nhấn vào panel, di chuyển cửa sổ
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // You may also need to override the OnResize event to update the region when the form is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedCorners();
        }
        private void ApplyRoundedCorners()
        {
            IntPtr region = CreateRoundRectRgn(0, 0, Width, Height, 25, 25);
            SetWindowRgn(Handle, region, true);
        }
        private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        {
            // Khi chuột được di chuyển trên panel, thực hiện các xử lý nếu cần
            // Ví dụ: thay đổi màu sắc hoặc hiển thị thông tin gì đó
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Đóng chương trình khi bấm vào button
            while (Opacity > 0)
            {
                Opacity -= 0.1; // Adjust the decrement value for the fading speed
                System.Threading.Thread.Sleep(1); // Add a delay to control the fading speed
                Application.DoEvents(); // Allow the UI to update during the loop
                if (Opacity == 0)
                {
                    this.Close();
                }
            }
        }


        /// <summary>
        /// CreateDwordKey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        static void CreateDwordKey(string keyPath, string valueName, int dwordValue)
        {
            try
            {
                // Mở hoặc tạo key trong registry để ghi
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    // Kiểm tra xem key có tồn tại không
                    if (key != null)
                    {
                        // Tạo DWORD với giá trị được chỉ định
                        key.SetValue(valueName, dwordValue, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        // Delete DWORD REG
        static void DeleteDwordKey(string keyPath, string valueName)
        {
            try
            {
                // Mở hoặc tạo key trong registry để ghi
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    // Kiểm tra xem key có tồn tại không
                    if (key != null)
                    {
                        // Xóa DWORD với tên được chỉ định
                        key.DeleteValue(valueName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void timer1_tick(object sender, EventArgs e)
        {

            Opacity += .1;
            if (Opacity == 1)
            {
                timer1.Stop();
            }
        }

        private void checkregistry_exist(object sender, EventArgs e)
        {

            MessageBox.Show("hi", "tiêu đề");


        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string baseKeyPath = @"Software\Policies\Microsoft\Windows";
                string edgeKeyPath = @"Edge";
                string fullPath = baseKeyPath + "\\" + edgeKeyPath;

                // Kiểm tra xem key "Edge" có tồn tại hay không
                if (Registry.CurrentUser.OpenSubKey(fullPath) != null)
                {

                    MessageBox.Show("Key đã được tạo , bạn không cần phải tạo nữa", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    // Nếu không tồn tại, tạo registry key mới
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(fullPath))
                    {
                        if (key != null)
                        {
                            MessageBox.Show("Tạo Key edge thành công.", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Tạo Key edge thất bại.", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "ON")
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled");
                button3.BackColor = Color.FromArgb(255, 202, 191);
                button3.Text = "OFF";
            }
            else if (button3.Text == "OFF")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled", 0);
                button3.BackColor = Color.FromArgb(144, 254, 191);
                button3.Text = "ON";
            }

        }

        private void panelmove_Paint(object sender, PaintEventArgs e)
        {
            // nothing
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.youtube.com/channel/UCqnb_ntxhhG_js7OdiSGs1A");
        }


        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.facebook.com/SiroCandy06/");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/SiroCandy06");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            OpenUrl("https://discord.gg/yZUst5DzqR");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (button7.Text == "ON")
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack");
                button7.BackColor = Color.FromArgb(255, 202, 191);
                button7.Text = "OFF";
            }
            else if (button7.Text == "OFF")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack", 1);
                button7.BackColor = Color.FromArgb(144, 254, 191);
                button7.Text = "ON";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.Text == "ON")
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled");
                button8.BackColor = Color.FromArgb(255, 202, 191);
                button8.Text = "OFF";
            }
            else if (button8.Text == "OFF")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled", 0);
                button8.BackColor = Color.FromArgb(144, 254, 191);
                button8.Text = "ON";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.Text == "ON")
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled");
                button9.BackColor = Color.FromArgb(255, 202, 191);
                button9.Text = "OFF";
            }
            else if (button8.Text == "OFF")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled", 0);
                button9.BackColor = Color.FromArgb(144, 254, 191);
                button9.Text = "ON";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Text = (button10.Text == "ON") ? "OFF" : "ON";
            button10.BackColor = (button10.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button10.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled", 0);
            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled");
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (button11.Text == "ON")
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed");
                button11.BackColor = Color.FromArgb(255, 202, 191);
                button11.Text = "OFF";
            }
            else if (button8.Text == "OFF")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed", 0);
                button11.BackColor = Color.FromArgb(144, 254, 191);
                button11.Text = "ON";
            }
        }
    }
}
