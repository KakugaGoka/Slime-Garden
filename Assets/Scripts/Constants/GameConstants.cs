using UnityEngine;

public enum ColorConstant {
    Red,
    Green,
    Blue,
    Cyan,
    Purple,
    Yellow,
    White,
    Black,
    Grey
}

public class GameConstants
{
    // all the constant string used across the game
    public const string k_AxisNameVertical                  = "Vertical";
    public const string k_AxisNameHorizontal                = "Horizontal";
    public const string k_MouseAxisNameVertical             = "Mouse Y";
    public const string k_MouseAxisNameHorizontal           = "Mouse X";
    public const string k_ButtonNameJump                    = "Jump";
    public const string k_ButtonNameFire                    = "Fire";
    public const string k_ButtonNameSprint                  = "Sprint";
    public const string k_ButtonNameCrouch                  = "Crouch";
    public const string k_ButtonNameSwitchWeapon            = "Mouse ScrollWheel";
    public const string k_ButtonNameSubmit                  = "Submit";
    public const string k_ButtonNameCancel                  = "Cancel";
    public const string k_ButtonNameInteract                = "Interact";
    public const string k_ButtonNameDrop                    = "Drop";

    //Vector4 constants for color creation
    public static Color k_Red = new Color(1f, 0f, 0f, 1f);
    public static Color k_Green = new Color(0f, 1f, 0f, 1f);
    public static Color k_Blue = new Color(0f, 0f, 1f, 1f);
    public static Color k_Cyan = new Color(0f, 1f, 1f, 1f);
    public static Color k_Purple = new Color(1f, 0f, 1f, 1f);
    public static Color k_Yellow = new Color(1f, 1f, 0f, 1f);
    public static Color k_Black = new Color(0f, 0f, 0f, 1f);
    public static Color k_White = new Color(1f, 1f, 1f, 1f);
    public static Color k_Grey = new Color(0.5f, 0.5f, 0.5f, 1f);

    public static Color ColorFromConstant(ColorConstant color) {
        switch (color) {
            case ColorConstant.Black:
                return k_Black;
            case ColorConstant.White:
                return k_White;
            case ColorConstant.Grey:
                return k_Grey;
            case ColorConstant.Red:
                return k_Red;
            case ColorConstant.Green:
                return k_Green;
            case ColorConstant.Blue:
                return k_Blue;
            case ColorConstant.Cyan:
                return k_Cyan;
            case ColorConstant.Purple:
                return k_Purple;
            case ColorConstant.Yellow:
                return k_Yellow;
        }
        return k_White;
    }
}
