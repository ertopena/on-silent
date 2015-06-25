using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Phone
{
	[System.Serializable]
	public class Timestamp : System.IComparable<Timestamp>
	{
		public int month, day, hours, minutes, seconds;


		public int CompareTo(Timestamp other)
		{
			if (month != other.month)
				return this.month.CompareTo(other.month);
			else if (day != other.day)
				return this.day.CompareTo(other.day);
			else if (hours != other.hours)
				return this.hours.CompareTo(other.hours);
			else if (minutes != other.minutes)
				return this.minutes.CompareTo(other.minutes);
			else
				return this.seconds.CompareTo(other.seconds);
		}


		public System.DateTime ToDateTime()
		{
			return new System.DateTime(2015, month, day, hours, minutes, seconds);
		}
	}
	
	
	public class Controller : MonoBehaviour
	{
		public delegate void ControllerEvent(Controller ctrl);
		public static event ControllerEvent OnControllerStart;


		public Image phoneBG;
		public Calculator calculator;
		public Texts.TextsApp texts;
		public AppLoader appLoader;
		public Dialer dialer;
		

		void Start()
		{
			if (OnControllerStart != null)
				OnControllerStart(this);
		}
	}
}
