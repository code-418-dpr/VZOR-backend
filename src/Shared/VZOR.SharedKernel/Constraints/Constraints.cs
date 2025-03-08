using System.Text.RegularExpressions;

namespace VZOR.SharedKernel.Constraints;

public static partial class Constraints
{
    public static readonly int MAX_VALUE_LENGTH = 100;
    public static readonly double MIN_VALUE = 0;
    public static readonly int MIDDLE_NAME_LENGTH = 50;
    public static readonly int MIN_LENGTH_PASSWORD = 8;
    
    public static readonly Regex ValidationRegex = new Regex(
        @"^[\w-\.]{1,40}@([\w-]+\.)+[\w-]{2,4}$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static readonly Regex ValidationPassword = new Regex(
        @"^(?=.*[A-Z])(?=.*\d).{8,}$", RegexOptions.Singleline | RegexOptions.Compiled);
    
    public static string[] Extensions = [".jpg", ".png", ".jpeg", ".svg"];
    
    public enum Contexts
    {
        AuthContext,
        ImagesContext
    }
}