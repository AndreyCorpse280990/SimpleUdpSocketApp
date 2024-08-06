using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleUdpSocketApp
{
    internal class Program
    {
        // RunFirstUdpInstance - процедура запуска первой стороны в udp-взаимодействии
        static void RunFirstUdpInstance(string localIpStr, int localPort, string remoteIpStr, int remotePort)
        {
            Socket socket = null;
            try
            {
                // 1. создать и настроить необходимые объекты (сокет, endpoint-ы для взаимодействия)
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localIpStr), localPort);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpStr), remotePort);
                Console.WriteLine("[first]: socket and endpoints created");

                // 2. привязать сокет к endpoint-у для получения udp-сообщений
                socket.Bind(localEndPoint);
                Console.WriteLine($"[first]: socket binded to {localEndPoint}");

                // 3. отправить сообщение
                string message = "hello from FIRST udp instance";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                socket.SendTo(messageBytes, remoteEndPoint);
                Console.WriteLine($"[first]: sent '{message}'");

                // 4. получить сообщение
                byte[] buf = new byte[1024];
                int bytesRead = socket.Receive(buf);
                message = Encoding.UTF8.GetString(buf, 0, bytesRead);
                Console.WriteLine($"[first]: received '{message}'");
            } catch (Exception ex)
            {
                Console.WriteLine($"[first]: error '{ex.Message}'");
            } finally
            {
                // 5. завершить работу
                socket.Close();
                Console.WriteLine($"[first]: first udp instance completed");
            }
        }

        // RunSecondUdpInstance - процедура запуска второй стороны в udp-взаимодействии
        static void RunSecondUdpInstance(string localIpStr, int localPort, string remoteIpStr, int remotePort) 
        {
            Socket socket = null;
            try
            {
                // 1. создать и настроить необходимые объекты (сокет, endpoint-ы для взаимодействия)
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localIpStr), localPort);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpStr), remotePort);
                Console.WriteLine("[second]: socket and endpoints created");

                // 2. привязать сокет к endpoint-у для получения udp-сообщений
                socket.Bind(localEndPoint);
                Console.WriteLine($"[second]: socket binded to {localEndPoint}");

                // 3. получить сообщение
                byte[] buf = new byte[1024];
                int bytesRead = socket.Receive(buf);
                string message = Encoding.UTF8.GetString(buf, 0, bytesRead);
                Console.WriteLine($"[second]: received '{message}'");

                // 4. отправить сообщение
                message = "hello from SECOND udp instance";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                socket.SendTo(messageBytes, remoteEndPoint);
                Console.WriteLine($"[second]: sent '{message}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[second]: error '{ex.Message}'");
            }
            finally
            {
                // 5. завершить работу
                socket.Close();
                Console.WriteLine($"[second]: first udp instance completed");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start ...");
            Console.ReadKey(true);
            // ОБЩИЙ АЛГОРИТМ:
            // 1. Первая сторона отпралвяет сообщение второй
            // 2. Вторая получает сообщение от первой, отправляет ей ответ
            // 3. Первая сторона получает ответ от второй

            // РАБОТА ЧЕРЕЗ UDP

            // 0. задать ip-адреса и порты сторон
            string firstUdpInstanceIpStr = "127.0.0.1";
            int firstUdpInstancePort = 1024;
            string secondUdpInstanceIpStr = "127.0.0.1";
            int secondUdpInstancePort = 1024;

            // 1. создать потоки udp-сторон
            Thread firstUdpInstanceThread = new Thread(() => RunFirstUdpInstance(
                firstUdpInstanceIpStr, firstUdpInstancePort, 
                secondUdpInstanceIpStr, secondUdpInstancePort
            ));
            Thread secondUdpInstanceThread = new Thread(() => RunSecondUdpInstance(
                secondUdpInstanceIpStr, secondUdpInstancePort,
                firstUdpInstanceIpStr, firstUdpInstancePort
            ));

            // 2. запустить потоки
            firstUdpInstanceThread.Start();
            secondUdpInstanceThread.Start();

            // 3. ждать завершения потоков
            firstUdpInstanceThread.Join();
            secondUdpInstanceThread.Join();
        }
    }
}
