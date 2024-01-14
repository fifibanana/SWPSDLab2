using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
using System.Data;

namespace Lab2
{

    public partial class MainWindow : Window
    {

        private StringBuilder input = new StringBuilder(); // Przechowuje wprowadzone przez użytkownika liczby i operatory
        private bool isNewInput = true; // Określa, czy aktualny ciąg znaków jest nowym wprowadzeniem

        static bool speechOn = true;
        static SpeechSynthesizer pTTS = new SpeechSynthesizer();
        private SpeechRecognitionEngine pSRE;
        public MainWindow()
        {
            InitializeComponent();
            pTTS.SetOutputToDefaultAudioDevice();
            pTTS.Speak("Witam w kalkulatorze");
            pSRE = new SpeechRecognitionEngine();


            // Ustawienie domyślnego urządzenia wejściowego:
            pSRE.SetInputToDefaultAudioDevice();



            pSRE.SpeechRecognized += PSRE_SpeechRecognized;
            // -------------------------------------------------------------------------
            Choices stopChoice = new Choices();
            stopChoice.Add("Stop");
            stopChoice.Add("Pomoc");

            // Budowa gramatyki numer 1 - definiowanie składni gramatyki:
            GrammarBuilder buildGrammarSystem = new GrammarBuilder();
            buildGrammarSystem.Append(stopChoice);

            // Budowa gramatyki numer 1 - utworzenie gramatyki:
            Grammar grammarSystem = new Grammar(buildGrammarSystem); // 
                                                                     // -------------------------------------------------------------------------
                                                                     // Budowa gramatyki numer 2 - POLECENIA DLA PROGRAMU
                                                                     // Budowa gramatyki numer 2 - określenie komend:
            Choices chNumbers = new Choices(); //możliwy wybór słów
            string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            chNumbers.Add(numbers);
            // Budowa gramatyki numer 2 - definiowanie składni gramatyki:
            GrammarBuilder grammarProgram = new GrammarBuilder();
            grammarProgram.Append("Oblicz");
            grammarProgram.Append(chNumbers);
            grammarProgram.Append("plus");
            grammarProgram.Append(chNumbers);



            // Budowa gramatyki numer 2 - utworzenie gramatyki:
            Grammar g_WhatIsXplusY = new Grammar(grammarProgram); //gramatyka
                                                                  // -------------------------------------------------------------------------
                                                                  // Załadowanie gramatyk:
            pSRE.LoadGrammarAsync(g_WhatIsXplusY);



            GrammarBuilder grammarProgram2 = new GrammarBuilder();
            grammarProgram2.Append("Oblicz");
            grammarProgram2.Append(chNumbers);
            grammarProgram2.Append("minus");
            grammarProgram2.Append(chNumbers);
            Grammar g_WhatIsXminusY = new Grammar(grammarProgram2);
            pSRE.LoadGrammarAsync(g_WhatIsXminusY);

            GrammarBuilder grammarProgram3 = new GrammarBuilder();
            grammarProgram3.Append("Oblicz");
            grammarProgram3.Append(chNumbers);
            grammarProgram3.Append("razy");
            grammarProgram3.Append(chNumbers);
            Grammar g_WhatIsXrazyY = new Grammar(grammarProgram3);
            pSRE.LoadGrammarAsync(g_WhatIsXrazyY);



            GrammarBuilder grammarProgram4 = new GrammarBuilder();
            grammarProgram4.Append("Oblicz");
            grammarProgram4.Append(chNumbers);
            grammarProgram4.Append("podziel");
            grammarProgram4.Append(chNumbers);
            Grammar g_WhatIsXpodzielY = new Grammar(grammarProgram4);
            pSRE.LoadGrammarAsync(g_WhatIsXpodzielY);
            pSRE.LoadGrammarAsync(grammarSystem);
            // Ustaw rozpoznawanie przy wykorzystaniu wielu gramatyk:
            pSRE.RecognizeAsync(RecognizeMode.Multiple);
            // -------------------------------------------------------------------------
            Console.WriteLine("\nAby zakonczyć działanie programu powiedz 'STOP'\n");



        }
       

