using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;


namespace ScrumExtreme.Domain.Entities
{
    [CollectionName("Calendar")]
    [BsonIgnoreExtraElements]
    public class CalendarEvent : BaseEntity
    {
        [BsonElement("employeeId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("orderId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? OrderId { get; set; }

        [BsonElement("eventType")]
        [BsonIgnoreIfNull]
        public string? EventType { get; set; }

        [BsonElement("start")]
        public DateTime Start { get; set; }


        [BsonElement("end")]
        public DateTime End { get; set; }


    }
}
