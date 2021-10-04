using System;
using System.IO;

namespace FileManager
{
    class Program
    {
        static void Main()
        {
            var commander = new Commander();
            try
            {
                commander.Run();
            }
            catch (Exception e)
            {
                File.WriteAllText(nameof(e), e.StackTrace);
            }
        }
    }
}