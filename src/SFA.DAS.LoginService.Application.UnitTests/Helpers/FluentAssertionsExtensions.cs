using FluentAssertions;
using FluentAssertions.Collections;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentAssertions
{
    public static class FluentAssertionsExtensions
    {
        public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this Dictionary<TKey, TValue> dict) =>
            new GenericDictionaryAssertions<TKey, TValue>(dict);

        public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict) =>
            dict is IDictionary<TKey, TValue> regularDictionary
                ? new GenericDictionaryAssertions<TKey, TValue>(regularDictionary)
                : throw new ArgumentException("Wait for FA 6.0");
    }
}