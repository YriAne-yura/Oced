using Microsoft.Win32;
using Oced.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            CheckRegistrySettings();

            if (!IsAdministrator())
            {
                MessageBox.Show("Yêu cầu chạy oced với quyền administrators để sử dụng 100% tính năng của công cụ", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            button3.Text = (button3.Text == "ON") ? "OFF" : "ON";
            button3.BackColor = (button3.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button3.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled", 0); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled"); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

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


            button7.Text = (button7.Text == "ON") ? "OFF" : "ON";
            button7.BackColor = (button7.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button7.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack", 0); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack"); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Text = (button8.Text == "ON") ? "OFF" : "ON";
            button8.BackColor = (button8.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button8.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled", 0); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled"); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.Text = (button9.Text == "ON") ? "OFF" : "ON";
            button9.BackColor = (button9.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button9.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled", 0); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled"); MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Text = (button10.Text == "ON") ? "OFF" : "ON";
            button10.BackColor = (button10.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button10.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled", 0);
                MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled");
                MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        private void button11_Click(object sender, EventArgs e)
        {


            button11.Text = (button11.Text == "ON") ? "OFF" : "ON";
            button11.BackColor = (button11.Text == "ON") ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);

            if (button11.Text == "ON")
            {
                CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed", 0);
                MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed");
                MessageBox.Show("Tinh chỉnh thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        // Button ON All
        private void button6_Click(object sender, EventArgs e)
        {
            ChangeAllColorButtonON();
            ChangeAllTextButtonON();
            ChangeAllRegON();
            MessageBox.Show("Hoàn tất chỉnh toàn bộ tối ưu thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Button OFF ALL
        private void button5_Click(object sender, EventArgs e)
        {
            ChangeAllColorButtonOFF();
            ChangeAllTextButtonOFF();
            ChangeAllRegOFF();
            MessageBox.Show("Hoàn tất xoá toàn bộ tối ưu thành công", "Oced", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// Check Registry Exist
        /// </summary>
        private void CheckRegistrySettings()
        {
            CheckButtonState(button3, "AutofillCreditCardEnabled");
            CheckButtonState(button7, "ConfigureDoNotTrack");
            CheckButtonState(button8, "HubsSidebarEnabled");
            CheckButtonState(button9, "PaymentMethodQueryEnabled");
            CheckButtonState(button10, "PersonalizationReportingEnabled");
            CheckButtonState(button11, "UserFeedbackAllowed");
        }
        private void CheckButtonState(System.Windows.Forms.Button button, string keyRegistry)
        {
            if (Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Edge") is RegistryKey edgeKey)
            {
                if (edgeKey.GetValueNames().Contains(keyRegistry))
                {
                    object value = edgeKey.GetValue(keyRegistry);
                    if (value != null && value is int intValue)
                    {
                        button.Text = (intValue == 0) ? "ON" : "OFF";
                        button.BackColor = (intValue == 0) ? Color.FromArgb(144, 254, 191) : Color.FromArgb(255, 202, 191);
                    }
                }
            }
        }




        /// <summary>
        /// This is Setting List Optimize or Add other bla bla
        /// </summary>
        private void ChangeAllColorButtonON()
        {
            button3.BackColor = Color.FromArgb(144, 254, 191);
            button7.BackColor = Color.FromArgb(144, 254, 191);
            button8.BackColor = Color.FromArgb(144, 254, 191);
            button9.BackColor = Color.FromArgb(144, 254, 191);
            button10.BackColor = Color.FromArgb(144, 254, 191);
            button11.BackColor = Color.FromArgb(144, 254, 191);
        }
        private void ChangeAllTextButtonON()
        {
            button3.Text = "ON";
            button7.Text = "ON";
            button8.Text = "ON";
            button9.Text = "ON";
            button10.Text = "ON";
            button11.Text = "ON";
        }
        private void ChangeAllRegON()
        {
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled", 0);
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack", 0);
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled", 0);
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled", 0);
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled", 0);
            CreateDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed", 0);

        }
        private void ChangeAllRegOFF()
        {
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "AutofillCreditCardEnabled");
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "ConfigureDoNotTrack");
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "HubsSidebarEnabled");
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PaymentMethodQueryEnabled");
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "PersonalizationReportingEnabled");
            DeleteDwordKey("Software\\Policies\\Microsoft\\Edge", "UserFeedbackAllowed");

        }
        private void ChangeAllColorButtonOFF()
        {
            button3.BackColor = Color.FromArgb(255, 202, 191);
            button7.BackColor = Color.FromArgb(255, 202, 191);
            button8.BackColor = Color.FromArgb(255, 202, 191);
            button9.BackColor = Color.FromArgb(255, 202, 191);
            button10.BackColor = Color.FromArgb(255, 202, 191);
            button11.BackColor = Color.FromArgb(255, 202, 191);
        }
        private void ChangeAllTextButtonOFF()
        {
            button3.Text = "OFF";
            button7.Text = "OFF";
            button8.Text = "OFF";
            button9.Text = "OFF";
            button10.Text = "OFF";
            button11.Text = "OFF";
        }

        static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.Show("Cài đặt để bật hoặc tắt tự động điền thông tin thẻ tín dụng \n trong một phần thanh toán bất kì nào đó.", pictureBox6);
        }

        private void pictureBox7_MouseMove(object sender, EventArgs e)
        {
            toolTip1.Show("\"Do Not Track,\" thường được sử dụng để yêu cầu ứng dụng web hoặc trang web \n không theo dõi hoạt động của người dùng.", pictureBox7);
        }
    }
}
