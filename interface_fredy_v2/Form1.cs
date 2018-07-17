using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace interface_fredy_v2
{
    public partial class Form1 : Form
    {

        private bool privodActivated = false;
        int counter = 0;
        //Thread readThread = new Thread(Read);
        public Form1()
        {
            InitializeComponent();
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MessageBox.Show("Отладка в форме: " + e.KeyCode.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            UpdateComList();
            button4.Enabled = false;
            maskedTextBox1.Mask = "000";
            maskedTextBox2.Mask = "000";
            button5.Enabled = false;
        }
        private void UpdateComList()
        {
            comboBox1.Items.Clear();
            //foreach (string com in SerialPort.GetPortNames()) comboBox1.Items.Add(com);
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateComList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    //readThread.Abort();
                    serialPort1.Close();
                    label1.Text = "Статус соединения: Отключено";
                    button2.Text = "Соединиться";
                    comboBox1.Enabled = true;
                    button1.Enabled = true;

                    button4.Enabled = false;
                    button5.Enabled = false;
                    //деактивация приводов
                    button3.Enabled = false;
                    deactivatePrivod();
                }
                else if (comboBox1.SelectedIndex >= 0)
                {
                    //readThread.Start();
                    serialPort1.BaudRate = 57600;
                    serialPort1.PortName = comboBox1.SelectedItem.ToString();
                    serialPort1.Handshake = Handshake.None;
                    serialPort1.ReadTimeout = 210; //210 здесь если в роботе 100
                    serialPort1.WriteTimeout = -1; //может ждать бесконечно
                    serialPort1.Open();
                    label1.Text = "Статус соединения: Подключено";
                    button2.Text = "Отсоединиться";
                    comboBox1.Enabled = false;
                    button1.Enabled = false;

                    button4.Enabled = true;
                    button5.Enabled = true;

                    //активация кнопки приводов
                    button3.Enabled = true;
                }
                else MessageBox.Show("Ошибка: Выберите COM порт!");
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка 1: " + error.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (privodActivated)
            {
                //button4.Enabled = true;
                deactivatePrivod();
            }
            else if (serialPort1.IsOpen)
            {
                //button4.Enabled = false;
                activatePrivod();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //чтобы при закрытии формы соединение закрывалось и у других приложений был доступ к порту
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void button3_KeyDown(object sender, KeyEventArgs e)
        {
            if (privodActivated)
            {
                switch (e.KeyCode.ToString())
                {
                    case "Q":
                        serialPort1.Write("q");
                        delay_counter();
                        break;
                    case "W":
                        serialPort1.Write("w");
                        delay_counter();
                        break;
                    case "E":
                        serialPort1.Write("e");
                        delay_counter();
                        break;
                    case "A":
                        serialPort1.Write("a");
                        delay_counter();
                        break;
                    case "S":
                        serialPort1.Write("s");
                        delay_counter();
                        break;
                    case "D":
                        serialPort1.Write("d");
                        delay_counter();
                        break;
                    case "Escape":
                        deactivatePrivod();
                        break;
                }
                //readThread.Join();
            }
        }

        private void activatePrivod()
        {
            privodActivated = true;
            label2.Text = "Статус приводов: Активированы";
            button3.Text = "Деактивировать приводы";
            //MessageBox.Show("Статус: Приводы активированы");
        }

        private void deactivatePrivod()
        {
            privodActivated = false;
            label2.Text = "Статус приводов: Деактивированы";
            button3.Text = "Активировать приводы";
            //MessageBox.Show("Статус: Приводы деактивированы");
        }

        //public static void Read()
        //{
        //    while (true)
        //         //(serialPort1.IsOpen)
        //    {
        //        try
        //        {
        //            //MessageBox.Show(serialPort1.ReadLine());
        //            //textBox1.Text = serialPort1.ReadLine();
        //        }
        //        catch (TimeoutException error)
        //        {
        //            MessageBox.Show("Ошибка 2: " + error.Message);
        //        }
        //    }
        //}

        private void delay_counter()
        {
            try
            {
                //задержка решает
                counter++;
                label3.Text = "Выполнено операций: " + counter;
                Thread.Sleep(40);
            }
            catch (TimeoutException error)
            {
                //MessageBox.Show("Ошибка 3: " + error.Message);
            }
        }
        private void refreshConsole()
        {
            try
            {
                //serialPort1.ReadTimeout = -1;
                while (serialPort1.BytesToRead > 0) textBox1.Text += serialPort1.ReadLine() + "\n";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
            catch (TimeoutException error)
            {
                MessageBox.Show("Ошибка 3: " + error.Message);
            }
            finally
            { //serialPort1.ReadTimeout = 210; }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            deactivatePrivod();
            if (serialPort1.IsOpen) serialPort1.Write("x" + maskedTextBox1.Text);
            //MessageBox.Show("x" + maskedTextBox1.Text);
            Thread.Sleep(2000);
            if (serialPort1.IsOpen) serialPort1.Write("c" + maskedTextBox2.Text);
            //MessageBox.Show("c" + maskedTextBox2.Text);
            Thread.Sleep(2000);
            refreshConsole();
            button3.Enabled = true;
            button3.Focus();
            activatePrivod();
            counter += 2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            refreshConsole();
        }
    }
}
