using System.Collections.Concurrent;
using MediatR;

namespace Alexandria.Infrastructure.Tests.TestUtils.Services;

public class MockPublisher : IPublisher
{
    private readonly ConcurrentQueue<object> _notifications = new();

    /// <summary>
    /// Gets all published notifications.
    /// </summary>
    public IEnumerable<object> Notifications => _notifications.ToArray();

    /// <summary>
    /// Publishes a notification and stores it in the internal queue.
    /// </summary>
    /// <param name="notification">The notification object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
    {
        if (notification == null) throw new ArgumentNullException(nameof(notification));

        _notifications.Enqueue(notification);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Publishes a strongly typed notification and stores it in the internal queue.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken())
        where TNotification : INotification
    {
        return Publish((object)notification, cancellationToken);
    }

    /// <summary>
    /// Clears all stored notifications.
    /// </summary>
    public void Clear()
    {
        while (_notifications.TryDequeue(out _)) { }
    }
}