using System.Globalization;

namespace AudioVideoTech
{
    // Базовый класс иерархии: хранит общие характеристики устройства (производитель, модель, цена).
    public class AudioVideoDevice
    {
        // Допустимые границы цены, руб.
        public const decimal MinPrice = 0m;
        public const decimal MaxPrice = 10_000_000m;

        // Значения, которые задаёт конструктор по умолчанию.
        public const string DefaultBrand = "Неизвестный производитель";
        public const string DefaultModel = "Без модели";

        // Закрытые поля; снаружи доступны только через свойства.
        private string _brand = DefaultBrand;
        private string _model = DefaultModel;
        private decimal _price;

        // Конструктор по умолчанию: производитель и модель по умолчанию, цена 0.
        public AudioVideoDevice()
        {
        }

        // Конструктор с параметрами. Значения присваиваются через свойства,
        // поэтому при создании объекта выполняются те же проверки, что и в set.
        public AudioVideoDevice(string brand, string model, decimal price)
        {
            Brand = brand;
            Model = model;
            Price = price;
        }

        // Производитель. Пустая строка запрещена (ArgumentException),
        // пробелы в начале и в конце отбрасываются.
        public string Brand
        {
            get => _brand;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Производитель не может быть пустым.", nameof(Brand));
                }

                _brand = value.Trim();
            }
        }

        // Модель. Проверка такая же, как у Brand.
        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Модель не может быть пустой.", nameof(Model));
                }

                _model = value.Trim();
            }
        }

        /* Цена, руб. Значение вне [MinPrice; MaxPrice] запрещено (ArgumentOutOfRangeException).
           Тип decimal хранит деньги без ошибок округления, которые есть у double. */
        public decimal Price
        {
            get => _price;
            set
            {
                if (value < MinPrice || value > MaxPrice)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Price), value, $"Цена должна быть в диапазоне [{MinPrice}; {MaxPrice}] руб.");
                }

                _price = value;
            }
        }

        /* Виртуальное свойство: название типа устройства.
           Наследники переопределяют его и возвращают своё название. */
        public virtual string DeviceType => "Аудио/видео устройство";

        /* Виртуальный метод: характеристики, присущие конкретному типу устройства.
           У базового устройства своих характеристик нет, поэтому возвращается пустая строка.
           Наследники переопределяют метод (override) и возвращают свои данные. */
        public virtual string GetSpecifications()
        {
            return string.Empty;
        }

        /* Полное описание устройства. Общая часть (тип, производитель, модель, цена)
           формируется здесь, а характеристики берутся из GetSpecifications().
           Какая версия GetSpecifications() вызовется, определяется фактическим типом объекта. */
        public override string ToString()
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                "{0}: {1} {2}, цена {3:N2} руб.",
                DeviceType, Brand, Model, Price);

            string specifications = GetSpecifications();

            return specifications.Length == 0 ? text : text + ", " + specifications;
        }
    }
}
