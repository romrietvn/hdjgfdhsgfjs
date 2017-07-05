using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessEdit : MonoBehaviour 
{
	public bool IsWhite = false;
	public int Type = 1;

	public void OnSelected()
	{
		this.gameObject.GetComponent<Button> ().interactable = false;
	}
}
