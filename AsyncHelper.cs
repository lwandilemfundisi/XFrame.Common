using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace XFrame.Common
{
    using EventQueue = ConcurrentQueue<Tuple<SendOrPostCallback, object>>;
    using EventTask = Tuple<SendOrPostCallback, object>;

    public static class AsyncHelper
    {
        public class AsyncBridge : IDisposable
        {
            private readonly ExclusiveSynchronizationContext _currentContext;
            private readonly SynchronizationContext _oldContext;
            private int _taskCount;
            private bool _hasAsyncTasks;

            public AsyncBridge()
            {
                _oldContext = SynchronizationContext.Current;
                _currentContext = new ExclusiveSynchronizationContext(_oldContext);
                SynchronizationContext.SetSynchronizationContext(_currentContext);
            }

            public void Run(Task task, Action<Task> callback = null)
            {
                _hasAsyncTasks = true;
                _currentContext.Post(
                    async _ =>
                    {
                        try
                        {
                            Increment();
                            await task.ConfigureAwait(true);
                            callback?.Invoke(task);
                        }
                        catch (Exception e)
                        {
                            _currentContext.InnerException = e;
                        }
                        finally
                        {
                            Decrement();
                        }
                    },
                    null);
            }

            public void Run<T>(Task<T> task, Action<Task<T>> callback = null)
            {
                if (null != callback)
                {
                    Run((Task)task, (finishedTask) => callback((Task<T>)finishedTask));
                }
                else
                {
                    Run((Task)task);
                }
            }

            public void Run<T>(Task<T> task, Action<T> callback)
            {
                Run(task, (t) => callback(t.Result));
            }

            private void Increment()
            {
                Interlocked.Increment(ref _taskCount);
            }

            private void Decrement()
            {
                Interlocked.Decrement(ref _taskCount);
                if (_taskCount == 0)
                {
                    _currentContext.EndMessageLoop();
                }
            }

            public void Dispose()
            {
                try
                {
                    if (_hasAsyncTasks)
                    {
                        _currentContext.BeginMessageLoop();
                    }
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(_oldContext);
                }
            }
        }

        public static AsyncBridge Wait => new AsyncBridge();

        public static void FireAndForget(
            Func<Task> task,
            Action<Exception> handle = null)
        {
            Task.Run(() =>
            {
                ((Func<Task>)(async () =>
                {
                    try
                    {
                        await task().ConfigureAwait(true);
                    }
                    catch (Exception e)
                    {
                        handle?.Invoke(e);
                    }
                }))();
            });
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
            private bool _done;
            private readonly EventQueue _items;

            public Exception InnerException { private get; set; }

            public ExclusiveSynchronizationContext(SynchronizationContext old)
            {
                var oldEx = old as ExclusiveSynchronizationContext;
                _items = null != oldEx ? oldEx._items : new EventQueue();
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _items.Enqueue(Tuple.Create(d, state));
                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!_done)
                {
                    EventTask task;

                    if (!_items.TryDequeue(out task))
                    {
                        task = null;
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null)
                        {
                            throw new AggregateException(
                                "AsyncBridge.Run method threw an exception.",
                                InnerException);
                        }
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
