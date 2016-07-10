// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;
using System.Reactive.Linq;
using Avalonia.Data;

namespace Avalonia.Markup.Data
{
    internal class LogicalNotNode : ExpressionNode
    {
        public override bool SetValue(object value, BindingPriority priority)
        {
            return false;
        }

        public override IDisposable Subscribe(IObserver<BindingNotification> observer)
        {
            return Next.Select(Negate).Subscribe(observer);
        }

        private static BindingNotification Negate(BindingNotification notification)
        {
            if (notification.HasValue)
            {
                try
                {
                    var s = notification.Value as string;
                    var boolean = s != null ?
                        bool.Parse(s) :
                        Convert.ToBoolean(notification.Value, CultureInfo.InvariantCulture);
                    return new BindingNotification(!boolean);
                }
                catch (Exception e)
                {
                    return new BindingNotification(e, BindingErrorType.Error);
                }
            }

            return notification;
        }
    }
}
