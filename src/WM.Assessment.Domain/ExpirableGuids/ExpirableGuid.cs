using System;
using WM.Assessment.Domain.ExpirableGuids.Events;

namespace WM.Assessment.Domain.ExpirableGuids
{
    public class ExpirableGuid : Entity
    {
        //using private setters to avoid values bypassing business rules that will be applied in Domain class methods
        public string Guid { get; private set; }
        public string User { get; private set; }
        public DateTimeOffset Expire { get; private set; }
        public bool IsExpired => Expire.ToUniversalTime() <= DateTimeOffset.UtcNow;

        /// <summary>
        ///     Creates instance of ExpirableGuid with supplied parameters and adds Created domain event
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="user"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public static ExpirableGuid Create(string guid, string user, DateTimeOffset? expire)
        {
            //apply business rules
            //The GUIDs are valid only for a limited period of time, with a default of 30 days from the time of creation
            var expDate = DateTimeOffset.UtcNow.AddDays(30);
            if (expire != null)
                expDate = expire.Value;

            var expGuid = Load(guid, user, expDate);
            expGuid.AddEvent(new ExpirableGuidCreated(expGuid));
            return expGuid;
        }

        public static ExpirableGuid Create(string user, DateTimeOffset? expire)
        {
            var guid = System.Guid.NewGuid().ToString("N").ToUpper();
            return Create(guid, user, expire);
        }

        /// <summary>
        ///     Creates instance of ExpirableGuid with supplied parameters without creating a domain event
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="user"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public static ExpirableGuid Load(string guid, string user, DateTimeOffset expire)
        {
            ValidateGuid(guid);

            var expGuid = new ExpirableGuid
            {
                Guid = guid,
                Expire = expire,
                User = user
            };

            return expGuid;
        }

        private static void ValidateGuid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid) ||
                !System.Guid.TryParseExact(guid, "N", out var sysGuid) ||
                sysGuid.ToString("N").ToUpper() != guid)
                throw new DomainException("Guid is not valid.  Guids are 32 hexadecimal characters, all uppercase.");
        }

        //update
        //only expire and user can be changed
        /// <summary>
        ///     Updates ExpirableGuid values and raises domain event
        /// </summary>
        /// <param name="user"></param>
        /// <param name="expire"></param>
        public void Update(string user, DateTimeOffset expire)
        {
            User = user;
            Expire = expire;

            AddEvent(new ExpirableGuidUpdated(this));
        }

        /// <summary>
        ///     Raises delete domain event
        /// </summary>
        public void Delete()
        {
            AddEvent(new ExpirableGuidDeleted(this));
        }
    }
}