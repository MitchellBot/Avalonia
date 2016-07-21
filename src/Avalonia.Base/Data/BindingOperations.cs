// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Avalonia.Data
{
    public static class BindingOperations
    {
        /// <summary>
        /// Applies an <see cref="InstancedBinding"/> a property on an <see cref="IAvaloniaObject"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="property">The property to bind.</param>
        /// <param name="binding">The instanced binding.</param>
        /// <param name="anchor">
        /// An optional anchor from which to locate required context. When binding to objects that
        /// are not in the logical tree, certain types of binding need an anchor into the tree in 
        /// order to locate named controls or resources. The <paramref name="anchor"/> parameter 
        /// can be used to provice this context.
        /// </param>
        /// <returns>An <see cref="IDisposable"/> which can be used to cancel the binding.</returns>
        public static IDisposable Apply(
            IAvaloniaObject target,
            AvaloniaProperty property,
            InstancedBinding binding,
            object anchor)
        {
            Contract.Requires<ArgumentNullException>(target != null);
            Contract.Requires<ArgumentNullException>(property != null);
            Contract.Requires<ArgumentNullException>(binding != null);

            var mode = binding.Mode;
            var notifications =
                binding.SourceType == BindingSourceType.NotificationObservable ||
                binding.SourceType == BindingSourceType.NotificationSubject;

            if (mode == BindingMode.Default)
            {
                mode = property.GetMetadata(target.GetType()).DefaultBindingMode;
            }

            switch (mode)
            {
                case BindingMode.Default:
                case BindingMode.OneWay:
                    if (!notifications)
                    {
                        return target.Bind(property, binding.Observable, binding.Priority);
                    }
                    else
                    {
                        return target.Bind(property, binding.NotificationObservable, binding.Priority);
                    }

                case BindingMode.TwoWay:
                    if (!notifications)
                    {
                        return new CompositeDisposable(
                            target.Bind(property, binding.Subject, binding.Priority),
                            target.GetObservable(property).Subscribe(binding.Subject));
                    }
                    else
                    {
                        var a = target.Bind(property, binding.NotificationSubject, binding.Priority);
                        var b = target.GetObservable(property)
                                  .Select(x => new BindingNotification(x))
                                  .Subscribe(binding.NotificationSubject);
                        return new CompositeDisposable(
                            target.Bind(property, binding.NotificationSubject, binding.Priority),
                            target.GetObservable(property)
                                  .Select(x => new BindingNotification(x))
                                  .Subscribe(binding.NotificationSubject));
                    }

                case BindingMode.OneTime:
                    if (binding.NotificationObservable != null)
                    {
                        return binding.NotificationObservable
                            .Where(x => x.ErrorType == BindingErrorType.None)
                            .Take(1)
                            .Subscribe(x => target.SetValue(property, x.Value, binding.Priority));
                    }
                    else if (binding.Observable != null)
                    {
                        return binding.Observable
                            .Take(1)
                            .Subscribe(x => target.SetValue(property, x, binding.Priority));
                    }
                    else
                    {
                        target.SetValue(property, binding.Value, binding.Priority);
                        return Disposable.Empty;
                    }
                case BindingMode.OneWayToSource:
                    if (!notifications)
                    {
                        return target.GetObservable(property).Subscribe(binding.Subject);
                    }
                    else
                    {
                        return target.GetObservable(property)
                            .Select(x => new BindingNotification(x))
                            .Subscribe(binding.NotificationSubject);
                    }

                default:
                    throw new ArgumentException("Invalid binding mode.");
            }
        }
    }
}
