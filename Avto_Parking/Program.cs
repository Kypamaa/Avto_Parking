using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Lab_Class_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = AppContext.BaseDirectory + @"\Base.txt";
            string pathHistory = AppContext.BaseDirectory + @"\History.txt";

            int command = 0;

            var parking = new CarPark
            {
                ParkedCars = new List<Car>
                {

                }
            };

            if (File.Exists(path) == false)
            {
                File.Create(AppContext.BaseDirectory + @"\Base.txt");
            }
            else
            {
                parking.LoadList();
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Park Controller Menu\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1. Добавить машину.");
            Console.WriteLine("2. Убрать машину досрочно.");
            Console.WriteLine("3. Информация о машинах на стоянке.");
            Console.WriteLine("4. История парковки.");
            Console.WriteLine("5. Выход из программы.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("\n\nВыбор: ");


            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                command = Convert.ToInt32(Console.ReadLine());
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Main(args);
                Console.WriteLine("Неккоректные данные!");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (command == 1)
            {
                Console.Clear();
                Console.WriteLine("Добавление машины на парковку:\n");

                Console.WriteLine("Производитель:");
                var producer = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Модель машины:");
                var model = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Цвет машины:");
                var color = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Номерной знак:");
                var stateNumber = Console.ReadLine();
                Console.Clear();

                bool once = false;

                parking.ParkedCars.ForEach(p =>
                {
                    if (stateNumber == p.PlateLicense)
                    {
                        once = true;
                    }
                });

                if (once == true)
                {
                    Console.WriteLine("Машина уже на парковке!");
                    once = false;
                }
                else
                {
                    parking.ParkCar(new Car
                    {
                        ArrivingTime = DateTime.Now,
                        DepartureTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                        Color = color,
                        Model = model,
                        Producer = producer,
                        PlateLicense = stateNumber
                    });
                    Console.WriteLine("Машина находится на парковке.");
                }
            }
            else if (command == 2)
            {
                Console.Clear();
                Console.WriteLine("Введите номер машины, которую нужно убрать:");

                var stateNumber = Console.ReadLine();
                Car car = null;
                parking.ParkedCars.ForEach(p =>
                {
                    if (stateNumber == p.PlateLicense)
                    {
                        car = p;
                    }
                });

                if (car != null)
                {
                    Console.Clear();
                    Console.WriteLine("Машина была убрана со стоянки.");
                    parking.CarRemove(car);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("На стоянке нет такой машины!");
                }
            }
            else if (command == 3)
            {
                Console.Clear();
                Console.WriteLine("Машины на парковке:");
                Console.WriteLine();
                parking.PrintParkedCars();
            }
            else if (command == 4)
            {
                Console.Clear();
                Console.WriteLine("История парковки:");
                Console.WriteLine();
                parking.PrintHistory();
            }
            else if (command == 5)
            {
                Environment.Exit(0);
            }
            else if (command >= 6)
            {
                Console.Clear();
                Console.WriteLine("Ошибка, попробуйте снова...");
                Console.WriteLine();

                Main(args);
            }

            parking.SaveList();
            Console.WriteLine();
            Console.WriteLine("Продолжить работу с программой?");

            string commandEnd = Console.ReadLine();

            if (commandEnd.ToString() != "exit")
            {
                Console.Clear();
                Main(args);
            }
        }
    }

    class CarPark
    {
        public List<Car> ParkedCars { get; set; }
        public List<string> FileList { get; set; }

        string path = AppContext.BaseDirectory + @"\Base.txt";
        string pathHistory = AppContext.BaseDirectory + @"\History.txt";

        public void PrintParkedCars()
        {
            ParkedCars.ForEach(p =>
            {
                Console.WriteLine($"{p.Producer}-{p.Model}-{p.Color}-{p.PlateLicense} прибыла на стоянку в: {p.ArrivingTime.ToShortDateString()}");
                Console.WriteLine($"Уедет в: {p.DepartureTime.ToShortDateString()}");
                Console.WriteLine();
            });
        }

        public void LoadList()
        {
            FileList = File.ReadAllLines(path).ToList();
            FileList.ForEach(p =>
            {
                string[] s = p.Split(';');

                DateTime parseMyFormat(string stringToParse)
                {
                    return DateTime.ParseExact(stringToParse, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }

                if (DateTime.Now < parseMyFormat(s[5]))
                {
                    ParkCarLoad(new Car
                    {
                        Producer = s[0],
                        Model = s[1],
                        Color = s[2],
                        PlateLicense = s[3],
                        ArrivingTime = parseMyFormat(s[4]),
                        DepartureTime = parseMyFormat(s[5]),
                    });
                }
                else
                {
                    string str = File.ReadAllText(pathHistory);

                    str += $"{s[0]} {s[1]} {s[2]} {s[3]} уехал со стоянки в {s[5]}" + Environment.NewLine;

                    File.WriteAllText(pathHistory, str);

                    str = null;
                }
            });
        }

        public void SaveList()
        {
            StringBuilder sb = new StringBuilder();
            ParkedCars.ForEach(parkedCar =>
            {
                sb.Append(parkedCar.ToString());
                sb.Append(Environment.NewLine);
            });
            File.WriteAllText(path, sb.ToString());
        }

        public void PrintHistory()
        {
            FileList = File.ReadAllLines(pathHistory).ToList();

            FileList.ForEach(Console.WriteLine);
            Console.WriteLine();
        }

        public void SaveHistory(Car car)
        {
            string str = File.ReadAllText(pathHistory);

            str += car.ToString() + Environment.NewLine;

            File.WriteAllText(pathHistory, str);

            str = null;
        }

        public void SaveHistoryCarRemove(Car car)
        {
            string str = File.ReadAllText(pathHistory);

            str += car.ToStringLeaveParking();
            str += Environment.NewLine;

            File.WriteAllText(pathHistory, str);

            str = null;
        }

        public void ParkCarLoad(Car car)
        {
            ParkedCars.Add(car);
        }

        public void ParkCar(Car car)
        {
            ParkedCars.Add(car);
            SaveHistory(car);
        }

        public void CarRemove(Car car)
        {
            ParkedCars.Remove(car);
            SaveHistoryCarRemove(car);
        }
    }

    class Car
    {
        public string Model { get; set; }
        public string Producer { get; set; }
        public string Color { get; set; }
        public string PlateLicense { get; set; }
        public DateTime ArrivingTime { get; set; }
        public DateTime DepartureTime { get; set; }

        public override string ToString()
        {

            return $"{Producer} {Model} {Color} {PlateLicense} {ArrivingTime.ToShortDateString()} {DepartureTime.ToShortDateString()}";
        }

        public string ToStringLeaveParking()
        {
            return $"{Producer} {Model} {Color} {PlateLicense} уехал со стоянки досрочно в {DateTime.Now}";
        }

    }
}