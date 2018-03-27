using System;
using System.Collections.Generic;
using System.Linq;
using Should.Fluent.Model;

namespace Test.It.With.RabbitMQ.Integration.Tests.Assertion
{
    public class ExtendedContain<T> : Contain<T>
    {
        private readonly IShould<IEnumerable<T>> _should;

        public ExtendedContain(IShould<IEnumerable<T>> should) : base(should)
        {
            _should = should;
        }

        public IEnumerable<T> Two(Func<T, bool> predicate)
        {
            return Exactly(2, predicate);
        }

        public IEnumerable<T> Four(Func<T, bool> predicate)
        {
            return Exactly(4, predicate);
        }

        public IEnumerable<T> Exactly(int number, Func<T, bool> predicate)
        {
            return _should.Apply((t, a) =>
            {
                var num = t.Where(predicate).Count();
                if (num == number)
                    return;
                a.Fail($"Expecting {number} matching item in list.  Found {num}.");
            }, (t, a) =>
            {
                var num = t.Where(predicate).Count();
                if (num != number)
                    return;
                a.Fail($"Expecting other than {number} matching item in list.  Found {num}.");
            });
        }
    }
}