// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class IEnumerableWithAddMethodConverter<TCollection>
        : IEnumerableDefaultConverter<TCollection, object?>
        where TCollection : IEnumerable
    {
        protected override void Add(in object? value, ref ReadStack state)
        {
            ((Action<TCollection, object?>)state.Current.AddMethodDelegate!)((TCollection)state.Current.ReturnValue!, value);
        }

        protected override void CreateCollection(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options)
        {
            JsonClassInfo.ConstructorDelegate? constructorDelegate = state.Current.JsonClassInfo.CreateObject;

            if (constructorDelegate == null)
            {
                ThrowHelper.ThrowNotSupportedException_CannotPopulateCollection(TypeToConvert, ref reader, ref state);
            }

            state.Current.ReturnValue = constructorDelegate();
            state.Current.AddMethodDelegate = GetAddMethodDelegate(options);
        }

        protected override bool OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, ref WriteStack state)
        {
            IEnumerator enumerator;
            if (state.Current.CollectionEnumerator == null)
            {
                enumerator = value.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    return true;
                }
            }
            else
            {
                enumerator = state.Current.CollectionEnumerator;
            }

            JsonConverter<object?> converter = GetElementConverter(ref state);
            do
            {
                if (ShouldFlush(writer, ref state))
                {
                    state.Current.CollectionEnumerator = enumerator;
                    return false;
                }

                object? element = enumerator.Current;
                if (!converter.TryWrite(writer, element, options, ref state))
                {
                    state.Current.CollectionEnumerator = enumerator;
                    return false;
                }
            } while (enumerator.MoveNext());

            return true;
        }

        private Action<TCollection, object?>? _addMethodDelegate;

        internal Action<TCollection, object?> GetAddMethodDelegate(JsonSerializerOptions options)
        {
            if (_addMethodDelegate == null)
            {
                // We verified this exists when we created the converter in the enumerable converter factory.
                _addMethodDelegate = options.MemberAccessorStrategy.CreateAddMethodDelegate<TCollection>();
            }

            return _addMethodDelegate;
        }
    }
}
