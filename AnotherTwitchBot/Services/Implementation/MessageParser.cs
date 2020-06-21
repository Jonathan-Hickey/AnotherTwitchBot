using System;
using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Models;
using AnotherTwitchBot.Services.Interfaces;

namespace AnotherTwitchBot.Services.Implementation
{
    public class MessageParser : IMessageParser
    {
        
        public TwitchClientCommand GetClientCommand(string message)
        {
            var messages = message.Split(" ");

            if(int.TryParse(messages[1],out int code ))
            {
                return null;
            }

            var ircCommand = GetIrcCommand(messages[1]);

            if (ircCommand != IrcCommand.PrivateMessage)
            {
                return null;
            }

            var intIndexParseSign = message.IndexOf(" :");
            var userName = GetUserName(message);
            var channel = GetChannel(message, intIndexParseSign);
            var userMessage = GetUserMessage(message, intIndexParseSign);

            return new TwitchClientCommand
            {
                UserName = userName,
                Channel = channel,
                Command = ircCommand,
                UserMessage = userMessage
            };
        }

        private static string GetUserMessage(string message, int intIndexParseSign)
        {
            return message.Substring(intIndexParseSign + 2);
        }

        private static string GetChannel(string message, int intIndexParseSign)
        {
            var startIndex = message.IndexOf('#') + 1;
            var length = intIndexParseSign - startIndex;
            return message.Substring(startIndex, length);
        }

        private static string GetUserName(string message)
        {
            var start = message.IndexOf(':') + 1;
            var length = message.IndexOf('!') - start;
            return message.Substring(start, length );
        }


        private IrcCommand GetIrcCommand(string command)
        {
            switch (command)
            {
                case InternetRelayChatCommands.PrivateMessage:
                    return IrcCommand.PrivateMessage;
                case InternetRelayChatCommands.Join:
                    return IrcCommand.Join;
                case InternetRelayChatCommands.Ping:
                    return IrcCommand.Ping;
                case InternetRelayChatCommands.Pong:
                    return IrcCommand.Pong;
                default:
                    throw new ArgumentException("Unknown command {0}", command);
            }
        }
    }
}
