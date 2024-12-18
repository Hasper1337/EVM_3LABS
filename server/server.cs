﻿using System; //lab 1
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    struct Data //структура с двумя полями
    {
        public int x;
        public float y;
    }

    internal class Program
    {
        static void Main()
        {
            var pipeServerStream = new NamedPipeServerStream("p", PipeDirection.InOut); //создание pipe-канала "p"
            Console.WriteLine("Ожидание клиента...");
            pipeServerStream.WaitForConnection();
            Console.WriteLine("Клиент подключен");

            while (true)
            {

                Console.WriteLine("Введите значение x: ");
                int valueX = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите значение y: ");
                float valueY = float.Parse(Console.ReadLine());

                var data = new Data { x = valueX, y = valueY }; //присваивание значений полей в новую структру

                byte[] buffer = new byte[Unsafe.SizeOf<Data>()]; //создание места для массива байтов по размеру структуры
                MemoryMarshal.Write(buffer, in data);  //запись массива байтов в буфер
                pipeServerStream.Write(buffer); //отправка
                Console.WriteLine($"Отправлено: x = {data.x}, y = {data.y}");

                byte[] responseBuffer = new byte[Unsafe.SizeOf<Data>()]; //создание места для массива байтов по размеру структуры от клиента
                int bytesRead = pipeServerStream.Read(responseBuffer); //чтение массива байтов
                if (bytesRead > 0)
                {
                    Data responseData = MemoryMarshal.Read<Data>(responseBuffer); //преобразование массива байтов в значения x, y 
                    Console.WriteLine($"Ответ получен: x = {responseData.x}, y = {responseData.y}");
                }
            }
        }
    }
}


//using System; //lab 2.02
//using System.IO.Pipes;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Threading;
//using System.IO;

//class Server
//{
//    static bool kostil = true;
//    static NamedPipeServerStream connection;
//    private static CancellationTokenSource cancellationTokenSource = new();
//    private static CancellationToken cancellationToken = cancellationTokenSource.Token;
//    static PriorityQueue<Message, int> prioritybuffer = new();

//    struct Message
//    {
//        public int first_num;
//        public int second_num;
//    }

//    static async Task SendMessage() // Отправка сообщений
//    {
//        if (prioritybuffer.Count == 0)
//        {
//            Console.WriteLine("Сообщений для отправки нет.");
//            return;
//        }

//        while (prioritybuffer.Count > 0)
//        {
//            var message = prioritybuffer.Dequeue();

//            byte[] messageBuffer = new byte[Unsafe.SizeOf<Message>()];
//            MemoryMarshal.Write(messageBuffer, in message);
//            //await connection.WriteAsync(buffer);
//            //Console.WriteLine($"Отправлено: x = {message.first_num}, y = {message.second_num}");

//            await connection.WriteAsync(messageBuffer, 0, messageBuffer.Length);
//            await connection.FlushAsync();

//            // Читаем ответ от клиента
//            byte[] buffer = new byte[Unsafe.SizeOf<Message>()];
//            connection.Read(buffer);
//            var receivedData = MemoryMarshal.Read<Message>(buffer);
//            Console.WriteLine($"Получено от клиента: x = {receivedData.first_num} , y =  {receivedData.second_num}");

//            //Console.WriteLine($"Получил от клиента: {receivedMessage}");
//        }
//    }

//    static async Task Start() // Запуск сервера
//    {
//        Console.WriteLine("Ожидание клиента...");
//        connection = new NamedPipeServerStream("p", PipeDirection.InOut);
//        await connection.WaitForConnectionAsync();
//        Console.WriteLine("Клиент подключен");
//        Console.WriteLine("Введите значения x, y по формату - x/ y/ приоритет:");
//        try
//        {
//            while (!cancellationToken.IsCancellationRequested)
//            {
//                var input = Console.ReadLine();
//                if (string.IsNullOrEmpty(input)) continue;

//                var part = input.Split("/");

