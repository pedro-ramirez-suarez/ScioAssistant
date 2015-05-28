using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;
using ScioAssistantPhone.Resources;
using ScioAssistantPhone.Serializer;
using RestSharp;

namespace ScioAssistantPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        public async  void SpeechToText_Click(object sender, RoutedEventArgs e)
        {
            //Speech recognition only supports spanish from spain not from México
            var Language = (from language in InstalledSpeechRecognizers.All
                            where language.Language == "es-ES"
                            select language).FirstOrDefault();

            SpeechRecognizerUI speechRecognition = new SpeechRecognizerUI();

            speechRecognition.Recognizer.SetRecognizer(Language);

            SpeechRecognitionUIResult recoResult = await speechRecognition.RecognizeWithUIAsync();

            if (recoResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                //launch the query
                var client = new RestClient("http://scioassistant.cloudapp.net");
                client.AddHandler("application/json", new DynamicSerializer());

                client.ExecuteAsync<dynamic>(new RestRequest("home/searchforphone?query=" + recoResult.RecognitionResult.Text), (res) =>
                {
                    MessageBox.Show("result " + res.Content);
                });

                //MessageBox.Show(string.Format("You said {0}.", recoResult.RecognitionResult.Text));

                //display the results
            }
        }

       //code to speak
        /*
          //Part 1 - Basic Demo
            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync("You are reading Visual Studio Magazine!");  
         
            //Part 2 - Different Voices
            //var frenchVoice = InstalledVoices.All
            //                               .Where(voice => voice.Language.Equals("fr-FR") & voice.Gender == VoiceGender.Female)
            //                               .FirstOrDefault();
            //synth.SetVoice(frenchVoice);
            //await synth.SpeakTextAsync("Salut tout le monde!");

            //Part 3 - SSML
            //SpeechSynthesizer synth = new SpeechSynthesizer();
            //// Build an SSML prompt in a string.
            //string ssmlPrompt = "<speak version=\"1.0\" ";
            //ssmlPrompt += "xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">";
            //ssmlPrompt += "This voice speaks English. </speak>";
            //// Speak the SSML prompt.
            //await synth.SpeakSsmlAsync(ssmlPrompt);
         */

    }
}