using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace socketClient
{
    public partial class FormClient : Form
    {
        Socket client;

        public FormClient()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AsyncConnect();
        }


        /// <summary>
        /// 連線到伺服器
        /// </summary>
        public void AsyncConnect()
        {
            try
            {
                //埠及IP
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(textBox_ip.Text), int.Parse(textBox_port.Text));
                //建立套接字
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //開始連線到伺服器
                client.BeginConnect(ipe, asyncResult =>
                {
                    client.EndConnect(asyncResult);
                    //向伺服器傳送訊息
                    AsyncSend(client, "你好我是客戶端");
                    //接受訊息
                    AsyncReceive(client);
                }, null);
            }
            catch (Exception ex)
            {
            }

        }

        public void AsyncSend(Socket socket, string message)
        {
            if (socket == null || message == string.Empty) return;
            //編碼
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {

                socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成傳送訊息
                    int length = socket.EndSend(asyncResult);
                }, null);
            }
            catch (Exception ex)
            {
            }
        }


        public void AsyncReceive(Socket socket)
        {
            byte[] data = new byte[1024];
            try
            {
                //開始接收資料
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None, asyncResult =>
                    {
                        try
                        {
                            int length = socket.EndReceive(asyncResult);
                            setText(Encoding.UTF8.GetString(data));
                        }
                        catch (Exception)
                        {
                            AsyncReceive(socket);
                        }
                        AsyncReceive(socket);
                    }, null);
            }
            catch (Exception ex)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AsyncSend(client, textBox4.Text);
        }

        private void setText(string str)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => setText(str)));
            }
            else
            {
                textBox3.Text += "\r\n" + str;
            }
        }



    }
}
