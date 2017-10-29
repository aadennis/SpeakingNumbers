using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NumberSpeaker.Common;
using NumberSpeaker.Model;

namespace NumberSpeaker.ViewModel
{
    //http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
    //http://blogs.msdn.com/b/blaine/archive/2014/04/24/prism-5-0-for-wpf-just-released.aspx
    //http://msdn.microsoft.com/en-us/library/microsoft.practices.prism.mvvm.bindablebase_members(v=pandp.50).aspx

    public class SpeakNumbersViewModel : BindableBase
    {
        private SpeechService _language;
        private string _generatedGuess;
        private static string _targetLanguageCode = "fr"; //french french (using frs for French Swiss)
        private CoreDispatcher _dispatcher;
        private static string _spokenResponse;
        private static readonly string _reponseToOKAnswer = "C'est juste";
        private static readonly string _reponseToWrongAnswer = "C'est faux";
        private static readonly string _audioFileForRightAnswer = "chime01.mp3";
        private static readonly string _audioFileForWrongAnswer = "throttle01.mp3";

        private static readonly int _startOkTries = 0;
        private static readonly int _startTotalTries = 0;
        private static readonly int _defaultRangeStart = 1;
        private static readonly int _defaultRangeEnd = 100;
        private MediaElement _mediaElement;

        public async Task Play(string fileName)
        {
            var packageLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assetsFolder = await packageLocation.GetFolderAsync("Assets");

            var myAudio = await assetsFolder.GetFileAsync(fileName);
            if (myAudio != null)
            {
                var stream = await myAudio.OpenAsync(Windows.Storage.FileAccessMode.Read);
                MediaElement snd = new MediaElement();
                snd.SetSource(stream, myAudio.ContentType);
                snd.Play();
            }
        }  

        public SpeakNumbersViewModel() {
            DoIt(true);
        }

        public SpeakNumbersViewModel(bool inUiContext) {
            DoIt(inUiContext);
        }

        public async void DoIt(bool uiContext = true) {

            if (await CheckLanguageInstalled()) return;

            NotCurrentlySpeaking = true;
            OnPropertyChanged(() => NotCurrentlySpeaking);
            OkTries = _startOkTries;
            TotalTries = _startTotalTries;
            AutomaticNextQuestion = true;

            DoSpeech = new DelegateCommand<string>(Speak, CanExecuteSpeak);
            // constructor begin - this version assumes a string parameter
            CheckAnswerCommand = new DelegateCommand<string>(CheckAnswer, CanExecuteCheckAnswer);
            EndMeansCommand = new DelegateCommand<string>(EndMeans, CanEndMeans);
            StartMeansCommand = new DelegateCommand<string>(StartMeans, CanStartMeans);
            ContinuousMeansCommand = new DelegateCommand<string>(ContinuousMeans, CanContinuousMeans);

            RangeStart = _defaultRangeStart;
            RangeEnd = _defaultRangeEnd;

            NumbersToGuess = new ObservableCollection<GuessableNumber>();
            NumberToGuess = "";

            if (uiContext) {
                //http://www.wintellect.com/blogs/jlikness/mvvm-and-accessing-the-ui-thread-in-windows-store-apps
                //http://csharperimage.jeremylikness.com/2013/06/mvvm-and-accessing-ui-thread-in-windows.html
                _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            }
        }

        private bool CanExecuteSpeak(string arg) {
            return NotCurrentlySpeaking;
        }
        
        // Check if the user has the language installed...
        private async Task<bool> CheckLanguageInstalled() {

            _language = new SpeechService(_targetLanguageCode);
            var languageFound = _language.IsLanguageFound();
            if (!languageFound) {
                var errorMessage =
                    string.Format(
                        "Requested language code[{0}] is not installed. Please exit and install the [{1}] speech pack for Windows Phone 8.1.",
                        _targetLanguageCode, _targetLanguageCode);
                var errorDialog = new MessageDialog(errorMessage);
                await errorDialog.ShowAsync();
                return true;
            }
            return false;
        }

        private static bool CanExecuteCheckAnswer(string arg) {
            return !string.IsNullOrEmpty(arg);
        }

        private ObservableCollection<GuessableNumber> _numbersToGuess;

        public ObservableCollection<GuessableNumber> NumbersToGuess {
            get { return _numbersToGuess; }
            set {
                if (_numbersToGuess == value) return;
                _numbersToGuess = value;
                OnPropertyChanged(() => NumbersToGuess);
            }

        }

        private int _totalTries;

