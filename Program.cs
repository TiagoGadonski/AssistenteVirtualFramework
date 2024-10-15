using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace AssistenteVirtualGratuito
{
    class Program
    {
        static SpeechRecognitionEngine recognizer;
        static SpeechSynthesizer synthesizer;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando a Assistente Virtual Gratuita...");

            // Verificar se a cultura "pt-BR" está instalada
            bool culturaDisponivel = false;
            foreach (var ci in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (ci.Culture.Name.Equals("pt-BR", StringComparison.InvariantCultureIgnoreCase))
                {
                    culturaDisponivel = true;
                    break;
                }
            }

            if (!culturaDisponivel)
            {
                Console.WriteLine("A cultura 'pt-BR' não está disponível no sistema. Por favor, instale os pacotes de idioma apropriados.");
                return;
            }

            // Inicializar Reconhecimento de Voz
            InicializarReconhecimentoVoz();

            // Inicializar Síntese de Voz
            synthesizer = new SpeechSynthesizer();
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
            synthesizer.SetOutputToDefaultAudioDevice();

            // Dizer algo para o usuário
            synthesizer.Speak("Olá! Eu sou sua assistente virtual. Como posso ajudar você hoje?");

            // Manter a aplicação rodando
            Console.WriteLine("Diga algo para a assistente. Pressione 'Esc' para sair.");
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    break;
                }

                await Task.Delay(100);
            }

            // Encerrar
            recognizer.Dispose();
            synthesizer.Dispose();
        }

        static void InicializarReconhecimentoVoz()
        {
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-BR"));

            // Carregar gramática básica
            var gramatica = new Choices();
            gramatica.Add(new string[] { "olá", "oi", "como vai", "qual é o seu nome", "tempo", "adeus", "tchau" });

            var gb = new GrammarBuilder();
            gb.Append(gramatica);

            var g = new Grammar(gb);
            recognizer.LoadGrammar(g);

            recognizer.SetInputToDefaultAudioDevice();
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string textoReconhecido = e.Result.Text;
            Console.WriteLine($"Você disse: {textoReconhecido}");

            // Processar intenção
            string resposta = ProcessarIntencao(textoReconhecido);

            // Responder ao usuário
            Console.WriteLine($"Assistente: {resposta}");
            synthesizer.SpeakAsync(resposta);
        }

        static string ProcessarIntencao(string texto)
        {
            switch (texto.ToLower())
            {
                case "olá":
                case "oi":
                case "como vai":
                case "qual é o seu nome":
                    return "Olá! Eu sou sua assistente virtual. Como posso ajudar você hoje?";
                case "tempo":
                    // Aqui você pode integrar com uma API gratuita de previsão do tempo
                    return "Atualmente, eu não tenho acesso à internet para verificar o tempo, mas espero que esteja tudo bem!";
                case "adeus":
                case "tchau":
                    synthesizer.Speak("Até logo! Tenha um ótimo dia.");
                    Environment.Exit(0);
                    return "Até logo!";
                default:
                    return "Desculpe, não entendi o que você quis dizer.";
            }
        }
    }
}

