# Задание

    Разработать консольную программу, обладающую след функционалом:
    -на вход программе подается список url
    -на выходе файл, где напротив каждого url указано кол внешних ссылок на этой странице

    Требования к программе:
    -асинхронная работа;
    -данные должны писаться в выходной файл по мере обработки (не сразу в куче)
    -после окончания обработки файл должен быть упорядочен в соотв с исходным файлом
    -по мере обработки в консоль выводится инфа (сколько осталось, сколько обработано)
    -все http ошибки должны сохранятся в логе, с указанием адреса страницы и кода ошибки

    Все что не указано в требованиях - делайте на свое усмотрение.

# Инструкция для Windows

1. Выкачать репозитарий
1. Открыть `CrawlerTask/CrawlerTask.sln` в Visual Studio 2017 (я использовал версию 15.7.1)
1. Выбрать конфигурацию Release, выполнить Rebuild Solution
1. Запустить `cmd.exe`
1. Перейти в каталог `CrawlerTask\Crawler\bin\Release`
1. Выполнить `Crawler.exe -i ..\..\..\urls.txt -o results.txt`
1. В текущем каталоге, в файле с раширением `.log` будет журнал ошибок работы приложения.
1. В текущем каталоге, в файле `results.txt` должно быть примерно следующее (перед двоеточием кол-во внешних ссылок, после двоеточия исходный URL):
<pre>
-: http://www.ya.ru/blin
16: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-create-a-file-or-folder
1: https://www.ya.ru/
1: https://www.wikipedia.org/
22: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file
1: https://www.microsoft.com/
-: https://www.apple.ru/
1: https://go.mysku.ru/?r=https%3A%2F%2Fwww.ya.ru%2F
-: https://tfs.ckline.ru/tfs
</pre>

# Инструкция для Linux

1. Установить .NET Core 2.0
1. Выполнить `git clone https://github.com/cbelyaev/CrawlerTask.git`
1. Выполнить `cd CrawlerTask/CrawlerCore`
1. Выполнить `dotnet run -- -i ../urls.txt -o results.txt`
1. В подкаталоге `bin/Debug/netcoreapp2.0`, в файле с раширением `.log` будет журнал ошибок работы приложения.
1. В текущем каталоге, в файле `results.txt` должно быть примерно следующее (перед двоеточием кол-во внешних ссылок, после двоеточия исходный URL):
<pre>
-: http://www.ya.ru/blin
16: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-create-a-file-or-folder
1: https://www.ya.ru/
1: https://www.wikipedia.org/
22: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file
1: https://www.microsoft.com/
-: https://www.apple.ru/
1: https://go.mysku.ru/?r=https%3A%2F%2Fwww.ya.ru%2F
-: https://tfs.ckline.ru/tfs
</pre>

# Ключи командной строки:
- -h - отображение справки
- -i *file* - файл со ссылками, в каждой строке - одна ссылка
- -o *file* - Файл с результатами
- -j *maxThreads* - [необязательно] максимальное кол-во потоков обработки, от 0 до 1000, при отсутствии или 0 будет использоваться кол-во доступных ядер CPU
- -v - повышение "болтливости", кроме кол-ва завершённых заданий, будет отображён и результат выполнения задания
