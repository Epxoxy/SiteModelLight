using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiteModelLight.Helpers
{
    public enum HandMode
    {
        Break,
        Continue
    }

    public class LoopingException : Exception
    {
        public HandMode HandMode { get; set; }
        public Exception Exception { get; set; }

        public LoopingException(Exception e)
        {
            this.Exception = e;
        }
    }

    public class LoopingInvoker : IDisposable
    {
        public Action Action { get; private set; }
        public Action Invoked { get; set; }
        public Action Started { get; set; }
        public Action Stopped { get; set; }

        public int Interval { get; private set; }
        public int RepeatTimes { get; private set; }
        public int Remain { get; private set; }
        
        public bool IsRunning { get; private set; }

        private CancellationTokenSource cancelSource;
        private CancellationToken cancelToken;

        public LoopingInvoker(Action action,int repeat, int interval)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (repeat < 0)
                throw new ArgumentException("Repeat times cannot less than zero");
            if(interval < 0)
                throw new ArgumentException("Interval cannot less than zero");
            this.Action = action;
            this.RepeatTimes = repeat;
            this.Interval = interval;
        }

        public LoopingInvoker Start(Action<LoopingException> onException)
        {
            if (!IsRunning && Action != null)
            {
                IsRunning = true;
                cancelSource = new CancellationTokenSource();
                cancelToken = cancelSource.Token;
                Remain = RepeatTimes + 1;
                Task.Run(async () =>
                {
                    Started?.Invoke();
                    while (IsRunning)
                    {
                        //Invoke main action
                        try
                        {
                            Action.Invoke();
                        }
                        catch (Exception e)
                        {
                            var ex = new LoopingException(e);
                            onException.Invoke(ex);

                            if(ex.HandMode == HandMode.Break)
                                break;
                            else if(ex.HandMode == HandMode.Continue)
                            {
                                --Remain;
                                IsRunning = Remain > 0;
                                continue;
                            }
                        }

                        //Check if continue
                        --Remain;
                        IsRunning = Remain > 0;

                        //After main action invoked
                        Action invokedCache = Invoked;
                        //Donot insert await so main task can run next after interval
                        if (invokedCache != null)
                        {
                            invokeAction(invokedCache, debugException);
                        }

                        //Delay for continue running
                        if (IsRunning)
                            await Task.Delay(Interval, cancelToken);
                    }
                    IsRunning = false;
                    Remain = 0;
                    Stopped?.Invoke();
                }, cancelToken).ContinueWith(task =>
                {
                    if(task.Status != TaskStatus.Canceled)
                    {
                        if(task.Exception != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Error" + task.Exception.Message);
                        }
                    }
                });
            }
            return this;
        }

        public void Stop()
        {
            Remain = 0;
            IsRunning = false;
            if (cancelSource != null)
            {
                if (cancelToken.CanBeCanceled)
                    cancelSource.Cancel();
                cancelSource.Dispose();
                cancelSource = null;
            }
        }

        private void invokeAction(Action action, Action<Exception> onError)
        {
            Task.Run(action).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    onError.Invoke(task.Exception);
                }
            });
        }

        private void debugException(Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Exception when invoke 'Invoked'" + e.Message);
        }

        public void Dispose()
        {
            if (IsRunning)
            {
                Stop();
            }
            else if (cancelSource != null)
            {
                cancelSource.Dispose();
            }
            Action = null;
            Invoked = null;
            Started = null;
            Stopped = null;
        }
    }
}
