using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Phone
{
	public class HomeScroller : MonoBehaviour
	{
		/*
		 * When we drag the Home Screen ScrollRect to reveal another home screen, this script will lerp the Scrollbar's value so that
		 * whatever screen the player chooses is always centered on the phone screen.
		 */ 


		public int numberOfScreens = 3;
		public float xFirstPage, xLastPage;
		public RectTransform homeScreensRect;			// Each home screen page is a child within this wider rect.
		
		
		private float xChangeForNextScreen;


		void OnEnable()
		{
			if (numberOfScreens > 1)
				xChangeForNextScreen = (xLastPage - xFirstPage) / (float)(numberOfScreens - 1);
			else
			{
#if UNITY_EDITOR
				Debug.Log("HomeScroller.numberOfScreens set too low. Script not necessary (consider disabling it).");
#endif
				xChangeForNextScreen = xLastPage - xFirstPage;
			}
		}


		public void HoneInOnHomeScreen()
		{
			Debug.Log("Called HoneInOnHomeScreen()");

			StopAllCoroutines();

			for (int i = 0; i < numberOfScreens; i++)
			{
				if (DoesValueCorrespondToScreen(i))
				{
					StartCoroutine(CoChooseHomeScreen(i));
					return;
				}
			}
		}


		// Where screen = 0 is the first home screen.
		float ValueForCenterOfScreen(int screen)
		{
			if (numberOfScreens < 2)
				return (xLastPage - xFirstPage) / 2f;

			Debug.Log("Value for center of screen " + screen.ToString() + " is " + (xFirstPage + (float)(screen) * (xLastPage - xFirstPage) / (float)(numberOfScreens - 1)).ToString());
			return xFirstPage + (float)(screen) * (xLastPage - xFirstPage) / (float)(numberOfScreens - 1);
		}


		bool DoesValueCorrespondToScreen(int screen)
		{
			if (homeScreensRect.anchoredPosition.x < ValueForCenterOfScreen(screen) - xChangeForNextScreen / 2f &&
				homeScreensRect.anchoredPosition.x >= ValueForCenterOfScreen(screen) + xChangeForNextScreen / 2f)
				return true;

			Debug.Log("DoesValueCorrespondToScreen(" + screen.ToString() + ") returned false.");
			return false;
		}


		IEnumerator CoChooseHomeScreen(int screen)
		{
			Vector2 targetPos = new Vector2(ValueForCenterOfScreen(screen), homeScreensRect.anchoredPosition.y);

			Debug.Log("targetPos set to " + targetPos.ToString());
			
			for (int i = 0; i < 35; i++)
			{
				homeScreensRect.anchoredPosition = Vector2.Lerp(homeScreensRect.anchoredPosition, targetPos, 0.115f);
				yield return new WaitForFixedUpdate();
			}

			homeScreensRect.anchoredPosition = targetPos;
		}
	}
}
