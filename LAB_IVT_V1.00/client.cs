﻿//using System; //lab 1, 2.02
//using System.IO.Pipes;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//namespace Client
//{
//    struct Data
//    {
//        public int first_num;
//        public int second_num;
//    }

//    class Program
//    {
//        static void Main()
//        {
//            Console.WriteLine("Соединение с сервером...");
//            var pipeStream = new NamedPipeClientStream(".", "p", PipeDirection.InOut); //подключение к pipe-каналу "p"
//            pipeStream.Connect();
//            Console.WriteLine("Подключено к серверу.");
//            Console.WriteLine("Ожидание данных...");
//            while (true)
//            {
//                byte[] buffer = new byte[Unsafe.SizeOf<Data>()];
//                pipeStream.Read(buffer);
//                var receivedData = MemoryMarshal.Read<Data>(buffer);
//                Console.WriteLine($"Получено от сервера: x = {receivedData.first_num}, y =  {receivedData.second_num}");

//                //double result = Calculate(receivedData.first_num, receivedData.second_num, 1000);

//                receivedData.second_num *= 3;
//                Console.WriteLine($"Отправлено: x = {receivedData.first_num}, y = {receivedData.second_num}");

//                byte[] responseBuffer = new byte[Unsafe.SizeOf<Data>()];
//                MemoryMarshal.Write(responseBuffer, in receivedData);
//                pipeStream.Write(responseBuffer);
//            }
//        }
//    }
//}

using System; //lab 3
using System.Globalization;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Client
{

    struct Message
    {
        public double first_num;
        public double second_num;
    }

    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Соединение с сервером...");
            var pipeStream = new NamedPipeClientStream(".", "p", PipeDirection.InOut); //подключение к pipe-каналу "p"
            pipeStream.Connect();
            Console.WriteLine("Подключено к серверу.");
            Console.WriteLine("Ожидание данных...");
            while (true)
            {
                byte[] buffer = new byte[Unsafe.SizeOf<Message>()];
                pipeStream.Read(buffer);
                var receivedData = MemoryMarshal.Read<Message>(buffer);
                //var input = Console.ReadLine();
                //if (string.IsNullOrEmpty(input)) continue;

                //var part = input.Split(" ");
                //string first = part[0].Trim();
                //string second = part[1].Trim();

                //if (!double.TryParse(first, NumberStyles.Any, CultureInfo.InvariantCulture, out double x) || 
                //    !double.TryParse(second, NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                //{
                //    Console.WriteLine($"Ошибка {first}, {second}");
                //    return;
                //}
                //Console.WriteLine($"{receivedData.first_num}, y =  {receivedData.second_num}");

                double result = Calculate(receivedData.first_num, receivedData.second_num, 2000);


                byte[] responseBuffer = BitConverter.GetBytes(result);
                pipeStream.Write(responseBuffer, 0, responseBuffer.Length);
                Console.WriteLine($"Отправлено: результат = {result}");
                //byte[] responseBuffer = new byte[Unsafe.SizeOf<Message>()];
                //MemoryMarshal.Write(responseBuffer, in result);
                //pipeStream.Write(responseBuffer);
                //Console.WriteLine($"Отправлено: x = {receivedData.first_num}, y = {receivedData.second_num}, result = {responseBuffer}");
            }
        }

        static double Calculate(double x, double y, int n)
        {
            double rez = 0;
            double h = (y - x) / n;
            double sum = 0.0;

            for (int i = 0; i < n; i++)
            {
                double z = x + i * h;
                double p = x + (i + 1) * h;
                sum += ((-2 * Math.Log(z)) + (-2 * Math.Log(p)));
            }

            rez += sum * h / 2.0;
            return rez;
        }
    }
}


