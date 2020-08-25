== ОБЩАЯ ИНФОРМАЦИЯ ==

Проект сделан в Visual Studio 2019 для .NET Core 3.1 (необходим .NET Core SDK 3.1 или старше)
Варианты запуска:
1. Непосредсвенно из студии
2. Сбилдить под конкретную платформу.

== ПРИМЕРЫ == 

Сбилдить под 64-х разрядную window8 
Собирается как self-contained, single executable т.е. внешние зависимости (.net core, sdk) для запуска не нужны.
>> publish.bat win81-x64

Генерация файла ~10Gb
>> .\bin\win81-x64\Altium.Generator.exe 8000000000 d:\tmp\10Gb.txt

Сортировка файла
>> .\bin\win81-x64\Altium.Sorter.exe d:\tmp\10Gb.txt d:\tmp\10Gb.txt d:\tmp\10Gb.sorted.txt

== МОЕ ОКРУЖЕНИЕ ==
Intel Core i5-8250U 1.4Gz, RAM 16Gb, HDD

Собранно под win81-x64
Генерация ~2мин 20сек
Сортировка ~14мин 10сек
