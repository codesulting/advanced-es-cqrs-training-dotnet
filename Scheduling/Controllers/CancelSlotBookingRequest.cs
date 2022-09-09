using System;
using Scheduling.Domain.DoctorDay.Commands;

namespace Scheduling.Controllers;

public record CancelSlotBookingRequest(
    Guid SlotId,
    string Reason,
    string RequestedBy
)
{
    public CancelSlotBooking ToCommand(string dayId) =>
        new CancelSlotBooking(dayId, SlotId, Reason, RequestedBy);
}