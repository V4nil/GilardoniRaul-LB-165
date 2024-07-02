using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDBCrudApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongoCRUD = new MongoCRUD("MMA_Database");

            // Create: Einfügen eines neuen Kämpfers
            var fighter = new Fighter
            {
                Name = "Raaul Gilardoni",
                Nickname = "The Invincible",
                Wins = 20,
                Losses = 0,
                Draws = 0,
                HeightCm = 185.42,
                WeightInKg = 90.72,
                ReachInCm = 200.66,
                Stance = "Southpaw",
                DateOfBirth = new DateTime(1990, 5, 15),
                SignificantStrikesLandedPerMinute = 4.25,
                SignificantStrikingAccuracy = 85.0,
                SignificantStrikesAbsorbedPerMinute = 1.75,
                SignificantStrikeDefence = 90.0,
                AverageTakedownsLandedPer15Minutes = 5.00,
                TakedownAccuracy = 95.0,
                TakedownDefense = 100.0,
                AverageSubmissionsAttemptedPer15Minutes = 3.5
            };
            mongoCRUD.InsertRecord("UFCFighters", fighter);
            Console.WriteLine("Ein neuer Kämpfer wurde eingefügt.");

            // Read: Lesen aller Kämpfer
            var fighters = mongoCRUD.LoadRecords<Fighter>("UFCFighters");
            Console.WriteLine("Liste aller Kämpfer:");
            DisplayFighters(fighters);

            // Update: Aktualisieren eines Kämpfers
            var filter = Builders<Fighter>.Filter.Eq("Name", "Raaul Gilardoni");
            var update = Builders<Fighter>.Update.Set("HeightCm", 200);
            var updateResult = mongoCRUD.UpdateRecord("UFCFighters", filter, update);
            Console.WriteLine($"Update Result: MatchedCount = {updateResult.MatchedCount}, ModifiedCount = {updateResult.ModifiedCount}");

            // Lesen der aktualisierten Kämpfer
            fighters = mongoCRUD.LoadRecords<Fighter>("UFCFighters");
            Console.WriteLine("Liste aller Kämpfer nach der Aktualisierung:");
            DisplayFighters(fighters);

            // Delete: Löschen eines Kämpfers
            var deleteResult = mongoCRUD.DeleteRecord<Fighter>("UFCFighters", fighter.Id);
            Console.WriteLine($"Delete Result: DeletedCount = {deleteResult.DeletedCount}");

            // Lesen der Kämpfer nach dem Löschen
            fighters = mongoCRUD.LoadRecords<Fighter>("UFCFighters");
            Console.WriteLine("Liste aller Kämpfer nach dem Löschen:");
            DisplayFighters(fighters);

            Console.WriteLine("Operationen abgeschlossen.");
        }

        private static void DisplayFighters(IEnumerable<Fighter> fighters)
        {
            foreach (var item in fighters)
            {
                Console.WriteLine($"{item.Id}: {item.Name} - {item.Stance} - {item.HeightCm} cm");
            }
        }
    }

    public class MongoCRUD
    {
        private readonly IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public UpdateResult UpdateRecord<T>(string table, FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            var collection = db.GetCollection<T>(table);
            return collection.UpdateOne(filter, update);
        }

        public DeleteResult DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.DeleteOne(filter);
        }
    }

    public class Fighter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Nickname { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public double HeightCm { get; set; }
        public double WeightInKg { get; set; }
        public double ReachInCm { get; set; }
        public string Stance { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double SignificantStrikesLandedPerMinute { get; set; }
        public double SignificantStrikingAccuracy { get; set; }
        public double SignificantStrikesAbsorbedPerMinute { get; set; }
        public double SignificantStrikeDefence { get; set; }
        public double AverageTakedownsLandedPer15Minutes { get; set; }
        public double TakedownAccuracy { get; set; }
        public double TakedownDefense { get; set; }
        public double AverageSubmissionsAttemptedPer15Minutes { get; set; }
    }
}
