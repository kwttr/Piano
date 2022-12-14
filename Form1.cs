using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace piano
{
    public partial class Form1 : Form
    {
        CancellationTokenSource _tokenSource = null;

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            IntPtr calcWindow = FindWindow(null, "Form1");
            if (SetForegroundWindow(calcWindow))
            {
                Player player = new Player(textBox1.Text,Convert.ToInt16(textBoxTact.Text));
                _tokenSource = new CancellationTokenSource();
                var token = _tokenSource.Token;
                try {
                    await Task.Run(() => player.PlaySong(token)); 
                }
                finally
                {
                    _tokenSource.Dispose();
                }
            }
            textBox1.Focus();
        }
        
        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop(_tokenSource);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxTact.Text = "200";
            HotKey hotKeyStop = new HotKey();
            hotKeyStop.Key = Keys.F7;
            hotKeyStop.HotKeyPressed += HotkeyStop_HotKeyPressed;

            HotKey hotKeyStart = new HotKey();
            hotKeyStart.Key = Keys.F6;
            hotKeyStart.HotKeyPressed += HotKeyStart_HotKeyPressed;


        }

        private void HotKeyStart_HotKeyPressed(object? sender, KeyEventArgs e)
        {
            buttonStart_Click(sender, e);
        }

        private void HotkeyStop_HotKeyPressed(object? sender, KeyEventArgs e)
        {
            Stop(_tokenSource);
        }

        public void Stop(CancellationTokenSource _tokenSource)
        {
            _tokenSource?.Cancel();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Song song = new Song(textBox1.Text, Convert.ToInt16(textBoxTact.Text));
            Form SaveForm = new SaveForm(song);
            SaveForm.ShowDialog();
        }

        private void comboBoxSongs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}