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
        return (false, "Runtime error:" + Environment.NewLine + e.Message);
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
        return (false, "Runtime error:" + Environment.NewLine + e.Message);
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
        return (false, "Runtime error:" + Environment.NewLine + e.Message);
    }
    return (true, "");
}

public static string Help() 
{
    return "";
}