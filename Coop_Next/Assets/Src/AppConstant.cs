using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConstant : Singleton<AppConstant> {
    public float playerHorizontalSpeed = 100.0f;
    public float playerVerticalSpeed = 100.0f;
    public float playerActionLongPressThreshold = 0.3f;
    public bool isMultiPlayer = false;
}
