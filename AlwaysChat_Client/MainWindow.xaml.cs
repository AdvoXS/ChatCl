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
namespace AlwaysChat_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int port = 8888;
        private const string server = "127.0.0.1";
        public MainWindow()
        {
            InitializeComponent();
        }
        public void startSession()
        {
            TcpClient client = new TcpClient();
            client.Connect(server, port);

            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();
            NetworkStream stream = client.GetStream();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while ((!response.Equals("VnI28i7:V)y"))); // пока данные есть в потоке
            string chatMessage = Convert.ToString(response);
            chatViewBox.AppendText(chatMessage);

            // Закрываем потоки
            stream.Close();
            client.Close();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            startSession();
        }
    }
}
