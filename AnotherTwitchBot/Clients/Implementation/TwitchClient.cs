using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Options;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot.Clients.Implementation
{
    public class TwitchClient : ITwitchClient
    {
        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private NetworkStream _networkStream;
        private readonly TwitchConfig _twitchConfig;


        public TwitchClient(IOptions<TwitchConfig> twitchConfigOptions)
        {
            try
            {
                _twitchConfig = twitchConfigOptions.Value;

                _tcpClient = new TcpClient(_twitchConfig.Server, _twitchConfig.Port);
                _networkStream = _tcpClient.GetStream();
                _inputStream = new StreamReader(_networkStream);
                _outputStream = new StreamWriter(_networkStream) { NewLine = "\r\n", AutoFlush = true }; ;


                // Try to join the room
                _outputStream.WriteLine("PASS " + _twitchConfig.OAuthToken);
                _outputStream.WriteLine("NICK " + _twitchConfig.UserName);
                _outputStream.WriteLine("USER " + _twitchConfig.UserName + " 8 * :" + _twitchConfig.UserName);
                _outputStream.WriteLine("JOIN #" + _twitchConfig.Channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
     
        public void SendIrcMessage(string message)
        {
            try
            {
                _outputStream.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendPublicChatMessage(string message)
        {
            try
            {
                SendIrcMessage(":" + _twitchConfig.UserName + "!" + _twitchConfig.UserName + "@" + _twitchConfig.UserName +
                               ".twitchClient.chat.twitchClient.tv PRIVMSG #" + _twitchConfig.Channel + " :" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> ReadMessageAsync()
        {
            try
            {
                if (_networkStream.DataAvailable)
                {
                    return await _inputStream.ReadLineAsync();
                }

                await Task.Delay(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error receiving message: " + ex.Message;
            }
        }
    }
}