using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConstant : Singleton<AppConstant> {
    public float playerHorizontalSpeed = 0.0f;
    public float playerVerticalSpeed = 0.0f;
    public float playerActionLongPressThreshold = 0.3f;
    public bool isMultiPlayer;
}
