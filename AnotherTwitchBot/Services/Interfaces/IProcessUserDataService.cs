using System.Threading.Tasks;

namespace AnotherTwitchBot.Services.Interfaces
{
    public interface IProcessUserDataService
    {
        Task Process(string message);
    }
}