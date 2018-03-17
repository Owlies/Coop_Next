using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PCInputController : InputController
{
    private string playerHorizontalAxisName;
    private string playerVerticalAxisName;

    public PCInputController(int pId) : base(pId)
    {
    }

    protected override void handleMovement()
    {
        base.handleMovement();
        

    }

    protected override void handleAction()
    {
        base.handleAction();
    }

}