        static void PSRE_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            string comments;
            float confidence = e.Result.Confidence;
            comments = String.Format("ROZPOZNANO (wiarygodność: {0:0.000}): '{1}'",
           e.Result.Confidence, txt);
            Console.WriteLine(comments);
            if (confidence > 0.30)
            {
                if (txt.IndexOf("Stop") >= 0)
                {
                    speechOn = false;
                }
                else if (txt.IndexOf("Pomoc") >= 0)
                {
                    pTTS.SpeakAsync("Składnia polecenia dodawanie: Oblicz liczba plus liczba. Na przykład: dwa plus trzy");
                    pTTS.SpeakAsync("Składnia polecenia odejmowanie: Oblicz liczba minus liczba. Na przykład: cztery minus trzy");
                    pTTS.SpeakAsync("Składnia polecenia mnożenie: Oblicz liczba razy liczba. Na przykład: cztery razy trzy");
                    pTTS.SpeakAsync("Składnia polecenia dzielenie: Oblicz liczba podziel liczba. Na przykład: cztery podziel trzy");
                }
                else if ((txt.IndexOf("Oblicz") >= 0) && (txt.IndexOf("plus") >= 0) && (speechOn == true))
                {
                    string[] words = txt.Split(' ');
                    int liczba1 = int.Parse(words[1]);
                    int liczba2 = int.Parse(words[3]);
                    int suma = liczba1 + liczba2;
                
                    comments = String.Format("\tOBLICZONO: {0} + {1} = {2}",
                    liczba1, liczba2, suma);

                    string equation = String.Format("{0} + {1} = {2}", liczba1, liczba2, suma);
                    UpdateEquation(equation);
                    Console.WriteLine(comments);
                    pTTS.SpeakAsync("Wynik działania to: " + suma);
                }
                else if ((txt.IndexOf("Oblicz") >= 0) && (txt.IndexOf("minus") >= 0) && (speechOn == true))
                {
                    string[] words = txt.Split(' ');
                    int liczba1 = int.Parse(words[1]);
                    int liczba2 = int.Parse(words[3]);
                    int suma = liczba1 - liczba2;
                    comments = String.Format("\tOBLICZONO: {0} - {1} = {2}",
                    liczba1, liczba2, suma);
                    string equation = String.Format("{0} - {1} = {2}", liczba1, liczba2, suma);
                    UpdateEquation(equation);
                    Console.WriteLine(comments);
                    pTTS.SpeakAsync("Wynik działania to: " + suma);
                }
                else if ((txt.IndexOf("Oblicz") >= 0) && (txt.IndexOf("razy") >= 0) && (speechOn == true))
                {
                    string[] words = txt.Split(' ');
                    int liczba1 = int.Parse(words[1]);
                    int liczba2 = int.Parse(words[3]);
                    int suma = liczba1 * liczba2;
                    comments = String.Format("\tOBLICZONO: {0} * {1} = {2}",
                    liczba1, liczba2, suma);
                    string equation = String.Format("{0} * {1} = {2}", liczba1, liczba2, suma);
                    UpdateEquation(equation);
                    Console.WriteLine(comments);
                    pTTS.SpeakAsync("Wynik działania to: " + suma);
                }
                else if ((txt.IndexOf("Oblicz") >= 0) && (txt.IndexOf("podziel") >= 0) && (speechOn == true))
                {
                    string[] words = txt.Split(' ');
                    int liczba1 = int.Parse(words[1]);
                    int liczba2 = int.Parse(words[3]);
                    int suma = liczba1 / liczba2;
                    comments = String.Format("\tOBLICZONO: {0} / {1} = {2}",
                    liczba1, liczba2, suma);
                    string equation = String.Format("{0} / {1} = {2}", liczba1, liczba2, suma);
                    UpdateEquation(equation);
                    Console.WriteLine(comments);
                    pTTS.SpeakAsync("Wynik działania to: " + suma);
                }
            }

            else
            {
                comments = String.Format("\tNISKI WSPÓŁCZYNNIK WIARYGODNOŚCI  - powtórz polecenie");
                Console.WriteLine(comments);
                pTTS.SpeakAsync("Proszę powtórzyć");
            }
        }


        private static void UpdateEquation(string text) {
            // Pobranie referencji do TextBox z wątku UI za pomocą Dispatcher.Invoke
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Application.Current.MainWindow != null && Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Wykonanie operacji na TextBox (ResultTextBox) z wątku UI
                    mainWindow.ResultTextBox.Clear();
                    mainWindow.ResultTextBox.AppendText(text);
                }
            });
        }

        private void HandleInput(string value)
        {
            if (isNewInput)
            {
                ResultTextBox.Text = ""; // Czyści pole tekstowe, gdy zaczynamy nowe wprowadzenie
                isNewInput = false;
            }

            input.Append(value); // Dodaje wartość (liczbę lub operator) do aktualnego wprowadzenia
            ResultTextBox.Text += value; // Aktualizuje pole tekstowe o dodaną wartość
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("1");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("2");
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("3");
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("4");
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("5");
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("6");

        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("7");
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("8");
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("9");
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("0");
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("+");
        }
        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("-");
        }
        private void buttonTimes_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("*");
        }

        private void buttonDivide_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("/");
        }

        private void buttonCount_Click(object sender, RoutedEventArgs e)
        {
            string expression = ResultTextBox.Text;
            DataTable dt = new DataTable();

            try
            {
                var result = dt.Compute(expression, ""); // Obliczanie wyrażenia przy użyciu DataTable

                ResultTextBox.Text = result.ToString(); // Wyświetlenie wyniku w polu tekstowym
                isNewInput = true; // Ustawienie flagi na nowe wprowadzenie
                input.Clear(); // Wyczyszczenie bufora wprowadzania
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = "Błąd"; // Informacja o błędzie w przypadku niepowodzenia obliczeń
            }
        }
    }
}