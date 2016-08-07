// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Data;

namespace Avalonia.Markup.Data.Plugins
{
    /// <summary>
    /// A <see cref="IPropertyAccessor"/> that wraps another <see cref="IPropertyAccessor"/>.
    /// </summary>
    public abstract class PropertyAccessorWrapper : IPropertyAccessor
    {
        private readonly IPropertyAccessor _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessorWrapper"/> class.
        /// </summary>
        /// <param name="inner">The inner property accessor.</param>
        protected PropertyAccessorWrapper(IPropertyAccessor inner)
        {
            _inner = inner;
        }

        /// <inheritdoc/>
        public Type PropertyType => _inner.PropertyType;

        /// <inheritdoc/>
        public object Value => _inner.Value;

        /// <inheritdoc/>
        public virtual void Dispose() => _inner.Dispose();

        /// <inheritdoc/>
        public virtual bool SetValue(object value, BindingPriority priority) => _inner.SetValue(value, priority);
    }
}