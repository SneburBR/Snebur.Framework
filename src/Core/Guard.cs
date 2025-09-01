using Snebur.Helpers;
using Snebur.Utilidade;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace Snebur;

public static class Guard
{

    public static void NotNull(
            [NotNull] object? value,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException($"{paramName} cannot be null.", paramName);
        }
    }

    public static void NotNullOrWhiteSpace(
            [NotNull] string? value,
            [CallerArgumentExpression(nameof(value))] string paramName = "")

    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
        }
    }

    public static void FileExists(
        [NotNull] string? filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Project file not found. {filePath}", filePath);
        }
    }
    public static void DirectoryExits(
         [NotNull] string? filePath)
    {
        if (!Directory.Exists(filePath))
        {
            throw new FileNotFoundException($"Project file not found. {filePath}", filePath);
        }
    }

    public static void MustBeGreaterThanZero<T>(
        IComparable<T> value,
        [CallerMemberName] string memberName = "")
        where T : struct
    {
        if (value.CompareTo(default) <= 0)
        {
            throw new ArgumentOutOfRangeException($"{memberName} deve ser maior que zero.");
        }
    }

    public static void NotEmpty<T>(
           [NotNull] ICollection<T> value,
           [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value.Count == 0)
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }

    public static void NotEmpty<T>(
      [NotNull] T value,
      [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

        var underlyingDefaultValue = TypeHelper.GetUnderlyingDefaultValue<T>();

        if (EqualityComparer<T>.Default.Equals(value, underlyingDefaultValue))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }

    public static void MustBeEmpty<T>(
        [NotNull] ICollection<T> value,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

        if (value.Count > 0)
            throw new ArgumentException($"{paramName} must be empty.", paramName);
    }

    public static void EnumDefined<TEnum>(
        [NotNull] TEnum value,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
        where TEnum : struct, Enum
    {
        if (!EnumUtil.IsDefined(value))
        {
            throw new ArgumentException(
                $"Parameter '{paramName}' with value '{value}' is not defined into enum '{typeof(TEnum).Name}'. Please provide a valid enum value.",
                paramName);
        }
    }
}

//public static class Guard
//{
//    public static void NotNull<T>(
//        [NotNull] T value,
//        [CallerArgumentExpression(nameof(value))] string? paramName = "")
//    {
//        if (value is null)
//            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
//    }

//    public static void NotNullOrWhiteSpace(
//        [NotNull] string? value,
//        [CallerArgumentExpression(nameof(value))] string? paramName = "")
//    {
//        if (string.IsNullOrWhiteSpace(value))
//            throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
//    }

//    public static void FullPhoneNumber(
//        [NotNull] string? fullNumber,
//        [CallerArgumentExpression(nameof(fullNumber))] string? paramName = "")
//    {
//        if (fullNumber is null)
//            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

//        if (!PhoneNumberValidationUtils.IsFullPhoneNumberValid(fullNumber))
//            throw new ArgumentException($"{paramName} is not a valid phone number.", paramName);
//    }

//    public static void Positive(
//        int value,
//        [CallerArgumentExpression(nameof(value))] string paramName = "")
//    {
//        if (value <= 0)
//            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
//    }

//    public static void Sha256(
//        [NotNull] string? value,
//        [CallerArgumentExpression(nameof(value))] string paramName = "")
//    {
//        if (value is null)
//            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

//        if (!ValidationUtils.IsSha256(value))
//            throw new ArgumentException($"{paramName} must be a SHA-256 hash value.", paramName);
//    }

//    public static void NotEmpty<T>(
//           [NotNull] ICollection<T> value,
//           [CallerArgumentExpression(nameof(value))] string paramName = "")
//    {
//        if (value.Count == 0)
//            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
//    }

//    public static void MustBeEmpty<T>(T value,
//        [CallerArgumentExpression(nameof(value))] string paramName = "")
//    {
//        if (value is null)
//            return;

//        if (!EqualityComparer<T>.Default.Equals(value, default))
//            throw new ArgumentException($"{paramName} must be empty.", paramName);
//    }

//    public static void MustBeEmpty<T>(
//        [NotNull] ICollection<T> value,
//        [CallerArgumentExpression(nameof(value))] string paramName = "")
//    {
//        if (value is null)
//            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

//        if (value.Count > 0)
//            throw new ArgumentException($"{paramName} must be empty.", paramName);
//    }

//    public static void EnumNotDefined<TEnum>(
//        [NotNull] TEnum value,
//        [CallerArgumentExpression(nameof(value))] string paramName = "")
//        where TEnum : struct, Enum
//    {
//        if (!EnumUtils.IsDefined(value))
//        {
//            throw new ArgumentException(
//                $"Parameter '{paramName}' with value '{value}' is not defined into enum '{typeof(TEnum).Name}'. Please provide a valid enum value.",
//                paramName);
//        }
//    }
//}
