using System;
using System.Linq;

namespace FileManager
{
    public class InterfaceReader
    {
        // считать ввод пользователя
        public Command ReadCommand()
        {
            var input = Console.ReadLine();
            var cmd = new Command();
            if (string.IsNullOrWhiteSpace(input))
                return null;
            var s = input.Split(' ');
            if (s.Length == 0)
                return null;
            cmd.Name = GetCommandEnum(s[0]);
            if (cmd.Name == CommandEnum.None)
                return null;
            cmd.Args = s.Skip(1).ToArray();
            return cmd;
        }

        // преобразовать строку в перечисление
        private CommandEnum GetCommandEnum(string s)
        {
            switch (s)
            {
                case "tree":
                    return CommandEnum.Tree;
                case "left":
                    return CommandEnum.Left;
                case "right":
                    return CommandEnum.Right;
                case "copy":
                    return CommandEnum.Copy;
                case "delete":
                    return CommandEnum.Delete;
                case "info":
                    return CommandEnum.Info;
                case "exit":
                    return CommandEnum.Exit;
                default:
                    return CommandEnum.None;
            }
        }
    }
}