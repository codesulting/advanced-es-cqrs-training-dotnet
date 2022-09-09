using System;
using Scheduling.Infrastructure.MongoDb;

namespace Scheduling.Domain.ReadModel;

public record AvailableSlot(
    string Id,
    string DayId,
    string Date,
    string StartTime,
    TimeSpan Duration,
    bool IsBooked
): IDocument;