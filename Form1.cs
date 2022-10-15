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
using static piano.InputSender;

namespace piano
{
    public partial class Form1 : Form
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox1_KeyDown);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox1_KeyPress);
            this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(textBox1_KeyUp);

            IntPtr calcWindow = FindWindow(null, "Form1");
            if (SetForegroundWindow(calcWindow))
            {
                Thread myThread1 = new Thread(PlaySong);
                myThread1.Start();
            }
            textBox1.Focus();
        }
        void PlaySong()
        {
            //Thread.Sleep(2000);
            //Input[] inputs = new Input[]
            //{
            //    new Input
            //    {
            //        type = (int)InputType.Keyboard,
            //        u = new InputUnion
            //        {
            //            ki = new KeyboardInput
            //            {
            //                wVk = 0,
            //                wScan = 0x11,
            //                dwFlags=(uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
            //                dwExtraInfo=GetMessageExtraInfo()
            //            }
            //        }
            //    },
            //    new Input
            //    {
            //        type = (int)InputType.Keyboard,
            //        u = new InputUnion
            //        {
            //            ki = new KeyboardInput
            //            {
            //                wVk = 0,
            //                wScan = 0x11,
            //                dwFlags=(uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
            //                dwExtraInfo=GetMessageExtraInfo()
            //            }
            //        }
            //    }
            //};
            //SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));

            int tact = Convert.ToInt32(textBoxTact.Text);
            Thread.Sleep(400);
            string text = textBox1.Text;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '[')
                {
                    i++;
                    string buffer = text[i].ToString();
                    i++;
                    while (text[i] != ']')
                    {
                        buffer = buffer + text[i];
                        i++;
                    }
                    if (text[i] == ']')
                    {
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            ConvertCharToVirtualKey(buffer[j]);
                        }
                        Thread.Sleep(tact * 10);
                        continue;
                    }
                }
                if (text[i] == ' ')
                {
                    //PresslowKey(Keys.Space); Прикол для гонок на клавиатуре08
                    Thread.Sleep(tact * 10);
                    continue;
                }
                if (text[i] == '|')
                {
                    Thread.Sleep(tact * 20);
                    continue;
                }
                ConvertCharToVirtualKey(text[i]);
                Thread.Sleep(Convert.ToInt32(tact)); //20
            }
        }
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(int uCode, uint uMapType);
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
                    byte bVk,
                    byte bScan,
                                int dwFlags, // Здесь целочисленный тип нажимается 0, отпускается 2
                                int dwExtraInfo // Это целочисленный тип. Обычно устанавливается в 0
                );
        void PressKey(Keys key)
        {
            //keybd_event((byte)key, 0, 0, 0);
            //Thread.Sleep(1);
            //keybd_event((byte)key, 0, 2, 0);
            Input[] inputs = new Input[]
            {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = (ushort)key,
                            //wScan = 0x11,
                            dwFlags=(uint)(KeyEventF.KeyDown),
                            dwExtraInfo=GetMessageExtraInfo()
                        }
                    }
                },
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk =(ushort) key,
                            //wScan = 0x11,
                            dwFlags=(uint)(KeyEventF.KeyUp ),
                            dwExtraInfo=GetMessageExtraInfo()
                        }
                    }
                }
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        void PressHighKey(Keys key)
        {
            keybd_event((byte)Keys.ShiftKey, 0, 0, 0);
            //PressKey(key);
            keybd_event((byte)Keys.ShiftKey, 0, 2, 0);
        }

        void ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            //int modifiers = vkey >> 8;
            //if ((modifiers & 1) != 0) { PressHighKey((Keys)retval); return; }
            ////if ((modifiers & 2) != 0) retval |= Keys.Control;
            ////if ((modifiers & 4) != 0) retval |= Keys.Alt;

            PressKey(retval);
            //PressKey(retval);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        private void buttonStop_Click(object sender, EventArgs e)
        {
            //Thread.ResetAbort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxTact.Text = "20";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            listBox1.Items.Add($"{DateTime.Now:mm:ss:ff}: Key Down");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            listBox1.Items.Add($"{DateTime.Now:mm:ss:ff}: Key Press");
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            listBox1.Items.Add($"{DateTime.Now:mm:ss:ff}: Key Up");

        }
    }
}