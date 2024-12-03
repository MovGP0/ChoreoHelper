using System.Diagnostics;
using Serilog;

namespace ChoreoHelper;

public sealed class DataBindingTraceListener : TraceListener
{
    private static ILogger Log => Serilog.Log.ForContext<DataBindingTraceListener>();

    public override void Write(string? message)
    {
        Log.Debug("Message: {Message}", message);
    }

    public override void WriteLine(string? message)
    {
        Log.Debug("Message: {Message}", message);
    }

    public override void Fail(string? message)
    {
        Log.Error("Trace Fail: {Message}", message);
    }

    public override void Fail(string? message, string? detailMessage)
    {
        Log.Error("Trace Fail: {Message}. Details: {DetailMessage}", message, detailMessage);
    }

    public override void TraceEvent(
        TraceEventCache? eventCache,
        string source,
        TraceEventType eventType, int id,
        string? message)
    {
        switch (eventType)
        {
            case TraceEventType.Critical:
                Log.Fatal("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
            case TraceEventType.Error:
                Log.Error("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
            case TraceEventType.Warning:
                Log.Warning("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
            case TraceEventType.Information:
                Log.Information("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
            case TraceEventType.Verbose:
                Log.Debug("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
            case TraceEventType.Start:
            case TraceEventType.Stop:
            case TraceEventType.Suspend:
            case TraceEventType.Resume:
            case TraceEventType.Transfer:
            default:
                Log.Debug("[{EventType}] {Source} (ID: {Id}): {Message}", eventType, source, id, message);
                break;
        }
    }

    public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, object? data)
    {
        Log.Information("{Source} (ID: {Id}): {Data}", source, id, data);
    }

    public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, params object?[]? data)
    {
        Log.Information("{Source} (ID: {Id}): {Data}", source, id, data);
    }
}