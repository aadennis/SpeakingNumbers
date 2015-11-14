using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using NumberSpeaker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumberSpeaker.Common;
using NumberSpeaker.ViewModel;


namespace UnitTestApp2
{


	[TestClass]
	public class UnitTest1 
	{
		[TestMethod]
		public void TestMethod1()
		{
			Assert.AreEqual(1, 1);
			var x = new SpeakNumbersViewModel(false);

		}

		[TestMethod]
		public void DictionaryContains2rowsBothValue1()
		{
			var randomNumberDictionary = NumberSpeaker.Common.RandomService.GetRandomIntSet(2, 1, 1);
			Assert.AreEqual(2, randomNumberDictionary.Count);

			// this all needs converting to a class not a dictionary, so we can more easily run Linq against it.
			// But for now we expect both values to be 1, as we asked for start as 1 and end as 1...
			foreach (var item in randomNumberDictionary) {
				Assert.AreEqual(1, item.Value);
			}

			//Keys are unique in a dictionary, so if we find "correct" (representing in the current single use-case 
			// the solitary correct value), then it must be the only one.
			var correctFound = false;
			foreach (var item in randomNumberDictionary)
			{
				if (!correctFound && item.Key.Equals("correct"))
				{
					correctFound = true;
				}
			}
			Assert.AreEqual(true, correctFound, "no Correct found");

	
		}

		[TestMethod]
		public void DictionaryContainsRequestedNumberOfRows()
		{
			var randomNumberDictionary = NumberSpeaker.Common.RandomService.GetRandomIntSet(500, 1, 1);
			Assert.AreEqual(500, randomNumberDictionary.Count);
		}

		[TestMethod]
		public void DictionaryLimitsAreWithinRequestRange()
		{
			var randomNumberDictionary = NumberSpeaker.Common.RandomService.GetRandomIntSet(500, 33, 37);
			var values = randomNumberDictionary.Values;
			int min = values.Min();
			int max = values.Max();

			Assert.IsTrue(min >= 33, string.Format("min outside the requested range [{0}]", min));
			Assert.IsTrue(max <= 37, string.Format("max outside the requested range [{0}]", max));

		}

		[TestMethod]
		public void ViewModelIsInstantiatedInDesignMode() {
			var vm = new SpeakNumbersViewModel(false);
			Assert.IsNotNull(vm);
		}

		[TestMethod]
		public void ViewModelIsInstantiatedInRunMode()
		{
			var vm = new SpeakNumbersViewModel(true);
			Assert.IsNotNull(vm);
		}

		[TestMethod]
		public void WrongAnswerCausesResponseToConfirmWrongAnswer() {
			var vm = new SpeakNumbersViewModel {NumberToGuess = "42", };
		}
	}
}
