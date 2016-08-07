// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Data;
using Avalonia.Markup.Data.Plugins;
using Xunit;

namespace Avalonia.Markup.UnitTests.Data
{
    public class IndeiValidatorTests
    {
        public class Data : INotifyPropertyChanged, INotifyDataErrorInfo
        {
            private int nonValidated;

            public int NonValidated
            {
                get { return nonValidated; }
                set { nonValidated = value; NotifyPropertyChanged(); }
            }

            private int mustBePositive;

            public int MustBePositive
            {
                get { return mustBePositive; }
                set
                {
                    mustBePositive = value;
                    NotifyErrorsChanged();
                }
            }

            public bool HasErrors
            {
                get
                {
                    return MustBePositive > 0;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private void NotifyErrorsChanged([CallerMemberName] string propertyName = "")
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            public IEnumerable GetErrors(string propertyName)
            {
                if (propertyName == nameof(MustBePositive) && MustBePositive <= 0)
                {
                    yield return $"{nameof(MustBePositive)} must be positive";
                }
            }
        }

        [Fact]
        public void Setting_Non_Validating_Does_Not_Trigger_Validation()
        {
            Assert.True(false);
            ////var inpcAccessorPlugin = new InpcPropertyAccessorPlugin();
            ////var validatorPlugin = new IndeiValidationPlugin();
            ////var data = new Data();
            ////var accessor = inpcAccessorPlugin.Start(new WeakReference(data), nameof(data.NonValidated), _ => { });
            ////IValidationStatus status = null;
            ////var validator = validatorPlugin.Start(new WeakReference(data), nameof(data.NonValidated), accessor, s => status = s);

            ////validator.SetValue(5, BindingPriority.LocalValue);

            ////Assert.Null(status);
        }

        [Fact]
        public void Setting_Validating_Property_To_Valid_Value_Returns_Successful_ValidationStatus()
        {
            Assert.True(false);
            ////var inpcAccessorPlugin = new InpcPropertyAccessorPlugin();
            ////var validatorPlugin = new IndeiValidationPlugin();
            ////var data = new Data();
            ////var accessor = inpcAccessorPlugin.Start(new WeakReference(data), nameof(data.MustBePositive), _ => { });
            ////IValidationStatus status = null;
            ////var validator = validatorPlugin.Start(new WeakReference(data), nameof(data.MustBePositive), accessor, s => status = s);

            ////validator.SetValue(5, BindingPriority.LocalValue);

            ////Assert.True(status.IsValid);
        }
        
        [Fact]
        public void Setting_Validating_Property_To_Invalid_Value_Returns_Failed_ValidationStatus()
        {
            Assert.True(false);
            ////var inpcAccessorPlugin = new InpcPropertyAccessorPlugin();
            ////var validatorPlugin = new IndeiValidationPlugin();
            ////var data = new Data();
            ////var accessor = inpcAccessorPlugin.Start(new WeakReference(data), nameof(data.MustBePositive), _ => { });
            ////IValidationStatus status = null;
            ////var validator = validatorPlugin.Start(new WeakReference(data), nameof(data.MustBePositive), accessor, s => status = s);

            ////validator.SetValue(-5, BindingPriority.LocalValue);

            ////Assert.False(status.IsValid);
        }
    }
}
