using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveItem : OverridableMonoBehaviour {
    public virtual bool LongPressAction(Player actor) { return true;}
    public virtual bool ShortPressAction(Player actor) { return true; }
    public virtual bool PressReleaseAction(Player actor) { return true; }
}
