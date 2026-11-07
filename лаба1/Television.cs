using System.Globalization;

namespace AudioVideoTech
{
    // Производный класс «Телевизор»: добавляет к базовому устройству диагональ экрана и признак Smart TV.
    public class Television : AudioVideoDevice
    {
        // Допустимые границы диагонали, дюймы.
        public const int MinScreenInch = 10;
        public const int MaxScreenInch = 120;

        // Диагональ, которую задаёт конструктор по умолчанию.
        public const int DefaultScreenInch = 32;

        // Число сантиметров в одном дюйме.
        public const double InchToCm = 2.54;

        private int _screenInch = DefaultScreenInch;

        // Конструктор по умолчанию: диагональ 32 дюйма, без Smart TV.
        public Television()
        {
        }

        // Конструктор с параметрами. Сначала через base(...) вызывается конструктор
        // базового класса, затем задаются собственные характеристики телевизора.
        public Television(string brand, string model, decimal price, int screenInch, bool isSmartTv)
            : base(brand, model, price)
        {
            ScreenInch = screenInch;
            IsSmartTv = isSmartTv;
        }

        // Диагональ, дюймы. Значение вне [MinScreenInch; MaxScreenInch]
        // запрещено (ArgumentOutOfRangeException).
        public int ScreenInch
        {
            get => _screenInch;
            set
            {
                if (value < MinScreenInch || value > MaxScreenInch)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(ScreenInch), value,
                        $"Диагональ должна быть в диапазоне [{MinScreenInch}; {MaxScreenInch}] дюймов.");
                }

                _screenInch = value;
            }
        }

        // Поддержка Smart TV. Автосвойство: проверять нечего,
        // скрытое поле создаёт компилятор.
        public bool IsSmartTv { get; set; }

        // Диагональ в сантиметрах. Свойство только для чтения:
        // значение не хранится, а вычисляется из ScreenInch при каждом обращении.
        public double ScreenCm => Math.Round(ScreenInch * InchToCm, 1);

        // Переопределение виртуального свойства: название типа.
        public override string DeviceType => "Телевизор";

        // Переопределение виртуального метода: диагональ и признак Smart TV.
        public override string GetSpecifications()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "диагональ {0} дюйм. ({1:N1} см), Smart TV: {2}",
                ScreenInch, ScreenCm, IsSmartTv ? "да" : "нет");
        }
    }
}
