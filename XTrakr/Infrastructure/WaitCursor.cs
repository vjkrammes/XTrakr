using System;
using System.Windows.Input;

namespace XTrakr.Infrastructure;
public sealed class WaitCursor : IDisposable
{
    private readonly Cursor _cursor;

    public WaitCursor()
    {
        _cursor = Mouse.OverrideCursor;
        Mouse.OverrideCursor = Cursors.Wait;
    }

    public void Dispose() => Mouse.OverrideCursor = _cursor;
}
