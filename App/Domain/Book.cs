namespace App.Domain
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public User Author { get; set; }

        public int AuthorId { get; set; }   
    }
}