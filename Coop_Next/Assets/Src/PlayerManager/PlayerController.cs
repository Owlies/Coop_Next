using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : OverridableMonoBehaviour {
    private int playerId;
    private InputController inputController;
    public PlayerController(InputController iController, int pId) {
        inputController = iController;
        playerId = pId;
    }
}
