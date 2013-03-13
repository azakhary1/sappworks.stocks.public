
namespace Stocks.ServiceClients.ETrade
{
    using System;
    using System.Threading;

    public class TokenBucket
    {
        private readonly object _locker = new object();
        private int _tokens;
        private readonly int _capacity;
        private readonly TimeSpan _refillInterval;
        private DateTime _lastGet;
        private bool _bucketWasReset;

        public TokenBucket(int capacity, TimeSpan refillInterval)
        {
            _capacity = capacity;
            _refillInterval = refillInterval;
        }

        public void Consume()
        {
            lock (_locker)
            {
                var bucketUpperBound = _lastGet.Add(_refillInterval);
                var now = DateTime.Now;

                if (bucketUpperBound > now)
                {
                    // we're reusing a bucket
                    _bucketWasReset = false;

                    if (_tokens < _capacity)
                    {
                        // consume one from the current bucket
                        _tokens++;

                        return;
                    }

                    // if we get here the bucket is full. wait for the next bucket
                    Thread.Sleep((int)Math.Max(bucketUpperBound.Subtract(now).TotalMilliseconds, 1d));

                    // try to consume one again
                    Consume();
                }
                else
                {
                    // we're in a new bucket

                    if (!_bucketWasReset)
                    {
                        // no one has set the new bucket

                        // set the new bucket
                        _lastGet = DateTime.Now;

                        // reset the tokens, consuming one
                        _tokens = 1;

                        // indicate we reset the bucket this round
                        _bucketWasReset = true;
                    }
                }
            }
        }
    }
}
