using System;
using System.Collections.Generic;
using Verse;

namespace DArcaneTechnology;

internal static class D
{
    private static readonly string PREFIX = "[ArcaneTechnology] ";

    private static void LOG(string line, Exception e = null, int level = 0)
    {
        if (level > 1)
            Log.Error(line + (e != null ? "\n" + e : ""));
        else if (level == 1)
            Log.Warning(line + (e != null ? "\n" + e : ""));
        else
            Log.Message(line + (e != null ? "\n" + e : ""));
    }

    public static void Text(string line, Exception ex = null)
    {
        LOG(PREFIX + line, ex);
    }

    public static void Warning(string line, Exception ex = null)
    {
        LOG(PREFIX + line, ex, 1);
    }


    public static void Debug(string title)
    {
        LOG(PREFIX + "[DEBUG] " + title);
    }


    public static void Error(string title, Exception ex = null)
    {
        LOG(PREFIX + "[ERROR] " + title, ex, 2);
    }


    public static void List<T>(string title, IEnumerable<T> list, bool internalList = false)
    {
        if (internalList)
        {
            var output = PREFIX + title;
            output += "\n" + "===== LIST =====";
            foreach (var t in list)
            {
                var str = "     ";
                var t2 = t;
                var v = str + (t2 != null ? t2.ToString() : null);
                if (!v.NullOrEmpty())
                    output += "\n" + v;
            }

            LOG(output);
        }
        else
        {
            LOG(PREFIX + "===== LIST - " + title + " =====");
            try
            {
                foreach (var t in list)
                {
                    var str = "     ";
                    var t2 = t;
                    LOG(str + (t2 != null ? t2.ToString() : null));
                }
            }
            catch (Exception ex)
            {
                LOG(PREFIX + "Error printing list: " + ex.Message);
            }
        }
    }
}