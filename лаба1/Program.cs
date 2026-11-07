using System.Text;

namespace AudioVideoTech
{
    // Тестирующая программа: демонстрация полиморфизма на коллекции устройств и интерактивное меню.
    public static class Program
    {
        // Модели демонстрационных устройств, которые изменяются в ShowDemo.
        private const string SamsungTvModel = "UE55AU7100";
        private const string LgTvModel = "32LQ570B";
        private const string SonyRadioModel = "ICF-P36";

        // Подсказка к вопросам «да/нет».
        private const string YesNoHint = " (1 — да / 0 — нет): ";

        /* Тип устройства, который можно добавить через меню:
           Title - название пункта меню, Create - метод, создающий объект
           по общим характеристикам (остальные характеристики он запрашивает сам). */
        private sealed record DeviceKind(string Title, Func<string, string, decimal, AudioVideoDevice> Create);

        // Пункты меню выбора типа. Чтобы добавить новый тип, достаточно добавить строку сюда.
        private static readonly DeviceKind[] DeviceKinds =
        {
            new("AudioVideoDevice (базовый)", (brand, model, price) => new AudioVideoDevice(brand, model, price)),
            new("Television (телевизор)", CreateTelevision),
            new("RadioReceiver (радиоприёмник)", CreateRadioReceiver)
        };

        /* Точка входа: создаёт демонстрационный набор, показывает демонстрацию
           и запускает меню. Если ввод прерван (Ctrl+Z), перехватывает
           EndOfStreamException и корректно завершает работу. */
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<AudioVideoDevice> devices = CreateDemoDevices();

            try
            {
                ShowDemo(devices);
                RunMenu(devices);
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine();
                Console.WriteLine("Ввод прерван, работа завершена.");
                return;
            }

            Console.WriteLine("Работа завершена.");
        }

        /* Создаёт список из пяти устройств всех трёх классов.
           Тип элементов списка - базовый класс, а объекты в нём разных классов. */
        private static List<AudioVideoDevice> CreateDemoDevices()
        {
            return new List<AudioVideoDevice>
            {
                new Television("Samsung", SamsungTvModel, 49990m, 55, true),
                new Television("LG", LgTvModel, 17500m, 32, false),
                new RadioReceiver("Sony", SonyRadioModel, 3200m, 87.5, 108.0, false),
                new RadioReceiver("Philips", "TAR3205", 4500m, 87.5, 108.0, true),
                new AudioVideoDevice("Generic", "AVD-100", 9999m)
            };
        }

        // Демонстрация: выводит список, меняет характеристики через свойства,
        // вызывает виртуальный метод через ссылку базового типа и выводит статистику.
        private static void ShowDemo(List<AudioVideoDevice> devices)
        {
            Console.WriteLine("=== Исходные объекты (полиморфный вызов ToString) ===");
            PrintDevices(devices);

            Console.WriteLine();
            Console.WriteLine("=== Изменение характеристик через свойства ===");

            // Устройства ищутся по модели, а не по индексу, чтобы не зависеть от порядка в списке.
            AudioVideoDevice? samsungTv = devices.Find(d => d.Model == SamsungTvModel);
            if (samsungTv != null)
            {
                samsungTv.Price = 45000m;
            }

            if (devices.Find(d => d.Model == LgTvModel) is Television lgTv)
            {
                lgTv.ScreenInch = 43;
                lgTv.IsSmartTv = true;
            }

            if (devices.Find(d => d.Model == SonyRadioModel) is RadioReceiver sonyRadio)
            {
                sonyRadio.HasBluetooth = true;
                sonyRadio.SetFrequencyRange(64.0, 108.0);
            }

            PrintDevices(devices);

            Console.WriteLine();
            Console.WriteLine("=== Вызов виртуального метода через ссылку базового типа ===");
            foreach (AudioVideoDevice device in devices)
            {
                string specifications = device.GetSpecifications();
                Console.WriteLine($"{device.GetType().Name,-16} -> " +
                                  (specifications.Length == 0 ? "(нет своих характеристик)" : specifications));
            }

            Console.WriteLine();
            Console.WriteLine("=== Статистика по коллекции ===");
            Console.WriteLine($"Всего устройств: {devices.Count}");
            Console.WriteLine($"Телевизоров: {devices.OfType<Television>().Count()}");
            Console.WriteLine($"Радиоприёмников: {devices.OfType<RadioReceiver>().Count()}");
            Console.WriteLine($"Суммарная стоимость: {devices.Sum(d => d.Price):N2} руб.");
        }

