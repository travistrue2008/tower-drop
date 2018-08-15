using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour {
	#region Fields
	[SerializeField]
	private Ball _ball;
	[SerializeField]
	private RectTransform _containerProgress;
	[SerializeField]
	private RectTransform _fillProgress;
	[SerializeField]
	private TextMeshProUGUI _scoreLabel;
	[SerializeField]
	private TextMeshProUGUI _currentLevelLabel;
	[SerializeField]
	private TextMeshProUGUI _nextLevelLabel;

	private Vector2 _size;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_size = new Vector2(_containerProgress.sizeDelta.x, _fillProgress.sizeDelta.y);
		_ball.OnScore.AddListener(Refresh);
		Refresh();
	}

	private void Refresh () {
		_scoreLabel.text = $"{_ball.Score}";
		_currentLevelLabel.text = $"{_ball.Level}";
		_nextLevelLabel.text = $"{_ball.Level + 1}";

		// update the progress fill
		_size.x = _containerProgress.sizeDelta.x * _ball.Progress;
		_fillProgress.sizeDelta = _size;
	}
	#endregion
}
