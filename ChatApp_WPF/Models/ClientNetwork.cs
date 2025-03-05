using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp_WPF.Models
{

    #region //NetworkSetting
    public static class NetworkSetting{
        public static string IP = "127.0.0.1";
        public static int Port = 5000;
    }
    #endregion
    public class ClientNetwork
    {
        private static TcpClient _client;
        private static NetworkStream _stream;
        public static event Action<string>? MessageReceived;  // 事件：收到消息
        public event Action? ConnectionClosed;         // 事件：连接断开

        public async Task ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(NetworkSetting.IP, NetworkSetting.Port);
                _stream = _client.GetStream();
                _ = ListenForMessages();  // 开始监听消息
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Connection Failed: {ex.Message}");
            }
        }
        public async Task SendMessageAsync(string message)
        {
            if (_client == null || !_client.Connected) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Send Message Failed: {ex.Message}");
            }
        }
        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (_client.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        ConnectionClosed?.Invoke();
                        break;
                    }
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Trace.WriteLine(receivedMessage);
                    MessageReceived?.Invoke(receivedMessage);
                    
                    // 触发事件，通知 UI
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Receive Message Failed: {ex.Message}");
                Disconnect();
            }
            finally
            {
                MessageReceived = null;
            }
        }
        public void Disconnect()
        {
            if (_client != null)
            {
                _stream?.Close();
                _client?.Close();
                _client = null;
            }
            ConnectionClosed?.Invoke();
        }
    }

}
