namespace Cherry.MessageBus.Tests.Messages
{
    public static class MessageFormatter
    {
        public static string Hello(string name)
        {
            return "Hello " + name;
        }

        public static string HelloWorld(string name)
        {
            return "Hello World" + name;
        }
    }
}