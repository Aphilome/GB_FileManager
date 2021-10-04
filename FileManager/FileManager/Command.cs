namespace FileManager
{
    public enum CommandEnum
    {
        // неизвестно
        None,
        // отобразить дерево
        Tree,
        // пейджинг влево
        Left,
        // пейджинг вправо
        Right,
        // копирование
        Copy,
        // удаление
        Delete,
        // информация
        Info,
        // выход
        Exit
    }
    
    public class Command
    {
        public CommandEnum Name { get; set; }
        
        public string[] Args { get; set; }
    }
}