        public int TotalTries
        {
            get { return _totalTries; }
            set
            {
                if (_totalTries == value) return;
                _totalTries = value;
                OnPropertyChanged(() => TotalTries);
                OnPropertyChanged(() => Score);

            }

        }

        private int _okTries;

        public int OkTries
        {
            get { return _okTries; }
            set
            {
                if (_okTries == value) return;
                _okTries = value;

                OnPropertyChanged(() => OkTries);
                
            }

        }

        private string CalcScore() {
            return string.Format("{0}/{1}", OkTries, TotalTries);
        }

        private string  _numberToGuess;

        public string NumberToGuess
        {
            get { return _numberToGuess; }
            set
            {
                if (_numberToGuess == value) return;
                _numberToGuess = value;
                OnPropertyChanged(() => NumberToGuess);
            }

        }

        private string _score;
        public string Score
        {
            get { return _score; }
            private set
            {
                if (_score == value) return;
                _score = value;
                OnPropertyChanged(() => Score);
            }

        }

        private string GetScore()
        {
            return string.Format("{0}/{1}", OkTries, TotalTries);
        }

        private ICommand _languageChecked;
        public ICommand LanguageChecked
        {
            get { return _languageChecked; }
            set
            {
                if (_languageChecked == value) return;
                _languageChecked = value;
                OnPropertyChanged(() => LanguageChecked);
            }
        }

        private ICommand _checkAnswerCommand;
        public  ICommand CheckAnswerCommand {
            get {	return _checkAnswerCommand; }
            set {	if (_checkAnswerCommand == value) return;
                _checkAnswerCommand = value;
                OnPropertyChanged(() => CheckAnswerCommand);
            }
        }

        private async void CheckAnswer(string cmdParameter) {
            bool correctAnswer = false;
            TotalTries++;
            OnPropertyChanged(()=>TotalTries);

            if (cmdParameter.Equals(_generatedGuess)) {
                correctAnswer = true;
                var i = OkTries++;
                Score = GetScore();
                OnPropertyChanged(() => OkTries);
                OnPropertyChanged(() => AutomaticNextQuestion);
            }

            // Say Right or Wrong, or the equivalent sound files denoting that (chime and throttle respectively)
            _spokenResponse = correctAnswer ? _audioFileForRightAnswer : _audioFileForWrongAnswer;
            Play(_spokenResponse);
            //_language.SpeakTheTranslatedText(_spokenResponse);

            // If they got it wrong, cut them some slack and repeat the number...
            if (!correctAnswer) {
                //Play("throttle01.mp3");
                await Task.Delay(TimeSpan.FromSeconds(1));
                _language.SpeakTheTranslatedText(_generatedGuess);
            }
           
            // until I understand how to get the next bit to pause until we have spoken (can I somehow do async/await on that?)
            // for now naively sleep for n seconds
            
            NumberToGuess = "";
            IsTextBoxFocused = true;
            if (correctAnswer && AutomaticNextQuestion) {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                Speak(null);
            }
        }

// Example binding for the xaml
// <Button Content="CheckAnswer" HorizontalAlignment="Left" Height="36" Margin="120,201,0,0" VerticalAlignment="Top" Width="111" Command="{Binding CheckAnswerCommand}" CommandParameter="{Binding Text, ElementName=MyBox}" />
      

        private ICommand _doSpeech;
        public ICommand DoSpeech
        {
            get { return _doSpeech; }
            set
            {
                if (_doSpeech == value) return;
                _doSpeech = value;
                OnPropertyChanged(() => DoSpeech);
                OnPropertyChanged(() => FirstNumberGenerated);
            }
        }


        private ICommand _continuousMeansCommand;
        public  ICommand ContinuousMeansCommand {
            get {	return _continuousMeansCommand; }
            set {	if (_continuousMeansCommand == value) return;
                _continuousMeansCommand = value;
                OnPropertyChanged(() => ContinuousMeansCommand);
            }
        }

        private ICommand _startMeansCommand;
        public  ICommand StartMeansCommand {
            get {	return _startMeansCommand; }
            set {	if (_startMeansCommand == value) return;
                _startMeansCommand = value;
                OnPropertyChanged(() => StartMeansCommand);
            }
        }

        private ICommand _endMeansCommand;
        public  ICommand EndMeansCommand {
            get {	return _endMeansCommand; }
            set {	if (_endMeansCommand == value) return;
                _endMeansCommand = value;
                OnPropertyChanged(() => EndMeansCommand);
            }
        }

        private bool CanStartMeans(string cmdParameter)
        {
            // Add logic here. If true is returned, then EndMeans is called.
            // Remove the unconditional return, as required.
            return true;
        }

