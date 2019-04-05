using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;
namespace AlwaysChat_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int port = 8888;
        private const string server = "127.0.0.1";
        TcpClient client;
        NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();
            chatViewBox.IsReadOnly = true;
            messageRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Text")));
        }
        public void startSession()
        {
            try
            {
                client = new TcpClient();
                client.Connect(server, port);
                Action act1 = () => connectButton.IsEnabled = false;
                Action appMessage;
                this.Dispatcher.Invoke(act1);
                byte[] data = new byte[2048];
                StringBuilder response = new StringBuilder();
                stream = client.GetStream();
                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    string chatMessage = Convert.ToString(response);
                    appMessage = ()=> chatViewBox.AppendText(chatMessage);
                    this.Dispatcher.Invoke(appMessage);

                }
                while ((!response.Equals("VnI28i7:V)y")));
                

                // Закрываем потоки
                stream.Close();
                client.Close();
            }
            catch(SocketException e)
            {
                MessageBox.Show(e.Message + "\n" + "Код ошибки: " + e.ErrorCode);
                Action act2 = () => connectButton.IsEnabled = true;
                this.Dispatcher.Invoke(act2);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                Action act2 = () => connectButton.IsEnabled = true;
                this.Dispatcher.Invoke(act2);   //BUG when close app
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Thread threadSession = new Thread(new ThreadStart(startSession));
            threadSession.Start();
            
            //startSession();
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RichTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                if(stream!=null && client != null)//&& messageRichTextBox.ToString()!=""
                {
                    
                    string message = new TextRange(messageRichTextBox.Document.ContentStart, messageRichTextBox.Document.ContentEnd).Text;
                    byte[] data  = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}
