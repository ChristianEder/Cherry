namespace Cherry.MessageBus.Tests.Messages
{
    public class HelloMessage
    {
        protected readonly string _name;

        public HelloMessage(string name)
        {
            _name = name;
        }

        public virtual string Text
        {
            get
            {
                return MessageFormatter.Hello(_name);
            }
        }
    }
}