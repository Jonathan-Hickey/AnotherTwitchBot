using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Options;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot.Clients.Implementation
{
    public class TwitchIrcClient : ITwitchIrcClient
    {
        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private NetworkStream _networkStream;
        private readonly TwitchIrcConfig _twitchIrcConfig;

        public TwitchIrcClient(IOptions<TwitchIrcConfig> twitchConfigOptions)
        {
            try
            {
                _twitchIrcConfig = twitchConfigOptions.Value;

                _tcpClient = new TcpClient(_twitchIrcConfig.Server, _twitchIrcConfig.Port);
                _networkStream = _tcpClient.GetStream();
                _inputStream = new StreamReader(_networkStream);
                _outputStream = new StreamWriter(_networkStream) { NewLine = "\r\n", AutoFlush = true }; ;

                // Try to join the room
                _outputStream.WriteLine("PASS " + _twitchIrcConfig.OAuthToken);
                _outputStream.WriteLine("NICK " + _twitchIrcConfig.UserName);
                _outputStream.WriteLine("USER " + _twitchIrcConfig.UserName + " 8 * :" + _twitchIrcConfig.UserName);
                _outputStream.WriteLine("JOIN #" + _twitchIrcConfig.Channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
     
        public Task SendIrcMessageAsync(string message)
        {
            try
            {
                return _outputStream.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.CompletedTask;
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

        public async Task SendPublicChatMessageAsync(string message)
        {
            try
            {
                await SendIrcMessageAsync(":" + _twitchIrcConfig.UserName + "!" + _twitchIrcConfig.UserName + "@" + _twitchIrcConfig.UserName +
                               ".twitchIrcClient.chat.twitchIrcClient.tv PRIVMSG #" + _twitchIrcConfig.Channel + " :" + message);
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