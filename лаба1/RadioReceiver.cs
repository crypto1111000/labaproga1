using System.Globalization;

namespace AudioVideoTech
{
    // Производный класс «Радиоприёмник»: добавляет к базовому устройству диапазон частот и признак наличия Bluetooth.
    public class RadioReceiver : AudioVideoDevice
    {
        // Физически допустимые частоты, МГц.
        public const double MinFrequencyMHz = 0.1;
        public const double MaxFrequencyMHz = 3000.0;

        // Диапазон FM, который задаёт конструктор по умолчанию, МГц.
        public const double DefaultFrequencyMinMHz = 87.5;
        public const double DefaultFrequencyMaxMHz = 108.0;

        private double _frequencyMinMHz = DefaultFrequencyMinMHz;
        private double _frequencyMaxMHz = DefaultFrequencyMaxMHz;

        // Конструктор по умолчанию: диапазон FM 87,5-108 МГц, без Bluetooth.
        public RadioReceiver()
        {
        }

        // Конструктор с параметрами. Обе границы задаются через SetFrequencyRange,
        // чтобы они проверялись вместе.
        public RadioReceiver(string brand, string model, decimal price,
                             double frequencyMinMHz, double frequencyMaxMHz, bool hasBluetooth)
            : base(brand, model, price)
        {
            SetFrequencyRange(frequencyMinMHz, frequencyMaxMHz);
            HasBluetooth = hasBluetooth;
        }

        // Нижняя граница диапазона, МГц. Проверяется на допустимость
        // и на то, что она не больше верхней границы.
        public double FrequencyMinMHz
        {
            get => _frequencyMinMHz;
            set
            {
                ValidateFrequency(value, nameof(FrequencyMinMHz));
                ValidateRange(value, _frequencyMaxMHz);
                _frequencyMinMHz = value;
            }
        }

        // Верхняя граница диапазона, МГц. Проверяется на допустимость
        // и на то, что она не меньше нижней границы.
        public double FrequencyMaxMHz
        {
            get => _frequencyMaxMHz;
            set
            {
                ValidateFrequency(value, nameof(FrequencyMaxMHz));
                ValidateRange(_frequencyMinMHz, value);
                _frequencyMaxMHz = value;
            }
        }

        // Наличие Bluetooth. Автосвойство.
        public bool HasBluetooth { get; set; }

        // Ширина диапазона, МГц. Свойство только для чтения, вычисляется из границ.
        public double BandwidthMHz => Math.Round(FrequencyMaxMHz - FrequencyMinMHz, 2);

        // Переопределение виртуального свойства: название типа.
        public override string DeviceType => "Радиоприёмник";

        // Задаёт обе границы диапазона сразу. Через свойства границы меняются по одной,
        // и каждая проверяется относительно второй: например, при диапазоне 87,5-108
        // нельзя первой поставить нижнюю границу 150, она больше текущей верхней.
        public void SetFrequencyRange(double minMHz, double maxMHz)
        {
            ValidateFrequency(minMHz, nameof(minMHz));
            ValidateFrequency(maxMHz, nameof(maxMHz));
            ValidateRange(minMHz, maxMHz);

            _frequencyMinMHz = minMHz;
            _frequencyMaxMHz = maxMHz;
        }

        // Переопределение виртуального метода: диапазон, его ширина и наличие Bluetooth.
        public override string GetSpecifications()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "диапазон {0:N1}-{1:N1} МГц (ширина {2:N1} МГц), Bluetooth: {3}",
                FrequencyMinMHz, FrequencyMaxMHz, BandwidthMHz, HasBluetooth ? "есть" : "нет");
        }

        /* Проверяет, что частота лежит в [MinFrequencyMHz; MaxFrequencyMHz] и не равна NaN.
           Иначе выбрасывает ArgumentOutOfRangeException. */
        private static void ValidateFrequency(double value, string paramName)
        {
            if (double.IsNaN(value) || value < MinFrequencyMHz || value > MaxFrequencyMHz)
            {
                throw new ArgumentOutOfRangeException(
                    paramName, value,
                    $"Частота должна быть в диапазоне [{MinFrequencyMHz}; {MaxFrequencyMHz}] МГц.");
            }
        }

        /* Проверяет, что нижняя граница не больше верхней.
           Иначе выбрасывает ArgumentException. */
        private static void ValidateRange(double minMHz, double maxMHz)
        {
            if (minMHz > maxMHz)
            {
                throw new ArgumentException("Нижняя граница диапазона не может превышать верхнюю.");
            }
        }
    }
}
