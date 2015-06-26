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
		public delegate void GameEvent();
		public delegate void AppEvent(App app);


		public bool AllowingInput { get; private set; }
		public Image phoneBG;
		public Calculator calculator;
		public Texts.TextsApp texts;
		public AppLoader appLoader;
		public Dialer dialer;
		

		void Awake()
		{
			AllowingInput = true;
		}


		void OnEnable()
		{
			texts.OnAnimStart += SuspendInput;
			texts.OnAnimEnd += ResumeInput;
		}


		void OnDisable()
		{
			texts.OnAnimStart -= SuspendInput;
			texts.OnAnimEnd -= ResumeInput;
		}


		void Start()
		{
			if (OnControllerStart != null)
				OnControllerStart(this);
		}


		#region Game flow

		void SuspendInput()
		{
			AllowingInput = false;
		}


		void ResumeInput()
		{
			AllowingInput = true;
		}
		#endregion
	}
}