        // Главное меню: добавление, вывод и удаление устройств. Повторяется до выбора 0.
        private static void RunMenu(List<AudioVideoDevice> devices)
        {
            int action;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Действие:");
                Console.WriteLine("  1 — добавить устройство");
                Console.WriteLine("  2 — вывести список");
                Console.WriteLine("  3 — удалить устройство по номеру");
                Console.WriteLine("  0 — завершить");

                action = ConsoleInput.ReadInt("Ваш выбор: ", 0, 3);

                switch (action)
                {
                    case 1:
                        AddDevice(devices);
                        break;
                    case 2:
                        PrintDevices(devices);
                        break;
                    case 3:
                        RemoveDevice(devices);
                        break;
                }
            }
            while (action != 0);
        }

        // Добавление устройства: выводит типы из DeviceKinds, запрашивает общие
        // характеристики и вызывает метод создания выбранного типа.
        private static void AddDevice(List<AudioVideoDevice> devices)
        {
            Console.WriteLine("Тип объекта:");
            for (int i = 0; i < DeviceKinds.Length; i++)
            {
                Console.WriteLine($"  {i + 1} — {DeviceKinds[i].Title}");
            }

            int type = ConsoleInput.ReadInt("Ваш выбор: ", 1, DeviceKinds.Length);

            string brand = ConsoleInput.ReadNonEmptyString("Производитель: ");
            string model = ConsoleInput.ReadNonEmptyString("Модель: ");
            decimal price = ConsoleInput.ReadDecimal(
                "Цена (руб.): ", AudioVideoDevice.MinPrice, AudioVideoDevice.MaxPrice);

            AudioVideoDevice device = DeviceKinds[type - 1].Create(brand, model, price);

            devices.Add(device);
            Console.WriteLine($"Добавлено: {device}");
        }

        // Запрашивает диагональ и Smart TV и создаёт телевизор.
        private static AudioVideoDevice CreateTelevision(string brand, string model, decimal price)
        {
            int screenInch = ConsoleInput.ReadInt(
                "Диагональ (дюймы): ", Television.MinScreenInch, Television.MaxScreenInch);
            bool isSmartTv = ConsoleInput.ReadBool("Smart TV" + YesNoHint);

            return new Television(brand, model, price, screenInch, isSmartTv);
        }

        // Запрашивает диапазон частот и Bluetooth и создаёт радиоприёмник.
        private static AudioVideoDevice CreateRadioReceiver(string brand, string model, decimal price)
        {
            double frequencyMin = ConsoleInput.ReadDouble(
                "Мин. частота (МГц): ", RadioReceiver.MinFrequencyMHz, RadioReceiver.MaxFrequencyMHz);
            // Нижняя граница для максимума равна введённому минимуму, поэтому min > max ввести нельзя.
            double frequencyMax = ConsoleInput.ReadDouble(
                "Макс. частота (МГц): ", frequencyMin, RadioReceiver.MaxFrequencyMHz);
            bool hasBluetooth = ConsoleInput.ReadBool("Bluetooth" + YesNoHint);

            return new RadioReceiver(brand, model, price, frequencyMin, frequencyMax, hasBluetooth);
        }

        // Удаляет устройство по номеру в списке (номера начинаются с 1).
        private static void RemoveDevice(List<AudioVideoDevice> devices)
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Список пуст, удалять нечего.");
                return;
            }

            PrintDevices(devices);
            int number = ConsoleInput.ReadInt("Номер удаляемого устройства: ", 1, devices.Count);

            Console.WriteLine($"Удалено: {devices[number - 1]}");
            devices.RemoveAt(number - 1);
        }

        // Выводит список устройств с номерами. Для каждого элемента вызывается ToString(),
        // а в нём GetSpecifications() того класса, к которому объект относится фактически.
        private static void PrintDevices(List<AudioVideoDevice> devices)
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            Console.WriteLine($"--- Список устройств ({devices.Count}) ---");

            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {devices[i]}");
            }
        }
    }
}