//                if (part.Length != 3 || !int.TryParse(part[2].Trim(), out int priority))
//                {
//                    Console.WriteLine("Неправильный формат");
//                    continue;
//                }

//                int valueX = Convert.ToInt32(part[0].Trim());

//                int valueY = Convert.ToInt32(part[1].Trim());

//                var data = new Message { first_num = valueX, second_num = valueY };

//                prioritybuffer.Enqueue(data, priority);
//            }
//        }
//        catch (Exception ex) { Console.WriteLine("Остановка сервера"); }

//    }

//    static async Task Main() // Main
//    {
//        Console.CancelKeyPress += async (sender, e) =>
//        {
//            e.Cancel = true;

//            kostil = false;

//            await SendMessage();
//        };
//        await Start();
//    }
//}


//using System; //lab 3.00
//using System.IO.Pipes;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Threading;
//using System.IO;
//using System.Diagnostics;

//class Server
//{

//    static NamedPipeServerStream connection;
//    private static CancellationTokenSource cancellationTokenSource = new();
//    private static CancellationToken cancellationToken = cancellationTokenSource.Token;
//    static PriorityQueue<Message, int> prioritybuffer = new();


//    struct Message
//    {
//        public double first_num;
//        public double second_num;
//    }

//    static async Task SendMessage() // Отправка сообщений
//    {
//        if (prioritybuffer.Count == 0)
//        {
//            Console.WriteLine("Сообщений для отправки нет.");
//            return;
//        }

//        while (prioritybuffer.Count > 0)
//        {
//            var message = prioritybuffer.Dequeue();

//            byte[] messageBuffer = new byte[Unsafe.SizeOf<Message>()];
//            MemoryMarshal.Write(messageBuffer, in message);
//            //await connection.WriteAsync(buffer);
//            Console.WriteLine($"Отправлено: x = {message.first_num}, y = {message.second_num}");

//            await connection.WriteAsync(messageBuffer, 0, messageBuffer.Length);
//            await connection.FlushAsync();

//            // Читаем ответ от клиента
//            byte[] buffer = new byte[Unsafe.SizeOf<Message>()];
//            connection.Read(buffer);
//            var receivedData = MemoryMarshal.Read<Message>(buffer);
//            Console.WriteLine($"Получено от клиента: x = {receivedData.first_num}");

//            //Console.WriteLine($"Получил от клиента: {receivedMessage}");
//        }
//    }

//    static async Task Start() // Запуск сервера
//    {
//        Console.WriteLine("Ожидание клиента...");

//        using (Process ClientStart = new())
//        {
//            ClientStart.StartInfo.FileName = "E:\LAB_IVT_V1.00\bin\Debug\net8.0\\client.exe";
//            ClientStart.StartInfo.UseShellExecute = true;
//            ClientStart.Start();
//        }

//        connection = new NamedPipeServerStream("p", PipeDirection.InOut);
//        await connection.WaitForConnectionAsync();
//        Console.WriteLine("Клиент подключен");
//        Console.WriteLine("Введите значения x, y по формату - (x/ y/ приоритет), для вычисления интеграла -2*ln(x):");
//        try
//        {
//            while (!cancellationToken.IsCancellationRequested)
//            {
//                var input = Console.ReadLine();
//                if (string.IsNullOrEmpty(input)) continue;

//                var part = input.Split("/");

//                if (part.Length != 3 || !int.TryParse(part[2].Trim(), out int priority))
//                {
//                    Console.WriteLine("Неправильный формат");
//                    continue;
//                }

//                double valueX = Convert.ToDouble(part[0].Trim());

//                double valueY = Convert.ToDouble(part[1].Trim());

//                var data = new Message { first_num = valueX, second_num = valueY };

//                prioritybuffer.Enqueue(data, priority);
//            }
//        }
//        catch (Exception ex) { Console.WriteLine("Остановка сервера"); }

//    }

//    static async Task Main() // Main
//    {
//        Console.CancelKeyPress += async (sender, e) =>
//        {
//            e.Cancel = true;
//            await SendMessage();
//        };
//        await Start();
//    }
//}