using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger {

    private static bool log = false;

    public static void Log(string logString) {
        if (log) {
            Logger.Log(logString);
        }
    }
}
