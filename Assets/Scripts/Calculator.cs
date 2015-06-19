using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Calculator : MonoBehaviour {

	public enum ButtonType { Number, Operation, Decimal };


	public Text displayText;


	private string lastButtonPressed = "";
	private double? memoryQuantity = null;
	private string memoryOperation = "";
	private int maxCharacters = 10;
	private bool readyForNewQuantity = true;


	#region Set-up and teardown

	void OnEnable()
	{
		InitCalc();
	}


	void InitCalc()
	{
		displayText.text = "0";
		memoryQuantity = null;
		memoryOperation = "";
		readyForNewQuantity = true;
	}
	#endregion


	#region Process operations

	public void RegisterButtonPress(CalcButton button)
	{
		if (button.typeOfButton == ButtonType.Number)
		{
			if (displayText.text == "0")
			{
				StartNewQuantity(button.name);
			}
			else
			{
				// If the calculator didn't most recently spit out a result, ...
				if (!readyForNewQuantity)
				{
					// ... if there is room to keep adding digits, then add this one.
					AppendToQuantity(button.name);
				}
				// If the calculator DID most recently spit out a result and expects a new quantity, ...
				else
				{
					// ... then start building the quantity from scratch with the name of the button that got pressed.
					StartNewQuantity(button.name);
				}
			}
		}
		else if (button.typeOfButton == ButtonType.Operation)
		{
			switch (button.name)
			{
				case "Sine":
					displayText.text = Mathf.Sin((float)DoubleParseDisplay()).ToString();
					break;
				case "Cosine":
					displayText.text = Mathf.Cos((float)DoubleParseDisplay()).ToString();
					break;
				case "Tangent":
					displayText.text = Mathf.Tan((float)DoubleParseDisplay()).ToString();
					break;
				case "C":
					if (lastButtonPressed == button.name)
						InitCalc();
					else
						displayText.text = "0";
					break;
				case "Add":
				case "Subtract":
				case "Multiply":
				case "Divide":
					if (memoryQuantity != null)
					{
						PerformOperation();
					}
					memoryOperation = button.name;
					memoryQuantity = DoubleParseDisplay();
					break;
				case "Equals":
					PerformOperation();
					memoryQuantity = null;
					memoryOperation = "";
					break;
				default:
					break;
			}

			
			// Handles cases when the result is huge or very small (e.g. 1.2342E+18)
			if (displayText.text.Contains("E") && displayText.text.Contains("."))
			{
				// Make a temp variable to hold the new display quantity.
				string newDisplay = "";


				// If there are enough digits between the decimal point and the E, ...
				if (displayText.text.IndexOf("E") - displayText.text.IndexOf(".") > 5)
				{
					// ...take the whole number, decimal, and 5 decimal places, ...
					newDisplay += displayText.text.Substring(0, 7);
					// ...and append the E and everything after.
					newDisplay += displayText.text.Substring(displayText.text.IndexOf("E"));
				}
				else
					newDisplay = displayText.text;

				
				// Set the text on screen to the temp variable.
				displayText.text = newDisplay;
			}


			// After pressing ANY operation button, the calculator expects the user to start on a new quantity.
			readyForNewQuantity = true;
		}
		// If it's a decimal point button...
		else
		{
			if (readyForNewQuantity)
			{
				displayText.text = "0.";
			}
			else if (displayText.text.Length < maxCharacters && !(displayText.text.Contains(".")))
			{
				AppendToQuantity(".");
			}


			readyForNewQuantity = false;
		}


		lastButtonPressed = button.name;
	}

	
	void StartNewQuantity (string buttonName)
	{
		displayText.text = buttonName;
		readyForNewQuantity = false;
	}


	void AppendToQuantity (string buttonName)
	{
		if (displayText.text.Length < maxCharacters)
		{
			displayText.text += buttonName;
		}
	}


	void PerformOperation()
	{
		switch (memoryOperation)
		{
			case "Add":
				displayText.text = (memoryQuantity + DoubleParseDisplay()).ToString();
				break;
			case "Subtract":
				displayText.text = (memoryQuantity - DoubleParseDisplay()).ToString();
				break;
			case "Multiply":
				displayText.text = (memoryQuantity * DoubleParseDisplay()).ToString();
				break;
			case "Divide":
				displayText.text = (memoryQuantity / DoubleParseDisplay()).ToString();
				break;
			default:
				break;
		}
	}


	double DoubleParseDisplay()
	{
		return double.Parse(displayText.text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}
	#endregion
}
