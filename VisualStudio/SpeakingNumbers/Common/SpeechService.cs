using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace NumberSpeaker.Common
{
    public class SpeechService
    {
        private string _targetLanguageCode;
        private SpeechSynthesizer _speech;
        private string _filterLanguage;
        private Dictionary<string, string> _langFilterLang = new Dictionary<string, string>() {
            {"fr","fr-FR"},
            {"frs","fr-CH"},
            {"es","es-MX"},
            {"de","de-DE"}
        };
        public SpeechService(string targetLanguageCode)
        {
            _targetLanguageCode = targetLanguageCode;
            _speech = new SpeechSynthesizer();
            _filterLanguage = _langFilterLang[targetLanguageCode];
            _filterLanguage =_filterLanguage == null ? "fr-CH" : _filterLanguage;
        }

        /// <summary>
        /// ties up the UI, too bad for now...
        /// </summary>
        public bool IsLanguageFound() {
            var voices = SpeechSynthesizer.AllVoices.Where(x => x.Language == _filterLanguage);
            return voices.Any();
        }

        /// <summary>
        /// Speak the passed text using the previously setup language.
        /// In fact right now, this takes a range, and returns a random number between that range.
        /// Maybe best to put correct answers in a list/dictionary, and check if they have been correct before,
        /// in which case we don't hear them again during this session
        /// </summary>
        /// <param name="translatedText"></param>
        //public async void SpeakTheTranslatedText(string translatedText)
        public async void SpeakTheTranslatedText(string numberToSpeak)
        {

            IEnumerable<VoiceInformation> voices = SpeechSynthesizer.AllVoices.Where(x => x.Language == _filterLanguage);

            // Set the voice as identified by the query.
            _speech.Voice = voices.ElementAt(0);

            var stream = await _speech.SynthesizeTextToStreamAsync(numberToSpeak);
            var media = new MediaElement();
            //MessageDialog msgBox = new MessageDialog("Point 1");
            //await msgBox.ShowAsync();
            media.SetSource(stream, stream.ContentType);
            media.Play();
        }

        public static IEnumerable<VoiceInformation> GetAllVoices()
        {
            return (SpeechSynthesizer.AllVoices).ToArray();
        }

    }
}
