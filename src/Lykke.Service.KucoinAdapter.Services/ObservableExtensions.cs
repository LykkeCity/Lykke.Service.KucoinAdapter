using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Lykke.Service.KucoinAdapter.Services
{
    public static class ObservableExtensions
    {
        private static IEnumerable<IObservable<T>> Generator<T>(IObservable<T> source, TimeSpan min, TimeSpan max)
        {
            TimeSpan? delay = null;

            while (true)
            {
                var delayed =  delay == null? source : source.DelaySubscription(delay.Value);

                var result = delayed.Do(
                    _ => delay = null,
                    err => IncreaseDelay(min, max, ref delay));

                yield return result
                    .Select(x => (true, x))
                    .Catch((Exception ex) => Observable.Return((false, default(T))))
                    .Where(x => x.Item1)
                    .Select(x => x.Item2);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static void IncreaseDelay(TimeSpan min, TimeSpan max, ref TimeSpan? delay)
        {
            if (delay == null)
            {
                delay = min;
            }
            else
            {
                var nextMs = delay.Value.Milliseconds * 2;
                delay = TimeSpan.FromMilliseconds(Math.Min(nextMs, max.TotalMilliseconds));
            }
        }

        public static IObservable<T> RetryWithBackoff<T>(
            this IObservable<T> source,
            RetryTimeouts timeouts = null)
        {
            timeouts = timeouts ?? new RetryTimeouts(TimeSpan.FromMilliseconds(50), TimeSpan.FromMinutes(5));
            return Generator(source, timeouts.Min, timeouts.Max).Concat();
        }
    }

    public sealed class RetryTimeouts
    {
        public RetryTimeouts(TimeSpan min, TimeSpan max)
        {
            Min = min;
            Max = max;
        }

        public TimeSpan Min { get; }
        public TimeSpan Max { get; }
    }
}
