using System;
using System.IO;

namespace FileManager
{
    public class Commander
    {
        // объекта для печати
        private InterfacePrinter Printer { get; set; }
        // объект для чтения команды
        private InterfaceReader Reader { get; set; }

        // конструктор
        public Commander()
        {
            Printer = new InterfacePrinter();
            Reader = new InterfaceReader();
            Printer.Init();
            Printer.TryLoad();
            Printer.PrintAll();
        }
        
        // запуск 
        public void Run()
        {
            while (true)
            {
                // подготовить зону вводу команды
                Printer.PrintCmd();
                // ввели команду
                var cmd = Reader.ReadCommand();
                // сброс сообщения ошибки
                Printer.PrintError("");
                if (cmd == null)
                    continue;
                // распределитель команды
                switch (cmd.Name)
                {
                    case CommandEnum.Tree:
                    {
                        SetWorkDir(cmd);
                        break;
                    }
                    case CommandEnum.Copy:
                    {
                        Copy(cmd);
                        break;
                    }
                    case CommandEnum.Delete:
                    {
                        Delete(cmd);
                        break;
                    }
                    case CommandEnum.Left:
                    {
                        Printer.PagingLeft();
                        Printer.PrintTree();
                        break;
                    }
                    case CommandEnum.Right:
                    {
                        Printer.PagingRight();
                        Printer.PrintTree();
                        break;
                    }
                    case CommandEnum.Info:
                    {
                        Info(cmd);
                        break;
                    }
                    case CommandEnum.Exit:
                    {
                        Printer.TrySave();
                        return;
                    }
                }
            }
        }

        // установить новое рабочее дерево
        private void SetWorkDir(Command cmd)
        {
            if (cmd.Args.Length >= 1)
                Printer.SetNewWorkDir(cmd.Args[0]);
        }
        
        private void Copy(Command cmd)
        {
            if (cmd.Args.Length >= 2)
            {
                if (File.Exists(cmd.Args[0]))
                {
                    try
                    {
                        File.Copy(cmd.Args[0], cmd.Args[1]);
                    }
                    catch (Exception e)
                    {
                        Printer.PrintError(e.Message);
                    }
                                
                }
                else if (Directory.Exists(cmd.Args[0]))
                {
                    try
                    {
                        DirectoryCopy(cmd.Args[0], cmd.Args[1]);
                    }
                    catch (Exception e)
                    {
                        Printer.PrintError(e.Message);
                    }
                }
                else
                    Printer.PrintError("It's not file or directory");
            }
        }

        private void Delete(Command cmd)
        {
            if (cmd.Args.Length >= 1)
            {
                if (File.Exists(cmd.Args[0]))
                {
                    try
                    {
                        File.Delete(cmd.Args[0]);
                    }
                    catch (Exception e)
                    {
                        Printer.PrintError(e.Message);
                    }
                }
                else if (Directory.Exists(cmd.Args[0]))
                {
                    try
                    {
                        Directory.Delete(cmd.Args[0], true);
                    }
                    catch (Exception e)
                    {
                        Printer.PrintError(e.Message);
                    }
                }
                else
                    Printer.PrintError("It's not file or directory");
            }
        }

        private void Info(Command cmd)
        {
            if (cmd.Args.Length >= 1)
            {
                if (File.Exists(cmd.Args[0]) || Directory.Exists(cmd.Args[0]))
                    Printer.PrintInfo(cmd.Args[0]);
                else
                    Printer.PrintError("It's not file or directory");
            }
        }
        
        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();
            var files = dir.GetFiles();
            Directory.CreateDirectory(destDirName);
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
            foreach (var subDir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subDir.Name);
                DirectoryCopy(subDir.FullName, tempPath);
            }
        }
    }
}