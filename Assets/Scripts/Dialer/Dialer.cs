using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dialer : MonoBehaviour 
{
	public Text displayText;
	public Text callingText;
	public GameObject callingScreen;
	public enum ButtonType { Symbol, Action, Number };
	
	private int maxCharacters = 13;

	void OnEnable ()
	{
		InitPhone ();
	}
	
	void InitPhone ()
	{
		displayText.text = "";
	}

	//function that handles the actual working of phone. See above, three dif. types of button (symbol, action, number )

	public void RegisterButtonPress (DialerButton button)
	{
		// if the button is numeric (0-9)...
		if (button.typeOfButton == ButtonType.Number)
		{
			// if there is room on the display for the number...
			if (displayText.text.Length < maxCharacters)
			{
				displayText.text += button.gameObject.name;
			}
			// if the display contains an * or #, only worry about adding the number if there is room.
			// (also a check against later funny business with the clear button)
			if (displayText.text.Contains("*") || displayText.text.Contains("#"))
			{
				return;
			}
			// if however the display does not contain * or #...
			else if (!(displayText.text.Contains ("*") || displayText.text.Contains("#")))
			{
				// if a number button is pressed that results in the display text length
				// being longer than 3...
				if (displayText.text.Length > 3)
				{
					// and if the display does not alrady have parentheses...
					if(!displayText.text.Contains("("))
					{
						string newString;
			
						newString = "(" + displayText.text.Substring (0,3) + ")" +displayText.text.Substring (3);

						displayText.text = newString;
					}
				}
				// if a number button is pressed that results in the display text length
				// being longer than 8...
				if (displayText.text.Length > 8)
				{
					// and if the display does not already contain a hyphen...
					if(!displayText.text.Contains ("-"))
					{
						string newString;

						newString = displayText.text.Substring (0,8) + "-" + displayText.text.Substring (8);

						displayText.text = newString;
					}
				}
			}
		}
		// if we are dealing with * or #
		if (button.typeOfButton == ButtonType.Symbol)
		{
			// if the display text length is maxed out, do not execute any of this code
			if (displayText.text.Length == maxCharacters)
			{
				return;
			}
			// if the display text length is less than the max, display the symbol
			if (displayText.text.Length < maxCharacters)
			{
				displayText.text += button.name;
			}
			// if the display text is long enough to contain parentheses but hasn't activated a hyphen yet
			// e.g. (123)40
			if(displayText.text.Contains("(") && !displayText.text.Contains("-"))
			{
	     		string newString;

				newString = displayText.text.Substring(1,3) + displayText.text.Substring (5);

				displayText.text = newString;
			}
			// if the display text is long enough to contain a hyphen and the addition of the symbol will
			// mean the display length is less than or equal to the maxCharacters...
			if (displayText.text.Contains ("-") && displayText.text.Length <= maxCharacters)
			{
				string newString;

				newString = displayText.text.Substring(1,3) + displayText.text.Substring(5,3) + displayText.text.Substring (9);

				displayText.text = newString;
			}

		}
		// if the button is an action i.e. voicemail, call, or clear...
		if (button.typeOfButton == ButtonType.Action)
		{
			switch (button.name)
			{
				//if the button is clear
				case "Clear":
					//do nothing if the display text is empty
					if (displayText.text == "")
					{
						return;
					}
					// otherwise, take the current text and take the substring that is 1 shorter than the length
					// of the actual display text
					string newStringy;
					newStringy = displayText.text.Substring(0,(displayText.text.Length - 1));
					displayText.text = newStringy;
					// if after running the previous operation (the backspace action, essentially), the display
					// text's length is 9 and includes a hyphen (which in this case, would have no numbers after it)
					// remove the hyphen as well from the string
					if (displayText.text.Length == 9 && displayText.text.Contains("-"))
					{
						string newString1;
						newString1 = displayText.text.Substring(0,8);
						displayText.text = newString1;
					}
					// same process here, but when pertaining to parentheses e.g. remove parentheses if "Clear" results
					// in no numbers after the parentheses
					if (displayText.text.Length == 5 && displayText.text.Contains("("))
					{
						string newString2;
						newString2 = displayText.text.Substring(1,3);
						displayText.text = newString2;
					}
					// the following two checks are for when a string is of a certain length and includes two symbols
					// in succession. Clearing would result in a string with parentheses/hyphens and a symbol. no good.
					// Of course, for this, there also needs to be no parentheses/hyphens already present.
					if (displayText.text.Length >= 4 && !displayText.text.Contains ("(") && !(displayText.text.Contains ("*") || displayText.text.Contains ("#")))
					{
						string newString3;
						newString3 = "(" + displayText.text.Substring(0,3) + ")" + displayText.text.Substring(3);
						displayText.text = newString3;
					}
					if (displayText.text.Length >= 9 && !displayText.text.Contains ("-") && !(displayText.text.Contains ("*") || displayText.text.Contains ("#")))
					{
						string newString4;
						newString4 = displayText.text.Substring(0,8) + "-" + displayText.text.Substring(8);
						displayText.text = newString4;
					}
					break;
				// if the button pressed is the voicemail button...
				case "Voicemail":
					callingScreen.gameObject.SetActive(true);
					string newString5;
					newString5 = "Calling... Voicemail";
					callingText.text = newString5;
					break;
				// if the button pressed is the call button...
				case "Call":
					if (displayText.text == "")
					{
						return;
					}
					callingScreen.gameObject.SetActive (true);
					string newString6;
					newString6 = "Calling..." + displayText.text;
					callingText.text = newString6;
					break;
				default:
					break;
			}
		}
	}
		

}
