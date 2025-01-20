using System;
using System.Collections.Generic;
using System.IO;

namespace CarWave
{
    class Auto
    {
        public string Marka { get; }
        public string Model { get; }
        public int RokProdukcji { get; }
        public float CenaWynajmu { get; }
        public float Kaucja { get; }
        public string NumerRejestracyjny { get; }
        public bool Wynajete { get; private set; }

        public Auto(string marka, string model, int rok, float cena, float kaucja, string rejestracja)
        {
            Marka = marka;
            Model = model;
            RokProdukcji = rok;
            CenaWynajmu = cena;
            Kaucja = kaucja;
            NumerRejestracyjny = rejestracja;
        }

        public void WynajmijAuto()
        {
            if (Wynajete)
            {
                Console.WriteLine("Nie można wynająć auta, ponieważ jest już wynajęte.");
            }
            else
            {
                Wynajete = true;
            }
        }

        public void ZwrocAuto()
        {
            if (!Wynajete)
            {
                Console.WriteLine("Nie można zwrócić auta, ponieważ nie było wynajęte.");
            }
            else
            {
                Wynajete = false;
            }
        }

        public void WyswietlInformacje() => Console.WriteLine($"{Marka} {Model} ({RokProdukcji}), Rejestracja: {NumerRejestracyjny}, Cena: {CenaWynajmu:C}, Kaucja: {Kaucja:C}, Wynajęte: {Wynajete}");

        public string ZapiszDoPliku() => $"{Marka},{Model},{RokProdukcji},{CenaWynajmu},{Kaucja},{NumerRejestracyjny},{Wynajete}";

        public static Auto OdczytajZPliku(string linia)
        {
            var dane = linia.Split(',');
            if (dane.Length == 7 &&
                int.TryParse(dane[2], out int rok) &&
                float.TryParse(dane[3], out float cena) &&
                float.TryParse(dane[4], out float kaucja) &&
                bool.TryParse(dane[6], out bool wynajete))
            {
                return new Auto(dane[0], dane[1], rok, cena, kaucja, dane[5]) { Wynajete = wynajete };
            }
            Console.WriteLine($"Niepoprawny format danych: {linia}");
            return null;
        }
    }

    class ZarzadzanieFlota
    {
        private List<Auto> auta = new();

        public void DodajAuto(Auto auto)
        {
            if (auto != null)
            {
                auta.Add(auto);
            }
        }

        public void ListaWszystkichAut()
        {
            foreach (var auto in auta)
            {
                auto.WyswietlInformacje();
            }
        }

        public IEnumerable<Auto> PobierzAuta(bool wynajete)
        {
            List<Auto> wynik = new List<Auto>();
            foreach (var auto in auta)
            {
                if (auto.Wynajete == wynajete)
                {
                    wynik.Add(auto);
                }
            }
            return wynik;
        }

        public void OznaczAutoJakoWynajete(string rejestracja)
        {
            var auto = auta.Find(a => a.NumerRejestracyjny == rejestracja);
            if (auto != null)
            {
                auto.WynajmijAuto();
                if (auto.Wynajete)
                {
                    Console.WriteLine($"Auto {rejestracja} zostało oznaczone jako wynajęte.");
                }
            }
            else
            {
                Console.WriteLine($"Nie znaleziono auta z numerem rejestracyjnym {rejestracja}.");
            }
        }

        public void CofnijAutoJakoWynajete(string rejestracja)
        {
            var auto = auta.Find(a => a.NumerRejestracyjny == rejestracja);
            if (auto != null)
            {
                auto.ZwrocAuto();
                if (!auto.Wynajete)
                {
                    Console.WriteLine($"Auto {rejestracja} zostało oznaczone jako dostępne.");
                }
            }
            else
            {
                Console.WriteLine($"Nie znaleziono auta z numerem rejestracyjnym {rejestracja}.");
            }
        }

        public void ZapiszFloteDoPliku(string sciezka)
        {
            File.WriteAllLines(sciezka, auta.ConvertAll(auto => auto.ZapiszDoPliku()));
        }

        public void OdczytajFloteZPliku(string sciezka)
        {
            if (File.Exists(sciezka))
            {
                foreach (var linia in File.ReadLines(sciezka))
                {
                    var auto = Auto.OdczytajZPliku(linia);
                    if (auto != null)
                    {
                        DodajAuto(auto);
                    }
                }
            }
            else
            {
                Console.WriteLine("Plik nie istnieje.");
            }
        }
    }

    class Program
    {
        static string GenerujWarszawskaRejestracje()
        {
            var random = new Random();
            string litery = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return $"WI {litery[random.Next(26)]}{litery[random.Next(26)]}{random.Next(1000, 9999)}";
        }

        static void Main()
        {
            var flota = new ZarzadzanieFlota();
            flota.OdczytajFloteZPliku("flota.txt");

            flota.DodajAuto(new Auto("BMW", "M3 Competition", 2022, 900, 5000, GenerujWarszawskaRejestracje()));
            flota.DodajAuto(new Auto("Mercedes", "AMG GT 63S", 2021, 1100, 7000, GenerujWarszawskaRejestracje()));

            Console.WriteLine("*** CARWAVE ***\nLogin:");
            if (Console.ReadLine() == "Grzegorz" && Console.ReadLine() == "35744")
            {
                Console.WriteLine("Witaj, Grzegorz!");
                bool dziala = true;
                while (dziala)
                {
                    Console.WriteLine("\n1. Lista Wszystkich Aut\n2. Lista Dostępnych\n3. Lista Wynajętych\n4. Zapisz Flotę\n5. Oznacz Auto Jako Wynajęte\n6. Cofnij Auto Jako Wynajęte\n7. Wyjdź");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            flota.ListaWszystkichAut();
                            break;
                        case "2":
                            Console.WriteLine("\n--- Dostępne ---");
                            foreach (var auto in flota.PobierzAuta(false)) auto.WyswietlInformacje();
                            break;
                        case "3":
                            Console.WriteLine("\n--- Wynajęte ---");
                            foreach (var auto in flota.PobierzAuta(true)) auto.WyswietlInformacje();
                            break;
                        case "4":
                            flota.ZapiszFloteDoPliku("flota.txt");
                            Console.WriteLine("Zapisano flotę.");
                            break;
                        case "5":
                            Console.WriteLine("Podaj numer rejestracyjny auta do oznaczenia jako wynajęte:");
                            flota.OznaczAutoJakoWynajete(Console.ReadLine());
                            break;
                        case "6":
                            Console.WriteLine("Podaj numer rejestracyjny auta do cofnięcia statusu wynajętego:");
                            flota.CofnijAutoJakoWynajete(Console.ReadLine());
                            break;
                        case "7":
                            dziala = false;
                            break;
                        default:
                            Console.WriteLine("Nieprawidłowy wybór.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy login lub hasło.");
            }
        }
    }
}
