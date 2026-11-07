using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AudioVideoTech.Tests
{
    // Тесты базового класса AudioVideoDevice.
    public class AudioVideoDeviceTests
    {
        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var device = new AudioVideoDevice();

            Assert.Equal(AudioVideoDevice.DefaultBrand, device.Brand);
            Assert.Equal(AudioVideoDevice.DefaultModel, device.Model);
            Assert.Equal(0m, device.Price);
        }

        [Fact]
        public void ParameterizedConstructor_SetsAllProperties()
        {
            var device = new AudioVideoDevice("Generic", "AVD-100", 9999m);

            Assert.Equal("Generic", device.Brand);
            Assert.Equal("AVD-100", device.Model);
            Assert.Equal(9999m, device.Price);
        }

        [Fact]
        public void Brand_TrimsWhitespace()
        {
            var device = new AudioVideoDevice { Brand = "  Sony  " };

            Assert.Equal("Sony", device.Brand);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Brand_EmptyValue_ThrowsArgumentException(string? value)
        {
            var device = new AudioVideoDevice();

            Assert.Throws<ArgumentException>(() => { device.Brand = value!; });
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Model_EmptyValue_ThrowsArgumentException(string value)
        {
            var device = new AudioVideoDevice();

            Assert.Throws<ArgumentException>(() => { device.Model = value; });
        }

        // decimal нельзя передать в атрибут [InlineData], поэтому значения передаются
        // как double и приводятся к decimal внутри теста
        [Theory]
        [InlineData(-0.01)]
        [InlineData(10_000_000.01)]
        public void Price_OutOfRange_ThrowsArgumentOutOfRangeException(double value)
        {
            var device = new AudioVideoDevice();

            Assert.Throws<ArgumentOutOfRangeException>(() => { device.Price = (decimal)value; });
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1234.56)]
        [InlineData(10_000_000.0)]
        public void Price_BoundaryAndValidValues_AreAccepted(double value)
        {
            var device = new AudioVideoDevice { Price = (decimal)value };

            Assert.Equal((decimal)value, device.Price);
        }

        [Fact]
        public void DeviceType_ForBaseClass_IsGeneric()
        {
            Assert.Equal("Аудио/видео устройство", new AudioVideoDevice().DeviceType);
        }

        [Fact]
        public void GetSpecifications_ForBaseClass_IsEmpty()
        {
            var device = new AudioVideoDevice("Generic", "AVD-100", 9999m);

            Assert.Empty(device.GetSpecifications());
        }

        [Fact]
        public void ToString_ForBaseClass_ContainsOnlyCommonData()
        {
            string text = new AudioVideoDevice("Generic", "AVD-100", 9999m).ToString();

            Assert.StartsWith("Аудио/видео устройство: Generic AVD-100", text);
            Assert.EndsWith("руб.", text); // без хвоста из специфичных характеристик
        }
    }

    // Тесты производного класса Television.
    public class TelevisionTests
    {
        [Fact]
        public void Constructor_SetsInheritedAndOwnProperties()
        {
            var tv = new Television("Samsung", "UE55AU7100", 49990m, 55, true);

            Assert.Equal("Samsung", tv.Brand);
            Assert.Equal("UE55AU7100", tv.Model);
            Assert.Equal(49990m, tv.Price);
            Assert.Equal(55, tv.ScreenInch);
            Assert.True(tv.IsSmartTv);
        }

        [Fact]
        public void DefaultConstructor_UsesDefaultValues()
        {
            var tv = new Television();

            Assert.Equal(Television.DefaultScreenInch, tv.ScreenInch);
            Assert.False(tv.IsSmartTv);
        }

        [Theory]
        [InlineData(Television.MinScreenInch - 1)]
        [InlineData(Television.MaxScreenInch + 1)]
        [InlineData(-10)]
        public void ScreenInch_OutOfRange_ThrowsArgumentOutOfRangeException(int value)
        {
            var tv = new Television();

            Assert.Throws<ArgumentOutOfRangeException>(() => { tv.ScreenInch = value; });
        }

        [Theory]
        [InlineData(Television.MinScreenInch)]
        [InlineData(Television.MaxScreenInch)]
        public void ScreenInch_BoundaryValues_AreAccepted(int value)
        {
            var tv = new Television { ScreenInch = value };

            Assert.Equal(value, tv.ScreenInch);
        }

        [Fact]
        public void ScreenCm_IsConvertedFromInches()
        {
            var tv = new Television("LG", "32LQ570B", 17500m, 32, false);

            // 32 * 2,54 = 81,28 -> округление до одного знака = 81,3
            Assert.Equal(81.3, tv.ScreenCm, 1);
        }

        [Fact]
        public void DeviceType_IsTelevision()
        {
            Assert.Equal("Телевизор", new Television().DeviceType);
        }

        [Fact]
        public void GetSpecifications_ContainsScreenAndSmartTv()
        {
            var tv = new Television("Samsung", "UE55AU7100", 49990m, 55, true);
            string specifications = tv.GetSpecifications();

            // Проверяем именно фразу «диагональ 55», а не просто «55»:
            // число 55 есть ещё и в названии модели UE55AU7100
            Assert.Contains("диагональ 55 дюйм.", specifications);
            Assert.Contains("Smart TV: да", specifications);
        }

        [Fact]
        public void GetSpecifications_WithoutSmartTv_ShowsNo()
        {
            var tv = new Television("LG", "32LQ570B", 17500m, 32, false);

            Assert.Contains("Smart TV: нет", tv.GetSpecifications());
        }

        [Fact]
        public void ToString_ContainsCommonAndOwnData()
        {
            string text = new Television("Samsung", "UE55AU7100", 49990m, 55, true).ToString();

            Assert.StartsWith("Телевизор: Samsung UE55AU7100", text);
            Assert.Contains("диагональ 55 дюйм.", text);
        }

        [Fact]
        public void Television_IsAudioVideoDevice()
        {
            Assert.IsAssignableFrom<AudioVideoDevice>(new Television());
        }
    }

    // Тесты производного класса RadioReceiver.
    public class RadioReceiverTests
    {
        [Fact]
        public void Constructor_SetsInheritedAndOwnProperties()
        {
            var radio = new RadioReceiver("Sony", "ICF-P36", 3200m, 87.5, 108.0, false);

            Assert.Equal("Sony", radio.Brand);
            Assert.Equal("ICF-P36", radio.Model);
            Assert.Equal(3200m, radio.Price);
            Assert.Equal(87.5, radio.FrequencyMinMHz);
            Assert.Equal(108.0, radio.FrequencyMaxMHz);
            Assert.False(radio.HasBluetooth);
        }

        [Fact]
        public void Constructor_MinGreaterThanMax_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(
                () => new RadioReceiver("Sony", "ICF-P36", 3200m, 108.0, 87.5, false));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-5.0)]
        [InlineData(5000.0)]
        public void FrequencyMin_OutOfRange_ThrowsArgumentOutOfRangeException(double value)
        {
            var radio = new RadioReceiver();

            Assert.Throws<ArgumentOutOfRangeException>(() => { radio.FrequencyMinMHz = value; });
        }

        [Fact]
        public void FrequencyMin_AboveCurrentMax_ThrowsArgumentException()
        {
            var radio = new RadioReceiver(); // диапазон 87,5-108

            Assert.Throws<ArgumentException>(() => { radio.FrequencyMinMHz = 120.0; });
        }

        [Fact]
        public void FrequencyMax_BelowCurrentMin_ThrowsArgumentException()
        {
            var radio = new RadioReceiver(); // диапазон 87,5-108

            Assert.Throws<ArgumentException>(() => { radio.FrequencyMaxMHz = 50.0; });
        }

        [Fact]
        public void SetFrequencyRange_ChangesBothBoundsAtOnce()
        {
            var radio = new RadioReceiver();

            // Поставить min = 150 через свойство нельзя (150 > 108),
            // а через SetFrequencyRange обе границы меняются вместе
            radio.SetFrequencyRange(150.0, 200.0);

            Assert.Equal(150.0, radio.FrequencyMinMHz);
            Assert.Equal(200.0, radio.FrequencyMaxMHz);
            Assert.Equal(50.0, radio.BandwidthMHz, 2);
        }

        [Fact]
        public void DeviceType_IsRadioReceiver()
        {
            Assert.Equal("Радиоприёмник", new RadioReceiver().DeviceType);
        }

        [Fact]
        public void GetSpecifications_ContainsRangeAndBluetooth()
        {
            var radio = new RadioReceiver("Philips", "TAR3205", 4500m, 87.5, 108.0, true);
            string specifications = radio.GetSpecifications();

            Assert.StartsWith("диапазон", specifications);
            Assert.Contains("Bluetooth: есть", specifications);
        }

        [Fact]
        public void ToString_ContainsCommonAndOwnData()
        {
            string text = new RadioReceiver("Philips", "TAR3205", 4500m, 87.5, 108.0, true).ToString();

            Assert.StartsWith("Радиоприёмник: Philips TAR3205", text);
            Assert.Contains("Bluetooth: есть", text);
        }

        [Fact]
        public void RadioReceiver_IsAudioVideoDevice()
        {
            Assert.IsAssignableFrom<AudioVideoDevice>(new RadioReceiver());
        }
    }

    // Тесты полиморфизма: виртуальные члены вызываются через ссылку базового типа,
    // а выполняется версия фактического класса объекта.
    public class PolymorphismTests
    {
        private static List<AudioVideoDevice> CreateCollection()
        {
            return new List<AudioVideoDevice>
            {
                new Television("Samsung", "UE55AU7100", 49990m, 55, true),
                new RadioReceiver("Sony", "ICF-P36", 3200m, 87.5, 108.0, false),
                new AudioVideoDevice("Generic", "AVD-100", 9999m)
            };
        }

        [Fact]
        public void BaseReference_CallsOverriddenGetSpecifications()
        {
            // Переменная объявлена базовым типом, объект — Television
            AudioVideoDevice device = new Television("Samsung", "UE55AU7100", 49990m, 55, true);

            Assert.Contains("диагональ 55 дюйм.", device.GetSpecifications());
        }

        [Fact]
        public void BaseReference_CallsOverriddenDeviceType()
        {
            AudioVideoDevice device = new RadioReceiver();

            Assert.Equal("Радиоприёмник", device.DeviceType);
        }

        [Fact]
        public void Collection_EachElementUsesItsOwnGetSpecifications()
        {
            List<string> specifications = CreateCollection().Select(d => d.GetSpecifications()).ToList();

            Assert.StartsWith("диагональ", specifications[0]);
            Assert.StartsWith("диапазон", specifications[1]);
            Assert.Empty(specifications[2]);
        }

        [Fact]
        public void Collection_ToStringOfEachElementStartsWithItsType()
        {
            List<string> texts = CreateCollection().Select(d => d.ToString()).ToList();

            Assert.StartsWith("Телевизор:", texts[0]);
            Assert.StartsWith("Радиоприёмник:", texts[1]);
            Assert.StartsWith("Аудио/видео устройство:", texts[2]);
        }

        [Fact]
        public void Collection_TypeFilteringWorks()
        {
            List<AudioVideoDevice> devices = CreateCollection();

            Assert.Single(devices.OfType<Television>());
            Assert.Single(devices.OfType<RadioReceiver>());
            Assert.Equal(3, devices.Count);
        }

        [Fact]
        public void InheritedProperty_IsAccessibleThroughBaseReference()
        {
            AudioVideoDevice device = new Television("LG", "32LQ570B", 17500m, 32, false);

            device.Price = 15000m;

            // Изменение через базовую ссылку видно и через ссылку производного типа — это один объект
            Assert.Equal(15000m, ((Television)device).Price);
        }

        [Fact]
        public void TotalPrice_IsCalculatedThroughBaseReferences()
        {
            decimal total = CreateCollection().Sum(d => d.Price);

            Assert.Equal(49990m + 3200m + 9999m, total);
        }
    }

    // Тесты класса ConsoleInput. Ввод с клавиатуры подменяется строкой
    // через Console.SetIn, вывод перенаправляется в буфер.
    public class ConsoleInputTests : IDisposable
    {
        private readonly TextReader _originalIn = Console.In;
        private readonly TextWriter _originalOut = Console.Out;

        // xUnit вызывает Dispose после каждого теста: возвращаем настоящие ввод и вывод.
        public void Dispose()
        {
            Console.SetIn(_originalIn);
            Console.SetOut(_originalOut);
        }

        // Подменяет ввод с клавиатуры заданным текстом, а вывод - буфером,
        // чтобы приглашения не мешали выводу тестов.
        private static void SetInput(string text)
        {
            Console.SetIn(new StringReader(text));
            Console.SetOut(new StringWriter());
        }

        [Fact]
        public void ReadInt_SkipsInvalidInput_ReturnsFirstValidValue()
        {
            SetInput("abc\n500\n43\n");

            Assert.Equal(43, ConsoleInput.ReadInt("", Television.MinScreenInch, Television.MaxScreenInch));
        }

        [Fact]
        public void ReadDouble_AcceptsCommaAsDecimalSeparator()
        {
            SetInput("87,5\n");

            Assert.Equal(87.5, ConsoleInput.ReadDouble("", RadioReceiver.MinFrequencyMHz, RadioReceiver.MaxFrequencyMHz));
        }

        [Fact]
        public void ReadDecimal_SkipsOutOfRangeValue()
        {
            SetInput("-100\n29990\n");

            Assert.Equal(29990m, ConsoleInput.ReadDecimal("", AudioVideoDevice.MinPrice, AudioVideoDevice.MaxPrice));
        }

        [Fact]
        public void ReadNonEmptyString_SkipsEmptyLineAndTrims()
        {
            SetInput("   \n  LG  \n");

            Assert.Equal("LG", ConsoleInput.ReadNonEmptyString(""));
        }

        [Fact]
        public void ReadBool_AcceptsOnlyOneOrZero()
        {
            SetInput("да\n1\n");

            Assert.True(ConsoleInput.ReadBool(""));
        }

        [Fact]
        public void ClosedInput_ThrowsEndOfStreamException()
        {
            SetInput("");

            Assert.Throws<EndOfStreamException>(() => ConsoleInput.ReadInt("", 0, 3));
        }
    }
}
