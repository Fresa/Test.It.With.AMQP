using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TextTemplating;

namespace Test.It.With.RabbitMQ.Extensions
{
    public static class TextTransformationExtensions
    {
        private const string TabCharacter = "\t";

        public static void WriteNewLine(this TextTransformation textTransformation, int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                textTransformation.WriteLine(string.Empty);
            }
        }

        public static void Indent(this TextTransformation textTransformation)
        {
            textTransformation.PushIndent(TabCharacter);
        }

        public static void Dedent(this TextTransformation textTransformation)
        {
            textTransformation.PopIndent();
        }

        public static void Tab(this TextTransformation textTransformation, Action action)
        {
            textTransformation.Indent();
            action();
            textTransformation.Dedent();
        }

        public static void Block(this TextTransformation textTransformation, Action action)
        {
            textTransformation.WriteNewLine();
            textTransformation.WriteLine("{");
            textTransformation.Tab(action);
            textTransformation.WriteNewLine();
            textTransformation.Write("}");
        }

        public static void PrintOnNewRowForEach<TValue>(this TextTransformation textTransformation, IEnumerable<TValue> iterator, ActionDelegateWithIndexAndLength<TValue> action)
        {
            var list = iterator.ToList();
            var count = list.Count;
            foreach (var pair in list.WithIndex())
            {
                if (pair.Key > 0)
                {
                    textTransformation.WriteNewLine();
                }

                action(pair.Value, pair.Key, count);
            }
        }

        public static void PrintOnNewRowForEach<TValue>(this TextTransformation textTransformation, IEnumerable<TValue> iterator, Action<TValue> action)
        {
            textTransformation.PrintOnNewRowForEach(iterator, (value, index, length) => action(value));
        }
    }

    public delegate void ActionDelegateWithIndexAndLength<in T>(T value, int index, int length);
}