/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TRUEStudios.Core {
	[Serializable]
	public class BoolEvent : UnityEvent<bool> {}
	
	[Serializable]
	public class IntEvent : UnityEvent<int> {}

	[Serializable]
	public class FloatEvent : UnityEvent<float> {}
}
