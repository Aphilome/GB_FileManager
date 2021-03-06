# GB_FileManager

Консольное приложение состоит из 5 визуально разделенных областей:
- область заголовка, в качестве которого является текущая директория;
- область дерева каталогов;
- область информации о директории или файле;
- область ввода команды;
- область информации об ошибке.

Команды следует вводить, разделяя аргументы через пробел. В некоторых ситуациях будет отображена информация о случившейся ошибки в нижней части окна, а в некоторых некорректных командах - игнорируется.

В приложении реализованы следующие команды:
- "tree". Установка текущей директории и перестроение дерева. Пример "tree Users\Dir".
- "left". Пейджинг влево. Аргументы игнорируются.
- "right". Пейджинг вправо. Аргументы игнорируются.
- "copy". Копирование из исходной пути в заданную файла или директории (рекурсивно). Пример "copy a.txt bbb\c.txt".
- "delete". Удаление файла или папки (рекурсивно). Пример "delete a.txt".
- "info". Отображение информации о файле или папке. Пример "info myfolder".
- "exit". Завершено программы с сохранением состояния.

При запуске приложения произойдет попытка десериализации конфигурационного файла config.json. При неудаче будут выставлены значения по умолчанию: Рабочий каталог - домашняя папка пользователя, размер пейджинга 20. Есть возможность вручную менять файл конфигурации.
Размер пейджинга можно задавать от 1 до 20. Сохранённая позиция текущей страницы не может быть отрицательной. Сохранённый рабочий каталог не может не существовать. При наличии ошибки конфигурационный файл будет проигнорирован.
