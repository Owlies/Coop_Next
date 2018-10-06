using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAxisEnum {

    private InputAxisEnum(string value) { Value = value; }

    public string Value { get; set; }

    public static InputAxisEnum Menu { get { return new InputAxisEnum("Menu"); } }
    public static InputAxisEnum SinglePlayerHorizontal { get { return new InputAxisEnum("SinglePlayerHorizontal"); } }
    public static InputAxisEnum SinglePlayerVertical { get { return new InputAxisEnum("SinglePlayerVertical"); } }
    public static InputAxisEnum Player1_Horizontal { get { return new InputAxisEnum("Player1_Horizontal"); } }
    public static InputAxisEnum Player1_Vertical { get { return new InputAxisEnum("Player1_Vertical"); } }
    public static InputAxisEnum Player2_Horizontal { get { return new InputAxisEnum("Player2_Horizontal"); } }
    public static InputAxisEnum Player2_Vertical { get { return new InputAxisEnum("Player2_Vertical"); } }
    public static InputAxisEnum SinglePlayerAction { get { return new InputAxisEnum("SinglePlayerAction"); } }
    public static InputAxisEnum Player1_Action { get { return new InputAxisEnum("Player1_Action"); } }
    public static InputAxisEnum Player2_Action { get { return new InputAxisEnum("Player2_Action"); } }
    public static InputAxisEnum Submit { get { return new InputAxisEnum("Submit"); } }
    public static InputAxisEnum Cancel { get { return new InputAxisEnum("Cancel"); } }
    public static InputAxisEnum Horizontal { get { return new InputAxisEnum("Horizontal"); } }
    public static InputAxisEnum Vertical { get { return new InputAxisEnum("Vertical"); } }
}
