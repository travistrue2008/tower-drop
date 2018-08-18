using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TRUEStudios.State;
using TRUEStudios.Tweens;
using TMPro;

public class CompletedPopup : Popup {
	#region Fields
	[SerializeField]
	private TextMeshProUGUI _congratsLabel;
	[SerializeField]
	private TextMeshProUGUI _recordLabel;
	#endregion

	#region Properties
	#endregion

	#region MonoBehaviour Hooks
	protected override void Awake () {
		base.Awake();
	}

	public void SetLevel (int level) {
		_congratsLabel.text = $"Level {level} Passed";
	}

	public void SetBestScore (int score) {
		_recordLabel.text = $"NEW RECORD\n{score}";
		var tween = _recordLabel.GetComponent<Tween>();
		tween.PlayForward();
	}
	#endregion
}
