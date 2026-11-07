using System.Globalization;
using System.Numerics;

namespace AudioVideoTech
{
    // Ввод значений с консоли: каждый метод повторяет запрос до получения корректного значения.
    public static class ConsoleInput
    {
        // Читает непустую строку и отбрасывает пробелы в начале и в конце.
        public static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                string line = ReadLine(prompt);

                if (!string.IsNullOrWhiteSpace(line))
                {
                    return line.Trim();
                }

                Console.WriteLine("Значение не может быть пустым, повторите ввод.");
            }
        }

        // Читает целое число из диапазона [min; max].
        public static int ReadInt(string prompt, int min, int max)
        {
            return ReadNumber(prompt, min, max, NumberStyles.Integer, "целое число");
        }

        // Читает вещественное число из диапазона [min; max];
        // дробную часть можно отделять точкой или запятой.
        public static double ReadDouble(string prompt, double min, double max)
        {
            return ReadNumber(prompt, min, max, NumberStyles.Float, "число");
        }

        // Читает десятичное число из диапазона [min; max] (используется для цены).
        public static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            return ReadNumber(prompt, min, max, NumberStyles.Float, "число");
        }

        // Читает ответ «да/нет»: 1 - true, 0 - false.
        public static bool ReadBool(string prompt)
        {
            while (true)
            {
                string line = ReadLine(prompt).Trim();

                if (line == "1")
                {
                    return true;
                }

                if (line == "0")
                {
                    return false;
                }

                Console.WriteLine("Введите 1 (да) или 0 (нет).");
            }
        }

        /* Общий метод чтения числа любого числового типа T (int, double, decimal).
           Ограничение where T : INumber<T> разрешает разбирать T из строки (T.TryParse)
           и сравнивать его с границами. Запрос повторяется, пока число не попадёт в [min; max]. */
        private static T ReadNumber<T>(string prompt, T min, T max, NumberStyles style, string kind)
            where T : INumber<T>
        {
            while (true)
            {
                // Запятая заменяется точкой, чтобы принимались оба разделителя дробной части.
                string line = ReadLine(prompt).Replace(',', '.');

                if (T.TryParse(line, style, CultureInfo.InvariantCulture, out T? value)
                    && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine($"Введите {kind} в диапазоне [{min}; {max}].");
            }
        }

        /* Выводит приглашение и читает строку. Если поток ввода закрыт (Ctrl+Z),
           ReadLine() возвращает null, и метод выбрасывает EndOfStreamException.
           Завершать ли программу, решает вызывающий код (Main), а не этот класс. */
        private static string ReadLine(string prompt)
        {
            Console.Write(prompt);

            return Console.ReadLine() ?? throw new EndOfStreamException("Поток ввода закрыт.");
        }
    }
}
