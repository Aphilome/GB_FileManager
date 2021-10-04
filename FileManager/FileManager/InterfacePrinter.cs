using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace FileManager
{
    public class InterfacePrinter
    {
        // высота части окна с заголовком
        private const int TitleWindowHeight = 1;
        // высота части окна с деревом
        private const int TreeWindowHeight = 20;
        // высота части окна с информацией
        private const int InfoWindowHeight = 5;
        // высота части окна с командой
        private const int CmdWindowHeight = 1;
        // высота части окна с текстом ошибки
        private const int ErrorWindowHeight = 1;
        // ширина окна
        private const int WindowWidth = 120;
        // печатель дерева каталогов
        private TreeViewer _treeViewer;
        
        // файл сохранения
        private string SaveFile { get; set; } = "config.json";
        // рабочая папка
        private string WorkDir { get; set; }
        // текущее дерево
        private string[] CurrentTree { get; set; }
        // размер страницы
        private int PageSize { get; set; }
        // текущая стараница
        private int CurrentPage { get; set; }
        
        // конструктор
        public InterfacePrinter()
        {
            _treeViewer = new TreeViewer();
            PageSize = TreeWindowHeight;
            CurrentPage = 0;
        }
        
        // инициализация консоли
        public void Init()
        {
            Console.Clear();
            Console.SetWindowSize(WindowWidth,
                TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1 + CmdWindowHeight + 1 + ErrorWindowHeight + 1);
            Console.SetBufferSize(WindowWidth,
                TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1 + CmdWindowHeight + 1 + ErrorWindowHeight + 1);
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            WorkDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SaveFile = Path.Combine(Directory.GetCurrentDirectory(), SaveFile);
            Directory.SetCurrentDirectory(WorkDir);
        }

        // попытаться загрузить
        public void TryLoad()
        {
            if (!File.Exists(SaveFile))
                return;
            try
            {
                var jsonString = File.ReadAllText(SaveFile);
                var save = JsonSerializer.Deserialize<Save>(jsonString);
                if (save == null || !Directory.Exists(save.WorkingDir) || 
                    save.CurrentPage < 0 || save.PageSize < 0)
                    return;
                WorkDir = save.WorkingDir;
                CurrentPage = save.CurrentPage;
                PageSize = Math.Min(TreeWindowHeight, save.PageSize);
            }
            catch (Exception)
            {
                // 
            }
        }

        public void TrySave()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(new Save
                {
                    WorkingDir = WorkDir,
                    CurrentPage = CurrentPage,
                    PageSize = PageSize
                });
                File.WriteAllText(SaveFile, jsonString);
            }
            catch (Exception)
            {
                //
            }
        }
        
        // напечатать все окно
        public void PrintAll()
        {
            Console.Clear();
            PrintTitle();
            PrintSeparator(1);
            PrintTree();
            PrintSeparator(TitleWindowHeight + 1 + TreeWindowHeight);
            PrintInfo(WorkDir);
            PrintSeparator(TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight);
            PrintCmd();
            PrintSeparator(TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1 + CmdWindowHeight);
            PrintError("");
            SetCursorPositionForReadingCmd();
        }

        // установить новое рабочее дерево
        public void SetNewWorkDir(string arg)
        {
            if (!Directory.Exists(arg))
                return;
            var info = new DirectoryInfo(arg);
            WorkDir = info.FullName;
            CurrentPage = 0;
            Directory.SetCurrentDirectory(arg);
            PrintTitle();   
            PrintTree();
            PrintInfo(WorkDir);
        }
        
        // пейджинг влево
        public void PagingLeft()
        {
            CurrentPage = Math.Max(0, CurrentPage - 1);
        }

        // пейджинг вправо
        public void PagingRight()
        {
            var maxPages = CurrentTree.Length / PageSize;
            if (CurrentTree.Length / (double)PageSize - maxPages > 0)
                maxPages++;
            CurrentPage = Math.Min(maxPages, CurrentPage + 1);
        }
        
        // печать секции дерева
        public void PrintTree()
        {
            Console.SetCursorPosition(0, TitleWindowHeight + 1);
            Console.Write(new string(' ', TreeWindowHeight * WindowWidth));
            Console.SetCursorPosition(0, TitleWindowHeight + 1);
            CurrentTree = _treeViewer.GetDirectoryTree(WorkDir);
            foreach (var i in CurrentTree.Skip(PageSize * CurrentPage).Take(PageSize))
                Console.WriteLine(i);
        }
        
        // печать секции информации
        public void PrintInfo(string path)
        {
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1);
            Console.Write(new string(' ', InfoWindowHeight * WindowWidth));
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1);

            try
            {
                if (File.Exists(path))
                {
                    var info = new FileInfo(path);
                    Console.Write($"Full name: {info.FullName}\n" +
                                  $"Size: {info.Length} bytes\n" +
                                  $"Created: {info.CreationTime.ToShortDateString()}\n" +
                                  $"Modified: {info.LastWriteTime.ToShortDateString()}\n" +
                                  $"Attributes: {info.Attributes.ToString()}"
                    );
                }
                else if (Directory.Exists(path))
                {
                    var info = new DirectoryInfo(path);
                    Console.Write($"Full name: {info.FullName}\n" +
                                  $"Created: {info.CreationTime.ToShortDateString()}\n" +
                                  $"Modified: {info.LastWriteTime.ToShortDateString()}\n" +
                                  $"Attributes: {info.Attributes.ToString()}"
                    );
                }
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }
        
        // печать секции команды
        public void PrintCmd()
        {
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1);
            Console.Write(new string(' ', CmdWindowHeight * WindowWidth));
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1);
            Console.Write("> ");
        }

        // печать секции ошибок
        public void PrintError(string message)
        {
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1 + CmdWindowHeight + 1);
            Console.Write(new string(' ',  ErrorWindowHeight * WindowWidth));
            Console.SetCursorPosition(0, TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1 + CmdWindowHeight + 1);
            Console.Write(message);
        }

        // установить место для корректного чтения команды
        private void SetCursorPositionForReadingCmd()
        {
            Console.SetCursorPosition(2, TitleWindowHeight + 1 + TreeWindowHeight + 1 + InfoWindowHeight + 1);
        }
        
        // печатель разделителей частей окна
        private void PrintSeparator(int top)
        {
            Console.SetCursorPosition(0, top);
            Console.Write(new string('=', WindowWidth));
        }
        
        // печать заголовка
        private void PrintTitle()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', TitleWindowHeight * WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.Write($"--- {WorkDir}");
        }
    }
}