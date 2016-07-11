// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.Utilities;

namespace Avalonia.Markup.Data
{
    /// <summary>
    /// Turns an <see cref="ExpressionObserver"/> into a subject that can be bound two-way with
    /// a value converter.
    /// </summary>
    public class ExpressionSubject : ISubject<BindingNotification>, IDescription
    {
        private readonly ExpressionObserver _inner;
        private readonly Type _targetType;
        private readonly object _fallbackValue;
        private readonly BindingPriority _priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionObserver"/> class.
        /// </summary>
        /// <param name="inner">The <see cref="ExpressionObserver"/>.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        public ExpressionSubject(ExpressionObserver inner, Type targetType)
            : this(inner, targetType, DefaultValueConverter.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionObserver"/> class.
        /// </summary>
        /// <param name="inner">The <see cref="ExpressionObserver"/>.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="converter">The value converter to use.</param>
        /// <param name="converterParameter">
        /// A parameter to pass to <paramref name="converter"/>.
        /// </param>
        /// <param name="priority">The binding priority.</param>
        public ExpressionSubject(
            ExpressionObserver inner,
            Type targetType,
            IValueConverter converter,
            object converterParameter = null,
            BindingPriority priority = BindingPriority.LocalValue)
            : this(inner, targetType, AvaloniaProperty.UnsetValue, converter, converterParameter, priority)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionObserver"/> class.
        /// </summary>
        /// <param name="inner">The <see cref="ExpressionObserver"/>.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="fallbackValue">
        /// The value to use when the binding is unable to produce a value.
        /// </param>
        /// <param name="converter">The value converter to use.</param>
        /// <param name="converterParameter">
        /// A parameter to pass to <paramref name="converter"/>.
        /// </param>
        /// <param name="priority">The binding priority.</param>
        public ExpressionSubject(
            ExpressionObserver inner, 
            Type targetType,
            object fallbackValue,
            IValueConverter converter,
            object converterParameter = null,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            Contract.Requires<ArgumentNullException>(inner != null);
            Contract.Requires<ArgumentNullException>(targetType != null);
            Contract.Requires<ArgumentNullException>(converter != null);

            _inner = inner;
            _targetType = targetType;
            Converter = converter;
            ConverterParameter = converterParameter;
            _fallbackValue = fallbackValue;
            _priority = priority;
        }

        /// <summary>
        /// Gets the converter to use on the expression.
        /// </summary>
        public IValueConverter Converter { get; }

        /// <summary>
        /// Gets a parameter to pass to <see cref="Converter"/>.
        /// </summary>
        public object ConverterParameter { get; }

        /// <inheritdoc/>
        string IDescription.Description => _inner.Expression;

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        public void OnNext(object value)
        {
            OnNext(new BindingNotification(value));
        }

        /// <inheritdoc/>
        public void OnNext(BindingNotification notification)
        {
            Contract.Requires<ArgumentNullException>(notification != null);

            var type = _inner.ResultType;

            if (type != null && notification.HasValue)
            {
                var converted = Converter.ConvertBack(
                    notification.Value, 
                    type, 
                    ConverterParameter, 
                    CultureInfo.CurrentUICulture);

                if (converted == BindingNotification.UnsetValue)
                {
                    _inner.SetValue(TypeUtilities.Default(type), _priority);
                }
                else if (converted == null || converted.ErrorType == BindingErrorType.Error)
                {
                    Logger.Error(
                        LogArea.Binding,
                        this,
                        "Error binding to {Expression}: {Message}",
                        _inner.Expression,
                        converted?.Error.Message);

                    object fallback;

                    if (_fallbackValue != AvaloniaProperty.UnsetValue)
                    {
                        if (TypeUtilities.TryConvert(
                            type,
                            _fallbackValue,
                            CultureInfo.InvariantCulture,
                            out fallback))
                        {
                            _inner.SetValue(fallback, _priority);
                        }
                        else
                        {
                            Logger.Error(
                                LogArea.Binding,
                                this,
                                "Could not convert FallbackValue {FallbackValue} to {Type}",
                                _fallbackValue,
                                type);
                        }
                    }
                }
                else
                {
                    _inner.SetValue(converted.Value, _priority);
                }
            }
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<BindingNotification> observer)
        {
            return _inner.Select(ConvertValue).Subscribe(observer);
        }

        private BindingNotification ConvertValue(BindingNotification notification)
        {
            if (notification.HasValue)
            {
                var converted = Converter.Convert(
                    notification.Value,
                    _targetType,
                    ConverterParameter,
                    CultureInfo.CurrentUICulture);

                if (converted == null ||
                    (converted.ErrorType != BindingErrorType.None &&
                     _fallbackValue != AvaloniaProperty.UnsetValue))
                {
                    converted = Converter.Convert(
                        _fallbackValue,
                        _targetType,
                        null,
                        CultureInfo.CurrentUICulture);
                }

                return converted?.WithError(notification.Error) ?? notification;
            }
            else
            {
                return notification;
            }
        }
    }
}
