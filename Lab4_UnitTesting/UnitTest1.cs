using Lab4;
using Npgsql;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Lab4_UnitTesting
{
    public class Tests
    {

        IBusinessLogic BL;
        public String connectionString;
        public ObservableCollection<Entry> entries;


        [SetUp]
        public void Setup()
        {

            BL = new BusinessLogic();
         

        }

        //Tests for Add method

        [Test]
        public void AddTypicalTest()
        {
           
            Assert.That(BL.AddEntry("How do celebrities stay cool", "Fans", 1, "19\\10\\2022"), Is.EqualTo(Lab4.InvalidFieldError.NoError));
        }
        
        
        [Test]
        public void AddInvalidClueLengthTest()
        {
           Assert.That(BL.AddEntry("", "Fans", 1, "19\\10\\2022"), Is.EqualTo(Lab4.InvalidFieldError.InvalidClueLength));
        }

        

        [Test]
        public void AddInvalidAnswerLengthTest()
        {
            Assert.That(BL.AddEntry("How do celebrities stay cool", "The Fans of the celebrities who dont want them to get too hot", 1, "19\\10\\2022"), Is.EqualTo(Lab4.InvalidFieldError.InvalidAnswerLength));
        }
        


        [Test]
        public void AddInvalidDifficultyTest()
        {
         Assert.That(BL.AddEntry("How do celebrities stay cool", "Fans", 5, "19\\10\\2022"), Is.EqualTo(Lab4.InvalidFieldError.InvalidDifficulty));
        }
     
        

        //Tests for Deletion
        
        [Test]
        public void DeleteTypicalTest()
        {
            BL.AddEntry("How do celebrities stay cool", "Fans", 1, "19\\10\\2022");
            Assert.That(BL.DeleteEntry(BL.GetEntries().Last().Id), Is.EqualTo(EntryDeletionError.NoError));
        }

        [Test]

        public void DeleteInvalidIdTest()
        {
            Assert.That(BL.DeleteEntry(0),Is.EqualTo(EntryDeletionError.EntryNotFound));
        }
        
        

        // Update Test

        [Test]

        public void UpdateTypicalTest()
        {
            BL.AddEntry("Test", "Test", 1, "19\\10\\2022");

            EntryEditError editError = BL.EditEntry("deer with no eyes", "no eye deer", 1, "20\\1\\2022", BL.GetEntries().Last().Id);
              Assert.That(editError,Is.EqualTo(EntryEditError.NoError));
        }

        [Test]

        public void UpdateInvalidClueTest()
        {

            BL.AddEntry("Test", "Test", 1, "19\\10\\2022");

            EntryEditError editError = BL.EditEntry("", "no eye deer", 1, "20\\1\\2022", BL.GetEntries().Last().Id);
            Assert.That(editError, Is.EqualTo(EntryEditError.InvalidFieldError));
        }

        [Test]
        public void UpdateInvalidAnswerTest()
        {
            BL.AddEntry("Test", "Test", 1, "19\\10\\2022");

            EntryEditError editError = BL.EditEntry("deer with no eyes", "", 1, "20\\1\\2022", BL.GetEntries().Last().Id);
            Assert.That(editError, Is.EqualTo(EntryEditError.InvalidFieldError));
        }

        [Test]

        public void UpdateInvalidDifficultyTest()
        {
            EntryEditError editError = BL.EditEntry("deer with no eyes","no eye deer",5,"20\\1\\2022",1);
            Assert.That(editError, Is.EqualTo(EntryEditError.InvalidFieldError));
        }

        [Test]
        public void UpdateInvalidIDTest()
        {
            EntryEditError editError =  BL.EditEntry("deer with no eyes", "no eye deer", 1, "20\\1\\2022", 0);
            Assert.That(editError, Is.EqualTo(EntryEditError.EntryNotFound));
        }


        [Test]
        public void RetreiveAllEntries()
        {
            connectionString = InitializeConnectionString();

            entries = new ObservableCollection<Entry>();

            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * FROM \"entries\";";
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            // Columns are clue, answer, difficulty, date, id in that order ...
            // Show all data
            while (reader.Read())
            { 
                for (int colNum = 0; colNum < reader.FieldCount; colNum++)
                {
                    Console.Write(reader.GetName(colNum) + "=" + reader[colNum] + " ");
                }
                Console.Write("\n");
                entries.Add(new Entry(reader[0] as String, reader[1] as String, (int)reader[2], reader[3] as String, (int)reader[4]));
            }
            con.Close();

            ObservableCollection<Entry> test = BL.GetEntries(); // Stores entries from the GetEntries method to the Test variable


            Assert.That(entries.Count, Is.EqualTo(test.Count));

        }

        public String InitializeConnectionString()
        {
            var bitHost = "db.bit.io";
            var bitApiKey = "v2_3ufw7_DrRrZnS29BzRiHA6LWhgZa2"; // from the "Password" field of the "Connect" menu

            var bitUser = "ShabbarKaz";
            var bitDbName = "ShabbarKaz/Lab3_ShabbarKazmi";

            return connectionString = $"Host={bitHost};Username={bitUser};Password={bitApiKey};Database={bitDbName}";

        }
    }
}