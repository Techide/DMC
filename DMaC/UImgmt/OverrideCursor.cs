using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DMaC.UImgmt
{
    public class OverrideCursor : IDisposable
    {

        static Stack<Cursor> _stack = new Stack<Cursor>();

        public OverrideCursor(Cursor changetoCursor)
        {
            _stack.Push(changetoCursor);

            if (changetoCursor != null && Mouse.OverrideCursor != changetoCursor)
            {
                Mouse.OverrideCursor = changetoCursor;
            }
        }

        public void Dispose()
        {
            _stack.Pop();

            var cursor = _stack.Count > 0 ? _stack.Peek() : null;

            if (Mouse.OverrideCursor != null && cursor != Mouse.OverrideCursor)
            {
                Mouse.OverrideCursor = cursor;
            }
        }
    }
}
