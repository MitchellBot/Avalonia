// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Avalonia.Data;
using Avalonia.Utilities;

namespace Avalonia.Markup.Data.Plugins
{
    /// <summary>
    /// Validates properties on objects that implement <see cref="INotifyDataErrorInfo"/>.
    /// </summary>
    public class IndeiValidationPlugin : IDataValidationPlugin
    {
        /// <inheritdoc/>
        public bool Match(object o) => o is INotifyDataErrorInfo;

        /// <inheritdoc/>
        public IPropertyAccessor Start(WeakReference reference, string name, IPropertyAccessor accessor)
        {
            return new IndeiValidationChecker(reference, name, accessor);
        }

        private class IndeiValidationChecker : PropertyAccessorWrapper, IWeakSubscriber<DataErrorsChangedEventArgs>
        {
            WeakReference _reference;

            public IndeiValidationChecker(WeakReference reference, string name, IPropertyAccessor inner)
                : base(inner)
            {
                _reference = reference;

                var target = reference.Target as INotifyDataErrorInfo;

                if (target != null)
                {
                    if (target.HasErrors)
                    {
                        ////SendValidationCallback(new IndeiValidationStatus(target.GetErrors(name)));
                    }

                    WeakSubscriptionManager.Subscribe(
                        target,
                        nameof(target.ErrorsChanged),
                        this);
                }
            }

            public override void Dispose()
            {
                base.Dispose();
                var target = _reference.Target as INotifyDataErrorInfo;
                if (target != null)
                {
                    WeakSubscriptionManager.Unsubscribe(
                        target,
                        nameof(target.ErrorsChanged),
                        this);
                }
            }

            public void OnEvent(object sender, DataErrorsChangedEventArgs e)
            {
                if (e.PropertyName == _name || string.IsNullOrEmpty(e.PropertyName))
                {
                    var indei = _reference.Target as INotifyDataErrorInfo;
                    ////SendValidationCallback(new IndeiValidationStatus(indei.GetErrors(e.PropertyName)));
                }
            }
        }
    }
}
