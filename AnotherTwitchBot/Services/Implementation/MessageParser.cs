using System;
using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Models;
using AnotherTwitchBot.Services.Interfaces;

namespace AnotherTwitchBot.Services.Implementation
{
    public class MessageParser : IMessageParser
    {
        public IrcCommand GetCommandType(string message)
        {
            var messages = message.Split(" ");

            
            if (int.TryParse(messages[1], out int code))
            {
                return IrcCommand.UnknownCommand;
            }

            if (messages.Length > 2)
            {
                return GetIrcCommand(messages[1]);
            }
            
            return GetIrcCommand(messages[0]);
        }

        public UserMessageModel GetUserMessageModel(string message)
        {
            var intIndexParseSign = message.IndexOf(" :");
            var userName = GetUserName(message);
            var channel = GetChannel(message, intIndexParseSign);
            var userMessage = GetUserMessage(message, intIndexParseSign);

            return new UserMessageModel
            {
                UserName = userName,
                Channel = channel,
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
