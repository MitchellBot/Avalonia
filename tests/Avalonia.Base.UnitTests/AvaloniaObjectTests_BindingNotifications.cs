// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.UnitTests;
using Moq;
using Xunit;

namespace Avalonia.Base.UnitTests
{
    public class AvaloniaObjectTests_BindingNotifications
    {
        [Fact]
        public void Sets_Current_Value()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification("initial"));

            target.Bind(Class1.Styledroperty, source);

            Assert.Equal("initial", target.GetValue(Class1.Styledroperty));
        }

        [Fact]
        public void Sets_Current_Value_Direct()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification("initial"));

            target.Bind(Class1.DirectProperty, source);

            Assert.Equal("initial", target.GetValue(Class1.DirectProperty));
        }

        [Fact]
        public void Resets_Value_On_UnsetValue()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification("initial"));

            target.Bind(Class1.Styledroperty, source);
            source.OnNext(new BindingNotification(AvaloniaProperty.UnsetValue));

            Assert.Equal("default", target.GetValue(Class1.Styledroperty));
        }

        [Fact]
        public void Resets_Value_On_UnsetValue_Direct()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification("initial"));

            target.Bind(Class1.DirectProperty, source);
            source.OnNext(new BindingNotification(AvaloniaProperty.UnsetValue));

            Assert.Equal("default", target.GetValue(Class1.DirectProperty));
        }

        [Fact]
        public void Doesnt_Set_Value_When_HasValue_False()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.Error));

            target.Bind(Class1.Styledroperty, source);

            Assert.Equal("default", target.GetValue(Class1.Styledroperty));
        }

        [Fact]
        public void Doesnt_Set_Value_When_HasValue_False_Direct()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.Error));

            target.Bind(Class1.DirectProperty, source);

            Assert.Equal("default", target.GetValue(Class1.DirectProperty));
        }

        [Fact]
        public void Doesnt_Set_Value_When_Value_Of_Invalid_Type()
        {
            var sink = new Mock<ILogSink>();

            Logger.Sink = sink.Object;

            using (Disposable.Create(() => Logger.Sink = null))
            {
                var target = new Class1();
                var source = new BehaviorSubject<BindingNotification>(
                    new BindingNotification(4));

                target.Bind(Class1.Styledroperty, source);

                Assert.Equal("default", target.GetValue(Class1.Styledroperty));

                sink.Verify(x => x.Log(
                    LogEventLevel.Error,
                    LogArea.Binding,
                    target,
                    "Binding produced invalid value for {$Property} ({$PropertyType}): {$Value} ({$ValueType})",
                    new object[] { Class1.Styledroperty, typeof(string), 4, typeof(int) }));
            }
        }

        [Fact]
        public void Doesnt_Set_Value_When_Value_Of_Invalid_Type_Direct()
        {
            var sink = new Mock<ILogSink>();

            Logger.Sink = sink.Object;

            using (Disposable.Create(() => Logger.Sink = null))
            {
                var target = new Class1();
                var source = new BehaviorSubject<BindingNotification>(
                    new BindingNotification(4));

                target.Bind(Class1.DirectProperty, source);

                Assert.Equal("default", target.GetValue(Class1.DirectProperty));

                sink.Verify(x => x.Log(
                    LogEventLevel.Error,
                    LogArea.Binding,
                    target,
                    "Binding produced invalid value for {$Property} ({$PropertyType}): {$Value} ({$ValueType})",
                    new object[] { Class1.DirectProperty, typeof(string), 4, typeof(int) }));
            }
        }

        [Fact]
        public void Notification_With_Error_Uses_Fallback_Value()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.Error, "fallback"));

            target.Bind(Class1.Styledroperty, source);

            Assert.Equal("fallback", target.GetValue(Class1.Styledroperty));
        }

        [Fact]
        public void Notification_With_Error_Uses_Fallback_Value_Direct()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.Error, "fallback"));

            target.Bind(Class1.DirectProperty, source);

            Assert.Equal("fallback", target.GetValue(Class1.DirectProperty));
        }

        [Fact]
        public void Invalid_Fallback_Value_Is_Ignored()
        {
            var sink = new Mock<ILogSink>();

            Logger.Sink = sink.Object;

            using (Disposable.Create(() => Logger.Sink = null))
            {
                var target = new Class1();
                var source = new BehaviorSubject<BindingNotification>(
                    new BindingNotification(new Exception(), BindingErrorType.Error, 5));

                target.Bind(Class1.Styledroperty, source);

                Assert.Equal("default", target.GetValue(Class1.Styledroperty));

                sink.Verify(x => x.Log(
                    LogEventLevel.Error,
                    LogArea.Binding,
                    target,
                    "Binding produced invalid value for {$Property} ({$PropertyType}): {$Value} ({$ValueType})",
                    new object[] { Class1.Styledroperty, typeof(string), 5, typeof(int) }));
            }
        }

        [Fact]
        public void Invalid_Fallback_Value_Is_Ignored_Direct()
        {
            var sink = new Mock<ILogSink>();

            Logger.Sink = sink.Object;

            using (Disposable.Create(() => Logger.Sink = null))
            {
                var target = new Class1();
                var source = new BehaviorSubject<BindingNotification>(
                    new BindingNotification(new Exception(), BindingErrorType.Error, 5));

                target.Bind(Class1.DirectProperty, source);

                Assert.Equal("default", target.GetValue(Class1.DirectProperty));

                sink.Verify(x => x.Log(
                    LogEventLevel.Error,
                    LogArea.Binding,
                    target,
                    "Binding produced invalid value for {$Property} ({$PropertyType}): {$Value} ({$ValueType})",
                    new object[] { Class1.DirectProperty, typeof(string), 5, typeof(int) }));
            }
        }

        [Fact]
        public void Notification_With_ValidationError_Uses_Fallback_Value()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.DataValidationError, "fallback"));

            target.Bind(Class1.Styledroperty, source);

            Assert.Equal("fallback", target.GetValue(Class1.Styledroperty));
        }

        [Fact]
        public void Notification_With_ValidationError_Uses_Fallback_Value_Direct()
        {
            var target = new Class1();
            var source = new BehaviorSubject<BindingNotification>(
                new BindingNotification(new Exception(), BindingErrorType.DataValidationError, "fallback"));

            target.Bind(Class1.DirectProperty, source);

            Assert.Equal("fallback", target.GetValue(Class1.DirectProperty));
        }

        [Theory]
        [InlineData(BindingErrorType.Error, true)]
        [InlineData(BindingErrorType.DataValidationError, false)]
        public void Notification_With_Exception_Logs_BindingError_But_Not_For_Validation_Errors(
            BindingErrorType errorType, 
            bool expected)
        {
            var target = new Class1();
            var source = new Subject<BindingNotification>();
            var called = false;
            var expectedMessageTemplate = "Error binding to {Target}.{Property}: {Message}";

            LogCallback checkLogMessage = (level, area, src, mt, pv) =>
            {
                if (level == LogEventLevel.Error &&
                    area == LogArea.Binding &&
                    mt == expectedMessageTemplate)
                {
                    called = true;
                }
            };

            using (TestLogSink.Start(checkLogMessage))
            {
                target.Bind(Class1.Styledroperty, source);
                source.OnNext(new BindingNotification("initial"));
                source.OnNext(new BindingNotification(
                    new InvalidOperationException("Foo"), 
                    errorType));

                Assert.Equal("initial", target.GetValue(Class1.Styledroperty));
                Assert.Equal(expected, called);
            }
        }

        [Fact]
        public void Notification_BindingError_With_Exception_Logs()
        {
            var target = new Class1();
            var source = new Subject<BindingNotification>();
            var sink = new Mock<ILogSink>();

            Logger.Sink = sink.Object;

            using (Disposable.Create(() => Logger.Sink = null))
            {
                target.Bind(Class1.DirectProperty, source);
                source.OnNext(new BindingNotification("initial"));
                source.OnNext(new BindingNotification(
                    new InvalidOperationException("Pity"),
                    BindingErrorType.Error));

                Assert.Equal("initial", target.GetValue(Class1.DirectProperty));

                sink.Verify(x => x.Log(
                    LogEventLevel.Error,
                    LogArea.Binding,
                    target,
                    "Error binding to {Target}.{Property}: {Message}",
                    new object[] { target, Class1.DirectProperty, "Pity" }));
            }
        }

        private class Class1 : AvaloniaObject
        {
            public static readonly StyledProperty<string> Styledroperty =
                AvaloniaProperty.Register<Class1, string>("Foo", "default");

            public static readonly DirectProperty<Class1, string> DirectProperty =
                AvaloniaProperty.RegisterDirect<Class1, string>(
                    "Bar",
                    o => o.Direct,
                    (o, v) => o.Direct = v,
                    unsetValue: "default");

            string _direct = "default";

            public string Direct
            {
                get { return _direct; }
                set { SetAndRaise(DirectProperty, ref _direct, value); }
            }

            public string ValidationError
            {
                get;
                private set;
            }
        }
    }
}
