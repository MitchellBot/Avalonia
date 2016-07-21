// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Subjects;

namespace Avalonia.Data
{
    public enum BindingSourceType
    {
        Value,
        Observable,
        Subject,
        NotificationObservable,
        NotificationSubject,
    }

    /// <summary>
    /// Holds the result of calling <see cref="IBinding.Initiate"/>.
    /// </summary>
    /// <remarks>
    /// Whereas an <see cref="IBinding"/> holds a description of a binding such as "Bind to the X
    /// property on a control's DataContext"; this class represents a binding that has been 
    /// *instanced* by calling <see cref="IBinding.Initiate(IAvaloniaObject, AvaloniaProperty, object)"/>
    /// on a target object.
    /// 
    /// When a binding is initiated, it can return one of 3 possible sources for the binding:
    /// - An <see cref="ISubject{Object}"/> which can be used for any type of binding.
    /// - An <see cref="IObservable{Object}"/> which can be used for all types of bindings except
    ///  <see cref="BindingMode.OneWayToSource"/> and <see cref="BindingMode.TwoWay"/>.
    /// - A plain object, which can only represent a <see cref="BindingMode.OneTime"/> binding.
    /// </remarks>
    public class InstancedBinding
    {
        public static InstancedBinding FromValue(
            object value,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            return new InstancedBinding(BindingSourceType.Value, value, BindingMode.OneTime, priority);
        }

        public static InstancedBinding FromObservable(
            IObservable<object> source,
            BindingMode mode = BindingMode.OneWay,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            return new InstancedBinding(BindingSourceType.Observable, source, mode, priority);
        }

        public static InstancedBinding FromObservable(
            IObservable<BindingNotification> source,
            BindingMode mode = BindingMode.OneWay,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            return new InstancedBinding(BindingSourceType.NotificationObservable, source, mode, priority);
        }

        public static InstancedBinding FromSubject(
            ISubject<object> source,
            BindingMode mode = BindingMode.OneWay,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            return new InstancedBinding(BindingSourceType.Subject, source, mode, priority);
        }

        public static InstancedBinding FromSubject(
            ISubject<BindingNotification> source,
            BindingMode mode = BindingMode.OneWay,
            BindingPriority priority = BindingPriority.LocalValue)
        {
            return new InstancedBinding(BindingSourceType.NotificationSubject, source, mode, priority);
        }

        protected InstancedBinding(
            BindingSourceType sourceType,
            object value,
            BindingMode mode,
            BindingPriority priority)
        {
            SourceType = sourceType;
            Value = value;
            Mode = mode;
            Priority = priority;
        }

        public BindingSourceType SourceType { get; }

        /// <summary>
        /// Gets the binding mode with which the binding was initiated.
        /// </summary>
        public BindingMode Mode { get; }

        /// <summary>
        /// Gets the binding priority.
        /// </summary>
        public BindingPriority Priority { get; }

        public object Value { get; }

        public IObservable<object> Observable => Value as IObservable<object>;

        /// <summary>
        /// Gets the observable for a one-way binding.
        /// </summary>
        public IObservable<BindingNotification> NotificationObservable => Value as IObservable<BindingNotification>;

        /// <summary>
        /// Gets the subject for a two-way binding.
        /// </summary>
        public ISubject<object> Subject => Value as ISubject<object>;

        public ISubject<BindingNotification> NotificationSubject => Value as ISubject<BindingNotification>;
    }
}
