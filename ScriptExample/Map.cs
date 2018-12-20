/*
This script automatically include:

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

Used packages:

System.ValueTuple

*/

public static (bool status, string desc) Install(string gamePath, string currentPath) 
{
    try
    {
        //Your code
    } 
    catch (Exception e)
    {
        return (false, "Runtime error:\n" + e.Message);
    }
    return (true, "");
}

public static (bool status, string desc) Remove(string gamePath, string currentPath) 
{
    try
    {
        //Your code
    } 
    catch (Exception e)
    {
        return (false, "Runtime error:\n" + e.Message);
    }
    return (true, "");
}

public static (bool status, string desc) Deploy(string gamePath, string currentPath, string parameter) 
{
    try
    {
        //Your code
    } 
    catch (Exception e)
    {
        return (false, "Runtime error:\n" + e.Message);
    }
    return (true, "");
}

public static string Help() 
{
    return "Level deploy help:" + Environment.NewLine +
    "Parameter formation: LEVEL-INDEX" + Environment.NewLine +
    "Parameter example: 1, 12" + Environment.NewLine +
    Environment.NewLine +
    "LEVEL-INDEX's legal value range is from 1 to 15";
}

//============================================= assistant function

static void CopyWithBackups(string target, string origin)
{
    if (File.Exist(target))
    {
        if (!File.Exist(target + ".bak")) File.Move(target, target + ".bak")
        else File.Delete(target);
    }

    File.Copy(origin, target);
}

static void RemoveWithRestore(string target)
{
    if (File.Exist(target))
        File.Delete(target);
    if (File.Exist(target + ".bak"))
        File.Move(target + ".bak", target);
}

static void RecordDeploy(string file, string value)
{
    using (var fs = new StreamWriter(file, false, Encoding.UTF8))
    {
        fs.Write(value);
        fs.Close();
    }
}

static string ReadDeploy(string file)
{
    if (!File.Exist(file)) return "";
    string res = "";
    using (var fs = new StreamReader(file, Encoding.UTF8))
    {
        res = fs.ReadToEnd();
        fs.Close();
    }

    return res;
}
