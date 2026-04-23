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
        public string UserId { get; set; }

        [BsonElement("orderId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }


        [BsonElement("start")]
        public DateTime Start {  get; set; }


        [BsonElement("end")]
        public DateTime End { get; set; }

     
    }
}
