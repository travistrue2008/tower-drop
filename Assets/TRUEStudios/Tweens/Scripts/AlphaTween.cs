/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace TRUEStudios.Tweens {
	public class AlphaTween : Tween<float> {
		#region Fields
		[SerializeField]
		private SpriteRenderer _spriteRenderer;
		[SerializeField]
		private Graphic _graphic;

		private Color _cachedColor;
		#endregion

		#region Properties
		public SpriteRenderer AttachedSpriteRenderer {
			set { _spriteRenderer = value; }
			get { return _spriteRenderer; }
		}

		public Graphic AttachedGraphic {
			set { _graphic = value; }
			get { return _graphic; }
		}
		#endregion

		#region Methods
		#if UNITY_EDITOR
		private AlphaTween () {
			begin = end = 1.0f;
		}
		#endif

		protected override void Awake () {
			base.Awake();
			if (_spriteRenderer == null) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}

			if (_graphic == null) {
				_graphic = GetComponent<Graphic>();
			}
		}

		public override void ApplyResult () {
			_result = ((_end - _begin) * DistributedValue) + _begin;

			// apply the result to the SpriteRenderer
			if (_spriteRenderer != null) {
				_cachedColor = _spriteRenderer.color;
				_cachedColor.a = _result;
				_spriteRenderer.color = _cachedColor;
			}

			// apply the result to the Graphic
			if (_graphic != null) {
				_cachedColor = _graphic.color;
				_cachedColor.a = _result;
				_graphic.color = _cachedColor;
			}
		}
		#endregion
	}
}
