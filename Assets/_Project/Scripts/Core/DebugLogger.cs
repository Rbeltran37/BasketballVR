using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public class DebugLogger : MonoBehaviour
{
    private const string INFO = "<color=cyan>INFO";
    private const string DEBUG = "<color=lime>DEBUG";
    private const string WARNING = "<color=yellow>WARNING";
    private const string ERROR = "<color=red>ERROR";
    private const string END = "</color>";
    private const string EMPTY = "";
    private const int PREVIOUS_CALL_SKIP_FRAME = 2;
    private const string IS_NULL_OR_EMPTY = "is NULL or EMPTY.";

    private static int _callNumber;
    

    public static void Info(string methodName, [CanBeNull] string message = EMPTY, [CanBeNull] Object classReference = null)
    {
        Log(INFO, methodName, message, classReference);
    }

    public static void Debug(string methodName, [CanBeNull] string message = EMPTY, [CanBeNull] Object classReference = null)
    {
        Log(DEBUG, methodName, message, classReference);
    }

    public static void Warning(string methodName, [CanBeNull] string message = EMPTY, [CanBeNull] Object classReference = null)
    {
        Log(WARNING, methodName, message, classReference);
    }

    public static void Error(string methodName, [CanBeNull] string message = EMPTY, [CanBeNull] Object classReference = null)
    {
        Log(ERROR, methodName, message, classReference);
    }

    private static void Log(string level, string methodName, string message = EMPTY, [CanBeNull] Object classReference = null)
    {
        if (message.Equals(EMPTY))
        {
            if (_callNumber == 0) CoroutineCaller.Instance.StartCoroutine(CallCoroutine());
            message = $"{_callNumber++}";
        }

        if (level.Equals(ERROR))
        {
            UnityEngine.Debug.LogError($"{level} | {methodName}: {message}{END}", classReference);
            return;
        }
        
        if (level.Equals(WARNING))
        {
            UnityEngine.Debug.LogWarning($"{level} | {methodName}: {message}{END}", classReference);
            return;
        }
        
        UnityEngine.Debug.Log($"{level} | {methodName}: {message}{END}", classReference);
    }

    public static bool IsNullOrEmpty<T>(IReadOnlyCollection<T> collection)
    {
        if (collection == null) return true;
        if (collection.Count == 0) return true;

        return false;
    }
    
    public static bool IsNullOrEmptyDebug<T>(IReadOnlyCollection<T> collection, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(DEBUG, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyDebug<T>(IReadOnlyCollection<T> collection, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(DEBUG, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyInfo<T>(IReadOnlyCollection<T> collection, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(INFO, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyInfo<T>(IReadOnlyCollection<T> collection, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(INFO, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyWarning<T>(IReadOnlyCollection<T> collection, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(WARNING, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyWarning<T>(IReadOnlyCollection<T> collection, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(WARNING, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyError<T>(IReadOnlyCollection<T> collection, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(ERROR, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullOrEmptyError<T>(IReadOnlyCollection<T> collection, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(ERROR, $"{IS_NULL_OR_EMPTY}. {additionalMessage}", classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsIndexOutOfRange<T>(IReadOnlyCollection<T> collection, int index)
    {
        if (IsNullOrEmpty(collection)) return true;
        if (index >= collection.Count) return true;
        
        return false;
    }
    
    public static bool IsIndexOutOfRangeDebug<T>(IReadOnlyCollection<T> collection, int index, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(DEBUG, IS_NULL_OR_EMPTY, classReference);
            return true;
        }
        
        if (index >= collection.Count)
        {
            LogStackTrace<T>(DEBUG, $"{nameof(index)}={index} is >= Count={collection.Count}", classReference);
            return true;
        }
        
        return false;
    }
    
    public static bool IsIndexOutOfRangeInfo<T>(IReadOnlyCollection<T> collection, int index, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(INFO, IS_NULL_OR_EMPTY, classReference);
            return true;
        }
        
        if (index >= collection.Count)
        {
            LogStackTrace<T>(INFO, $"{nameof(index)}={index} is >= Count={collection.Count}", classReference);
            return true;
        }
        
        return false;
    }
    
    public static bool IsIndexOutOfRangeWarning<T>(IReadOnlyCollection<T> collection, int index, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(WARNING, IS_NULL_OR_EMPTY, classReference);
            return true;
        }
        
        if (index >= collection.Count)
        {
            LogStackTrace<T>(WARNING, $"{nameof(index)}={index} is >= Count={collection.Count}", classReference);
            return true;
        }
        
        return false;
    }
    
    public static bool IsIndexOutOfRangeError<T>(IReadOnlyCollection<T> collection, int index, Object classReference = null)
    {
        if (IsNullOrEmpty(collection))
        {
            LogStackTrace<T>(ERROR, IS_NULL_OR_EMPTY, classReference);
            return true;
        }
        
        if (index >= collection.Count)
        {
            LogStackTrace<T>(DEBUG, $"{nameof(index)}={index} is >= Count={collection.Count}", classReference);
            return true;
        }
        
        return false;
    }
    
    public static bool IsNullDebug<T>(T objectToCheck, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(DEBUG, additionalMessage, classReference);

        return true;
    }

    public static bool IsNullDebug<T>(T objectToCheck, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(DEBUG, additionalMessage, classReference);

        return true;
    }
    
    public static bool IsNullInfo<T>(T objectToCheck, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(INFO, additionalMessage, classReference);

        return true;
    }

    public static bool IsNullInfo<T>(T objectToCheck, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(INFO, additionalMessage, classReference);

        return true;
    }
    
    public static bool IsNullWarning<T>(T objectToCheck, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(WARNING, additionalMessage, classReference);

        return true;
    }

    public static bool IsNullWarning<T>(T objectToCheck, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (objectToCheck == null)
        {
            LogNullStackTrace<T>(WARNING, additionalMessage, classReference);
            return true;
        }

        return false;
    }
    
    public static bool IsNullError<T>(T objectToCheck, string additionalMessage = EMPTY, Object classReference = null)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(ERROR, additionalMessage, classReference);

        return true;
    }

    public static bool IsNullError<T>(T objectToCheck, Object classReference = null, string additionalMessage = EMPTY)
    {
        if (objectToCheck != null) return false;

        LogNullStackTrace<T>(ERROR, additionalMessage, classReference);

        return true;
    }

    private static void LogNullStackTrace<T>(string level, string additionalMessage, Object classReference)
    {
        var method = new StackFrame(PREVIOUS_CALL_SKIP_FRAME).GetMethod();
        var methodName = method.Name;
        var objectName = EMPTY;

        LogNull<T>(level, methodName, objectName, additionalMessage, classReference);
    }

    private static void LogNull<T>(string level, string methodName, string objectName, string additionalMessage, Object classReference)
    {
        if (objectName.Equals(EMPTY)) objectName = $"Object of Type {typeof(T)}";

        Log(level, methodName, $"{objectName} is null. {additionalMessage}", classReference);
    }
    
    private static void LogStackTrace<T>(string level, string additionalMessage, Object classReference)
    {
        var method = new StackFrame(PREVIOUS_CALL_SKIP_FRAME).GetMethod();
        var methodName = method.Name;

        Log(level, methodName, $"{typeof(T)} {additionalMessage}.", classReference);
    }

    private static IEnumerator CallCoroutine()
    {
        yield return null;
        _callNumber = 0;
    }
}
