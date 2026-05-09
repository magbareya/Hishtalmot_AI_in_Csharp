namespace AIChat
{
    public class UtilityTools
    {
        private readonly Random random = new();

        public string GetRandomNumber()
        {
            return random.Next(1, 101).ToString();
        }

        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
