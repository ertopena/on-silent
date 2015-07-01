using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Calendar : MonoBehaviour {

	public enum ButtonDirection { Left, Right };

	public GameObject dateDisplayGrid;
	public GameObject calendarTilePrefab;
	public GameObject[] dateTiles;
	public Text monthNameDisplay;
	public Text calendarYear;

	private int currentMonth;
	private Color originalColor;
	
	string[] monthName = {
		"January",
		"February",
		"March",
		"April",
		"May",
		"June",
		"July",
		"August",
		"September",
		"October",
		"November",
		"December"
	};

	int[] daysInMonth = {
		31,
		28,
		31,
		30,
		31,
		30,
		31,
		31,
		30,
		31,
		30,
		31
	};

	//On Awake, this variable will store the original color of the tiles, so when they are not grey, they will be the adequate color.
	void Awake ()
	{
		originalColor = calendarTilePrefab.GetComponent<Image>().color;
	}

	void OnEnable ()
	{
		InitCalendar();
	}

	//On Enable, the Calendar must instantiate 42 calendar tiles, set the month name to December, and set the year to 2015. This corresponds
	//to the setting of the story.
	void InitCalendar () 
	{
		dateTiles = new GameObject[42];
		
		for (int i = 0; i < dateTiles.Length; i++)
		{
			dateTiles[i] = Instantiate(calendarTilePrefab, calendarTilePrefab.transform.position,Quaternion.identity) as GameObject;
			dateTiles[i].transform.SetParent(dateDisplayGrid.transform, false);
		}

		monthNameDisplay.text = monthName[11];
		calendarYear.text = "2015";
		currentMonth = 11; //private integer to call upon the month names. See the array at the start.
		tileColoring(); //function that handles tile coloring and tile number (for the date of the month). Defined below.
	}

	//this function is what will handle the buttons to switch between months.
	public void RegisterButtonPress (CalendarButton button)
	{
		//if we are going back in time...
		if (button.buttonDirection == ButtonDirection.Left)
		{
			//in the event that we have reached the earliest month (January), and the player presses the left button (go back)
			//prepare currentMonth to display December, and subtract one from the current year.
			if (monthNameDisplay.text == "January")
			{
				currentMonth = 12;
				string newStringy;
				newStringy = (DoubleParseYearDisplay() - 1).ToString();
				calendarYear.text = newStringy;
			}
			//execute the standard function i.e. go back one month value by subtracting 1 from currentMonth, used to define a position
			//in the monthName array.
				currentMonth = currentMonth - 1;
				string newString;
				newString = monthName[currentMonth];
				monthNameDisplay.text = newString;
		}
		//if we are going forward in time...
		if (button.buttonDirection == ButtonDirection.Right)
		{
			//in the event that we have reached the final month (December), and the player presses the right button (go forward)
			//prepare currentMonth to display January, and add one to the current year.
			if (monthNameDisplay.text == "December")
			{
				currentMonth = -1;
				string newString1;
				newString1 = (DoubleParseYearDisplay() + 1).ToString ();
				calendarYear.text = newString1;
			}
			//execute the standard function i.e. go forward one month value by adding 1 to currentMonth, used to define a position
			//in the monthName array.
			currentMonth = currentMonth + 1;
			string newString2;
			newString2 = monthName[currentMonth];
			monthNameDisplay.text = newString2;
		}
		//execute the tileColoring function for the new month, making sure the calendar continues to function.
		tileColoring();
	}

	void tileColoring ()
	{ 
		//if we are dealing with a leap year, February must have 29 days
		if ((int)DoubleParseYearDisplay() % 4 == 0)
		{
			daysInMonth[1] = 29;
		}
		//if not, 28
		else
		{
			daysInMonth[1] = 28;
		}

		//create a variable that will find the starting day (the first) for any given month in any given year.
		//notice the (currentMonth + 1). Our array is defined with ints 0-11 but system.DateTime defines months with 1-12.
		System.DayOfWeek day;
		System.DateTime initialWeekDay = new System.DateTime((int)DoubleParseYearDisplay(), (currentMonth + 1), 1); //year, month, day
		day = initialWeekDay.DayOfWeek; // if typecast to an int, DayOfWeek yields an int 0-6 for the weekday (Sun = 0, Sat = 6)

		for (int i = 0; i < dateTiles.Length; i++) //for loop for properly coloring and handling text in the tiles according to the month
		{
			//if the tile number (i) is between the initial day of the month (defined by variable day, typecast to an int here (see
			//line 139)) and the total days in the month (defined here by the tiles in positions starting at the initial day + the days in
			//the month - 1 (this is to prevent months from having an extra day)), then the tile will be the normal color, and have a number
			//counting the positions of the normal tiles starting from 1 to the end of the month.
			if (((int)day <= i) && (i <= ((int)day + (daysInMonth[currentMonth] - 1))))
			{
				dateTiles[i].GetComponent<Image>().color = originalColor;
				dateTiles[i].GetComponentInChildren<Text>().text = (i - (((int)day) - 1)).ToString();
			}
			//otherwise, tiles will be gray and have no number
			else
			{
				dateTiles[i].GetComponent<Image>().color = Color.gray;
				dateTiles[i].GetComponentInChildren<Text>().text = "";
			}
		}
		// if it is December 2015, then the 30th must be highlighted in blue (cyan), to indicate the present day in the story.
		for (int i = 0; i < dateTiles.Length; i++)
		{
			if ((monthNameDisplay.text == monthName[11]) && (calendarYear.text == "2015"))
			{
				dateTiles[31].GetComponent<Image>().color = Color.cyan;
			}
		}
	}
	//function to turn the string of the year into a number that can have mathematical operations performed to it (-1 or +1)
	double DoubleParseYearDisplay()
	{
		return double.Parse(calendarYear.text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
	}

	

}
