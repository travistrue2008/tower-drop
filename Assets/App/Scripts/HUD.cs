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
	private RectTransform _progressContainer;
	[SerializeField]
	private RectTransform _streakContainer;
	[SerializeField]
	private RectTransform _fillProgress;
	[SerializeField]
	private TextMeshProUGUI _scoreLabel;
	[SerializeField]
	private TextMeshProUGUI _currentLevelLabel;
	[SerializeField]
	private TextMeshProUGUI _nextLevelLabel;
	[SerializeField]
	private TextMeshProUGUI _streamLabelPrefab;

	private int _oldScore = 0;
	private Vector2 _size;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_size = new Vector2(_progressContainer.sizeDelta.x, _fillProgress.sizeDelta.y);
		_ball.OnScore.AddListener(Refresh);
		Refresh();
	}

	private void Refresh () {
		_scoreLabel.text = $"{_ball.Score}";
		_currentLevelLabel.text = $"{_ball.Level}";
		_nextLevelLabel.text = $"{_ball.Level + 1}";

		// update the progress fill
		_size.x = _progressContainer.sizeDelta.x * _ball.Progress;
		_fillProgress.sizeDelta = _size;

		// check if the score has increased
		if (_oldScore < _ball.Score) {
			var obj = GameObject.Instantiate(_streamLabelPrefab);
			obj.transform.SetParent(_streakContainer);
			obj.transform.localScale = Vector3.one;
			var label = obj.GetComponent<TextMeshProUGUI>();
			label.text = $"+{_ball.Score - _oldScore}";
			Destroy(obj, 1.0f); // TODO: add object pooling
		}

		_oldScore = _ball.Score;
	}
	#endregion
}
