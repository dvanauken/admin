// MainForm.cs
using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Admin
{
    public partial class MainForm : Form
    {
        private Panel headerPanel;
        private Panel footerPanel;
        private Panel leftPanel;
        private Panel rightPanel;
        private Panel contentPanel;
        private MenuStrip menuStrip;
        private RichTextBox contentTextBox;

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            this.Text = "Windows Diagnostics Tool";
            this.Size = new Size(1024, 768);
        }

        private void SetupUI()
        {
            // Initialize MenuStrip
            menuStrip = new MenuStrip();
            ToolStripMenuItem diagnosticsMenu = new ToolStripMenuItem("Diagnostics");
            
            ToolStripMenuItem diskSpaceItem = new ToolStripMenuItem("Disk Space");
            diskSpaceItem.Click += (s, e) => ShowDiskSpace();
            
            ToolStripMenuItem memoryUsageItem = new ToolStripMenuItem("Memory Usage");
            memoryUsageItem.Click += (s, e) => ShowMemoryUsage();
            
            ToolStripMenuItem portUsageItem = new ToolStripMenuItem("Port Usage");
            portUsageItem.Click += (s, e) => ShowPortUsage();

            diagnosticsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                diskSpaceItem,
                memoryUsageItem,
                portUsageItem
            });

            menuStrip.Items.Add(diagnosticsMenu);
            this.Controls.Add(menuStrip);

            // Initialize Panels
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray
            };

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.LightGray
            };

            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.WhiteSmoke
            };

            rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 200,
                BackColor = Color.WhiteSmoke
            };

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Initialize RichTextBox for content
            contentTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 10F),
                Multiline = true
            };

            // Add controls in correct order
            this.Controls.Add(contentPanel);
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(headerPanel);
            contentPanel.Controls.Add(contentTextBox);

            // Add labels to identify panels
            headerPanel.Controls.Add(new Label { Text = "Header", AutoSize = true, Location = new Point(10, 10) });
            footerPanel.Controls.Add(new Label { Text = "Footer", AutoSize = true, Location = new Point(10, 5) });
            leftPanel.Controls.Add(new Label { Text = "Left Navigation", AutoSize = true, Location = new Point(10, 10) });
            rightPanel.Controls.Add(new Label { Text = "Right Panel", AutoSize = true, Location = new Point(10, 10) });
        }

        private void ShowDiskSpace()
        {
            contentTextBox.Clear();
            contentTextBox.AppendText("Disk Space Information:\n\n");

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    contentTextBox.AppendText($"Drive {drive.Name}:\n");
                    contentTextBox.AppendText($"  Format: {drive.DriveFormat}\n");
                    contentTextBox.AppendText($"  Free space: {drive.TotalFreeSpace / 1073741824:N2} GB\n");
                    contentTextBox.AppendText($"  Total size: {drive.TotalSize / 1073741824:N2} GB\n");
                    contentTextBox.AppendText($"  % Free: {(double)drive.TotalFreeSpace / drive.TotalSize:P2}\n\n");
                }
            }
        }

        private void ShowMemoryUsage()
        {
            contentTextBox.Clear();
            contentTextBox.AppendText("Memory Usage Information:\n\n");

            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            var memoryValues = wmiObject.Get();

            foreach (ManagementObject obj in memoryValues)
            {
                var totalVisibleMemory = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                var freePhysicalMemory = Convert.ToDouble(obj["FreePhysicalMemory"]);
                
                contentTextBox.AppendText($"Total Memory: {totalVisibleMemory / 1024:N2} GB\n");
                contentTextBox.AppendText($"Free Memory: {freePhysicalMemory / 1024:N2} GB\n");
                contentTextBox.AppendText($"Used Memory: {(totalVisibleMemory - freePhysicalMemory) / 1024:N2} GB\n");
                contentTextBox.AppendText($"Memory Usage: {((totalVisibleMemory - freePhysicalMemory) / totalVisibleMemory):P2}\n");
            }
        }

        private void ShowPortUsage()
        {
            contentTextBox.Clear();
            contentTextBox.AppendText("Active TCP Connections:\n\n");

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation connection in connections)
            {
                contentTextBox.AppendText($"Local: {connection.LocalEndPoint}\n");
                contentTextBox.AppendText($"Remote: {connection.RemoteEndPoint}\n");
                contentTextBox.AppendText($"State: {connection.State}\n\n");
            }
        }
    }
}

