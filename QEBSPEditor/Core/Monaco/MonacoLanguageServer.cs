using QEBSPEditor.Components;
using System.Diagnostics.CodeAnalysis;

namespace QEBSPEditor.Core.Monaco;

public abstract class MonacoLanguageServer : IAsyncDisposable
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

    [MemberNotNullWhen(true, nameof(_cts))]
    [MemberNotNullWhen(true, nameof(_task))]
    [MemberNotNullWhen(true, nameof(_editor))]
    private bool IsRunning => _task is not null;

    private Task? _task;
    private CancellationTokenSource? _cts;
    private MonacoEditor? _editor;

    protected List<Marker> _markers = new List<Marker>();
    public IReadOnlyList<Marker> Markers => _markers;



    public async ValueTask DisposeAsync()
    {
        if (IsRunning && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            await _task;
        }
    }

    public void Start(MonacoEditor editor)
    {
        _editor = editor;

        // Start task
        _cts = new CancellationTokenSource();
        _task = TaskAsync(_cts.Token);
    }

    private async Task TaskAsync(CancellationToken cancellationToken)
    {
        await OnStartAsync(cancellationToken);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_editor!.Code is not null)
            {
                await OnParseAsync(_editor.Code, cancellationToken);
                await _editor.SetMarkersAsync(_markers, cancellationToken);
            }

            await Task.Delay(Delay, cancellationToken);
        }
    }

    protected abstract Task OnStartAsync(CancellationToken cancellationToken);
    protected abstract Task OnParseAsync(string code, CancellationToken cancellationToken);
}
