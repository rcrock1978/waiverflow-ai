using MediatR;

namespace WaiverFlow.Shared.Events;

public abstract record DomainEvent : INotification
{
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid EventId { get; init; } = Guid.NewGuid();
}
