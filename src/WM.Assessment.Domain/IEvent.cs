using System;

namespace WM.Assessment.Domain
{
    public interface IEvent
    {
        /// <summary>
        ///     The id for this event
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///     When the event happened
        /// </summary>
        DateTimeOffset DateOccurred { get; }
    }
}