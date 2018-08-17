/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TRUEStudios.Tweens {
	[CustomEditor(typeof(ColorTween)), CanEditMultipleObjects]
	public class ColorTweenEditor : TweenEditor<ColorTween> { }
}
