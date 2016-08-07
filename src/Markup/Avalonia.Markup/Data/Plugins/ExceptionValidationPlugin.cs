// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Data;
using System;
using System.Reflection;

namespace Avalonia.Markup.Data.Plugins
{
    /// <summary>
    /// Validates properties that report errors by throwing exceptions.
    /// </summary>
    public class ExceptionValidationPlugin : IDataValidationPlugin
    {
        public static ExceptionValidationPlugin Instance { get; } = new ExceptionValidationPlugin();

        /// <inheritdoc/>
        public bool Match(WeakReference reference) => true;

        /// <inheritdoc/>
        public IPropertyAccessor Start(WeakReference reference, string name, IPropertyAccessor accessor, Action<BindingNotification> callback)
        {
            return new ExceptionValidationChecker(reference, name, accessor, callback);
        }

        private class ExceptionValidationChecker : PropertyAccessorWrapper
        {
            public ExceptionValidationChecker(WeakReference reference, string name, IPropertyAccessor accessor, Action<BindingNotification> callback)
                : base(reference, name, accessor, callback)
            {
            }

            public override bool SetValue(object value, BindingPriority priority)
            {
                try
                {
                    var success = base.SetValue(value, priority);
                    ////SendValidationCallback(new ExceptionValidationStatus(null));
                    return success;
                }
                catch (TargetInvocationException ex)
                {
                    ////SendValidationCallback(new ExceptionValidationStatus(ex.InnerException));
                }
                catch (Exception ex)
                {
                    ////SendValidationCallback(new ExceptionValidationStatus(ex));
                }
                return false;
            }
        }
    }
}
