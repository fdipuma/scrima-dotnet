using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scrima.Queryable;

internal static class Methods
{
    public static readonly MethodInfo StringConcat =
        ReflectionHelper.GetMethod<object[], string>(x => string.Concat(x));

    public static readonly MethodInfo EnumerableConcat =
        ReflectionHelper.GetMethod<IEnumerable<object>, IEnumerable<object>>(x => x.Concat(new object[0]));

    public static readonly MethodInfo StringContains =
        ReflectionHelper.GetMethod<string, bool>(x => x.Contains(""));

    public static readonly MethodInfo StringEndsWith =
        ReflectionHelper.GetMethod<string, bool>(x => x.EndsWith(""));

    public static readonly MethodInfo StringStartsWith =
        ReflectionHelper.GetMethod<string, bool>(x => x.StartsWith(""));

    public static readonly MethodInfo StringIndexOf =
        // ReSharper disable once StringIndexOfIsCultureSpecific.1
        ReflectionHelper.GetMethod<string, int>(x => x.IndexOf(""));

    public static readonly MethodInfo StringSubstringOneParam =
        ReflectionHelper.GetMethod<string, string>(x => x.Substring(0));

    public static readonly MethodInfo StringSubstringTwoParam =
        ReflectionHelper.GetMethod<string, string>(x => x.Substring(0, 1));

    public static readonly MethodInfo EnumerableContains =
        ReflectionHelper.GetMethod<IEnumerable<object>, bool>(x => x.Contains(null));

    public static readonly MethodInfo EnumerableSkip =
        ReflectionHelper.GetMethod<IEnumerable<object>, IEnumerable<object>>(x => x.Skip(1));

    public static readonly MethodInfo EnumerableTake =
        ReflectionHelper.GetMethod<IEnumerable<object>, IEnumerable<object>>(x => x.Take(1));

    public static readonly PropertyInfo StringLength =
        (PropertyInfo)ReflectionHelper.GetMember<string, int>(x => x.Length);

    public static readonly MethodInfo EnumerableCount =
        ReflectionHelper.GetMethod<IEnumerable<object>, int>(x => x.Count());

    public static readonly MethodInfo StringToLowerInvariant =
        ReflectionHelper.GetMethod<string, string>(x => x.ToLower());

    public static readonly MethodInfo StringToUpperInvariant =
        ReflectionHelper.GetMethod<string, string>(x => x.ToUpper());

    public static readonly MethodInfo StringTrim =
        ReflectionHelper.GetMethod<string, string>(x => x.Trim());

    public static readonly MemberInfo DateTimeOffsetDate =
        ReflectionHelper.GetMember<DateTimeOffset, DateTime>(x => x.Date);

    public static readonly MemberInfo DateTimeOffsetTime =
        ReflectionHelper.GetMember<DateTimeOffset, TimeSpan>(x => x.TimeOfDay);

    public static readonly MemberInfo DateTimeOffsetOffset =
        ReflectionHelper.GetMember<DateTimeOffset, TimeSpan>(x => x.Offset);

    public static readonly MemberInfo DateTimeOffsetDay =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Day);

    public static readonly MemberInfo DateTimeOffsetMonth =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Month);

    public static readonly MemberInfo DateTimeOffsetYear =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Year);

    public static readonly MemberInfo DateTimeOffsetHour =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Hour);

    public static readonly MemberInfo DateTimeOffsetMinute =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Minute);

    public static readonly MemberInfo DateTimeOffsetSecond =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Second);

    public static readonly MemberInfo DateTimeOffsetMilliseconds =
        ReflectionHelper.GetMember<DateTimeOffset, int>(x => x.Millisecond);

    public static readonly MemberInfo DateTimeDate =
        ReflectionHelper.GetMember<DateTime, DateTime>(x => x.Date);

    public static readonly MemberInfo DateTimeTime =
        ReflectionHelper.GetMember<DateTime, TimeSpan>(x => x.TimeOfDay);

    public static readonly MemberInfo DateTimeDay =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Day);

    public static readonly MemberInfo DateTimeMonth =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Month);

    public static readonly MemberInfo DateTimeYear =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Year);

    public static readonly MemberInfo DateTimeHour =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Hour);

    public static readonly MemberInfo DateTimeMinute =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Minute);

    public static readonly MemberInfo DateTimeSecond =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Second);

    public static readonly MemberInfo DateTimeMilliseconds =
        ReflectionHelper.GetMember<DateTime, int>(x => x.Millisecond);
    
    public static readonly MemberInfo DateOnlyDay =
        ReflectionHelper.GetMember<DateOnly, int>(x => x.Day);

    public static readonly MemberInfo DateOnlyMonth =
        ReflectionHelper.GetMember<DateOnly, int>(x => x.Month);

    public static readonly MemberInfo DateOnlyYear =
        ReflectionHelper.GetMember<DateOnly, int>(x => x.Year);

    public static readonly MemberInfo NullableDateTimeHasValue =
        ReflectionHelper.GetMember<DateTime?, bool>(x => x.HasValue);

    public static readonly MemberInfo NullableDateTimeValue =
        ReflectionHelper.GetMember<DateTime?, DateTime>(x => x.Value);

    public static readonly MemberInfo TimeSpanTotalMinutes =
        ReflectionHelper.GetMember<TimeSpan, double>(x => x.TotalMinutes);

    public static readonly MethodInfo MathCeiling =
        ReflectionHelper.GetMethod(() => Math.Ceiling((double)1));

    public static readonly MethodInfo MathFloor =
        ReflectionHelper.GetMethod(() => Math.Floor((double)1));

    public static readonly MethodInfo MathRound =
        ReflectionHelper.GetMethod(() => Math.Round((double)1));

    public static readonly MethodInfo ObjectToString =
        ReflectionHelper.GetMethod<object, string>(o => o.ToString());

    public static readonly MethodInfo DateOnlyFromDateTime =
        ReflectionHelper.GetMethod(() => DateOnly.FromDateTime(DateTime.MinValue));

    public static readonly MethodInfo HasFlag =
        typeof(Enum).GetMethod(nameof(Enum.HasFlag), BindingFlags.Static | BindingFlags.Public);

    public static readonly MethodInfo QueryableOrderBy =
        ReflectionHelper.GetMethod<IQueryable<object>, IQueryable<object>>(x => x.OrderBy(y => y == null));

    public static readonly MethodInfo QueryableOrderByDescending =
        ReflectionHelper.GetMethod<IQueryable<object>, IQueryable<object>>(x =>
            x.OrderByDescending(y => y == null));

    public static readonly MethodInfo QueryableThenBy =
        ReflectionHelper.GetMethod<IOrderedQueryable<object>, IQueryable<object>>(x => x.ThenBy(y => y == null));

    public static readonly MethodInfo QueryableThenByDescending =
        ReflectionHelper.GetMethod<IOrderedQueryable<object>, IQueryable<object>>(x =>
            x.ThenByDescending(y => y == null));

    public static readonly MethodInfo EnumerableOrderBy =
        ReflectionHelper.GetMethod<IEnumerable<object>, IEnumerable<object>>(x => x.OrderBy(y => y == null));
}
