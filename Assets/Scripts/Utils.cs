using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public static string ReverseString(string s) {
        char[] chars = s.ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }
}
