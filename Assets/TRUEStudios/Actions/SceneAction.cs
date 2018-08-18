/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAction : MonoBehaviour {
	#region Fields
	[SerializeField]
	private string _sceneName;
	#endregion

	#region Methods
	public void GotoScene () {
		SceneManager.LoadScene(_sceneName);
	}
	#endregion
}
