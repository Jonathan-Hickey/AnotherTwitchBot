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
            try
            {
                var intIndexParseSign = message.IndexOf(" :");
                var userName = GetUserName(message);
                var channel = GetChannel(message, intIndexParseSign);
                var userMessage = GetUserMessage(message, intIndexParseSign);
                var messages = message.Split(" ");

                var ircCommand = GetIrcCommand(messages[1]);

                return new TwitchClientCommand
                {
                    UserName = userName,
                    Channel = channel,
                    Command = ircCommand,
                    UserMessage = userMessage
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

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
                default:
                    throw new ArgumentException("Unknown command {0}", command);
            }
        }
    }
}
