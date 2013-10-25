namespace Cherry.MessageBus.Tests.Messages
{
    public class HelloWorldMessage : HelloMessage
    {
        public HelloWorldMessage(string name) : base(name)
        {
        }

        public override string Text
        {
            get
            {
                return MessageFormatter.HelloWorld(_name);
            }
        }
    }
}
