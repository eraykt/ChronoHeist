using UnityEngine;

public static class Logger
{
    private const string COLOR_INFO = "#2196F3";
    private const string COLOR_SUCCESS = "#4CAF50";
    private const string COLOR_ERROR = "#F44336";

    public static void Info(object sender, string message)
    {
        Log(sender, message, COLOR_INFO);
    }

    public static void Success(object sender, string message)
    {
        Log(sender, message, COLOR_SUCCESS);
    }

    public static void Error(object sender, string message)
    {
        string formattedMessage = FormatMessage(sender, message, COLOR_ERROR);
        Debug.LogError(formattedMessage, sender as UnityEngine.Object);
    }

    private static void Log(object sender, string message, string colorHex)
    {
        string formattedMessage = FormatMessage(sender, message, colorHex);
        Debug.Log(formattedMessage, sender as UnityEngine.Object);
    }

    private static string FormatMessage(object sender, string message, string color)
    {
        string className = sender != null ? sender.GetType().Name : "Global";
        return $"<color={color}><b>[{className}]:</b> {message}</color>";
    }
}