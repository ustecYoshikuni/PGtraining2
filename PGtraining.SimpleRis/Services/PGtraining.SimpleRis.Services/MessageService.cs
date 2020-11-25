using PGtraining.SimpleRis.Services.Interfaces;

namespace PGtraining.SimpleRis.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
