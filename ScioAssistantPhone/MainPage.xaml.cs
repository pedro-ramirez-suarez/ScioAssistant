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
using Newtonsoft.Json;

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
                txtPregunta.Text = recoResult.RecognitionResult.Text.Replace(".","");
                LaunchSearch();
            }
        }

        public  void busca_Click(object sender, RoutedEventArgs e)
        {
            LaunchSearch();
        }


        public  void  LaunchSearch()
        {
            results.Children.Clear();
            //Hard coded...I know...it's temporary
            var client = new RestClient(AppResources.ScioAssistantRoot);
            client.AddHandler("application/json", new DynamicSerializer());
            //Hard coded...I know...it's temporary
            client.ExecuteAsync<dynamic>(new RestRequest(AppResources.ScioAssistantSearch + txtPregunta.Text), (res) =>
            {
                //MessageBox.Show("result " + res.Content);
                var data = JsonConvert.DeserializeObject<dynamic>(res.Content);
                SpeechSynthesizer synth = new SpeechSynthesizer();
                VoiceInformation spvoice;                
                bool found = true;
                //show the results
                foreach(var e in data)
                {
                    string result = e.ToString();
                    result = result.Replace("{", "").Replace("}", "").Replace("\r\n","").Replace("\"", "");
                    var lines = result.Split(new char[] { ','});
                    result = string.Empty;
                    foreach (var l in lines)
                    {
                        if (l.Contains("no puedo entenderte"))
                        {
                            found = false;                            
                            break;
                        }
                        if (!l.Trim().ToLower().StartsWith("id"))
                            result += l + "\r\n";                        
                    }
                    results.Children.Add(new TextBlock { Text = result, Padding= new Thickness(2,2,2,8) });
                }
                //set voice and message
                string msg = string.Empty;
                if (found)
                {
                    msg = "Esto es lo que encontré";
                    spvoice = InstalledVoices.All
                            .Where(voice => voice.Language.Equals("es-ES") & voice.Gender == VoiceGender.Female)
                            .FirstOrDefault();
                }
                else
                {
                    msg = "Lo siento, no puedo entenderte, intenta de nuevo.";
                    spvoice = InstalledVoices.All
                            .Where(voice => voice.Language.Equals("es-ES") & voice.Gender == VoiceGender.Male)
                            .FirstOrDefault();
                }
                
                synth.SetVoice(spvoice);
                synth.SpeakTextAsync(msg);
            });
        }

        private void txtPregunta_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                LaunchSearch();
        }

        private void txtPregunta_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


       

    }
}