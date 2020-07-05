using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Services.Implementation;
using AnotherTwitchBot.Services.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace AnotherTwitchBotTests.Services
{
    [TestFixture]
    public class MessageParserTests
    {

        [Test]
        public void When_Received_Then_ParseMessage_IntoCommand()
        {
            var message = ":jonathan9375!jonathan9375 @jonathan9375.tmi.twitch.tv PRIVMSG #jonathan9375 :hello world";

            IMessageParser messageParser = new MessageParser();

            var command = messageParser.GetUserMessageModel(message);

            command.Should().NotBeNull();
            command.Channel.Should().BeEquivalentTo("jonathan9375");
            command.UserName.Should().BeEquivalentTo("jonathan9375");
            command.UserMessage.Should().BeEquivalentTo("hello world");

        }
    }
}