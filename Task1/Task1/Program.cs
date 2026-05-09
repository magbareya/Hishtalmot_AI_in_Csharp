namespace AIChat
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point of the Windows Forms application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