        private async static void StartMeans(string cmdParameter)
        {
            var errorDialog = new MessageDialog("[Start] means the smallest number you want to hear in the language.");
            await errorDialog.ShowAsync();

        }

        private bool CanEndMeans(string cmdParameter) {
            return true;
        }

        private async static void EndMeans(string cmdParameter) {
            var errorDialog = new MessageDialog("[End] means the largest number you want to hear in the language.");
            await errorDialog.ShowAsync();
        }

         private bool CanContinuousMeans(string cmdParameter) {
            return true;
        }

        private async static void ContinuousMeans(string cmdParameter) {
            var errorDialog = new MessageDialog("Checking [Continue] means that if you get a number right, you don't need to click 'Next Number'");
            await errorDialog.ShowAsync();
        }

// Example binding for the xaml
// <Button Content="EndMeans" HorizontalAlignment="Left" Height="36" Margin="120,201,0,0" VerticalAlignment="Top" Width="111" Command="{Binding EndMeansCommand}" CommandParameter="{Binding Text, ElementName=MyBox}" />
      


        private async void Speak(string cmdParameter) {

            if (!NotCurrentlySpeaking) return;

            if (RangeStart > RangeEnd) {
                RangeStart = 1;
                RangeEnd = 100;
                var msg = new MessageDialog("The start number must not be bigger than the end number. I'm setting it back to the default range.");
                IAsyncOperation<IUICommand> asyncOperation = msg.ShowAsync();

                return;
            }

            RangeStart = Math.Abs(RangeStart);
            RangeEnd = Math.Abs(RangeEnd);

            var o = new object();

            NotCurrentlySpeaking = false;
            OnPropertyChanged(() => NotCurrentlySpeaking);
            // Just crudely wait for say n seconds before allowing anything new until I have worked this out...
            await Task.Delay(TimeSpan.FromSeconds(1));
            _generatedGuess = (RandomService.GetRandomInt(RangeStart, RangeEnd)).ToString();



            _language.SpeakTheTranslatedText(_generatedGuess);



            NumberToGuess = "";

            FirstNumberGenerated = true;
            IsTextBoxFocused = true;
            NotCurrentlySpeaking = true;
            OnPropertyChanged(() => NotCurrentlySpeaking);
            await Task.Delay(TimeSpan.FromSeconds(0));

        }

      


       

        private bool _isFocused = false;
        public bool IsTextBoxFocused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                if (_isFocused == value)
                {
                    _isFocused = false;
                    OnPropertyChanged(() => IsTextBoxFocused);
                }
                _isFocused = value;
                OnPropertyChanged(() => IsTextBoxFocused);
            }
        } 


        private int _rangeStart;
        public int RangeStart {
            get {
                return _rangeStart; }
            set {
                if (_rangeStart == value) return;
                _rangeStart = value;
                OnPropertyChanged(() => RangeStart);
            }
        }

        private int _rangeEnd;
        public int RangeEnd {
            get {
                return _rangeEnd; }
            set {
                if (_rangeEnd == value) return;
                _rangeEnd = value;
                OnPropertyChanged(() => RangeEnd);
            }
        }

        private bool _numberEntryBoxIsPopulated;
        public bool NumberEntryBoxIsPopulated {
            get {
                return _numberEntryBoxIsPopulated; }
            set {
                if (_numberEntryBoxIsPopulated == value) return;
                _numberEntryBoxIsPopulated = NumberToGuess.Length > 0;
                OnPropertyChanged(() => NumberEntryBoxIsPopulated);
            }
        }

        private bool _automaticNextQuestion;
        public bool AutomaticNextQuestion {
            get {
                return _automaticNextQuestion; }
            set {
                if (_automaticNextQuestion == value) return;
                _automaticNextQuestion = value;
                OnPropertyChanged(() => AutomaticNextQuestion);
            }
        }

        private bool _firstNumberGenerated;
        public bool FirstNumberGenerated {
            get {
                return _firstNumberGenerated; }
            set {
                if (_firstNumberGenerated == value) return;
                _firstNumberGenerated = value;
                OnPropertyChanged(() => FirstNumberGenerated);
            }
        }

        private bool _notCurrentlySpeaking;
        public bool NotCurrentlySpeaking {
            get {
                return _notCurrentlySpeaking;
            }
            set {
                if (_notCurrentlySpeaking == value) return;
                _notCurrentlySpeaking = value;
                OnPropertyChanged(() => NotCurrentlySpeaking);
            }
        }



    }
}
