namespace NumberSpeaker.Model
{
    public class GuessableNumber {
        
        private bool _isTheAnswer;
        private int _displayedNumber;


        public GuessableNumber(bool IsTheAnswer, int DisplayedNumber)
        {
            _isTheAnswer = IsTheAnswer;
            _displayedNumber = DisplayedNumber;
        }

        /// <summary>
        /// In a set of Guessable Numbers, represents the single item that is correct.
        /// </summary>
        public bool IsTheAnswer {
            get { return _isTheAnswer;}
            set { _isTheAnswer = value;}
        }

        /// <summary>
        /// 
        /// </summary>
        public int DisplayedNumber {    
            get { return _displayedNumber;}
            set { _displayedNumber = value;} }
    }
}
