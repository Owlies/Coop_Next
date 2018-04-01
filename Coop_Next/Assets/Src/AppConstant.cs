using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConstant : Singleton<AppConstant> {
    public float playerMovingSpeed = 100.0f;
    public float playerActionLongPressThreshold = 0.3f;
    public float playerActionRange = 1.0f;
    public bool isMultiPlayer = false;
    public float resourceCollectingSeconds = 2.0f;
    public float moveBuildingScaleChange = 0.5f;
}
