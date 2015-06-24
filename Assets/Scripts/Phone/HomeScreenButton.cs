using UnityEngine;
using System.Collections;

namespace Phone
{
	public class HomeScreenButton : MonoBehaviour
	{
		public GameObject appToLoad;


		Phone.Controller gameController;


		void Start()
		{
			gameController = GetComponentInParent<Phone.Controller>();
		}


		public void LoadApp()
		{
			// TODO!
		}
	}
}
