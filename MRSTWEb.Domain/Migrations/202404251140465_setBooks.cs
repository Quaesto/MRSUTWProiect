namespace MRSTWEb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setBooks : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('To Kill a Mockingbird', 'Harper Lee','/Images/book-1.png',9.99)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Great Gatsby', 'F. Scott Fitzgerald','/Images/book-2.png',12.95)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Catcher in the Rye', 'J.D. Salinger ','/Images/book-3.png',8.50)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('Pride and Prejudice', 'Jane Austen','/Images/book-4.png',7.99)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Hobbit', ' J.R.R. Tolkien','/Images/book-5.png',14.99)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('To Kill a Kingdom', 'Alexandra Christo','/Images/book-6.png',15.00)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Alchemist', 'Paulo Coelho','/Images/book-7.png',10.50)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Girl on the Train', 'Paula Hawkins','/Images/book-8.png',9.95)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Night Circus', ' Erin Morgenstern','/Images/book-9.png',13.99)");
            Sql("INSERT INTO Books (Title,Author,PathImage,Price) VALUES ('The Silent Patient', 'Alex Michaelides ','/Images/book-10.png',16.00)");
        }
        
        public override void Down()
        {
        }
    }
}
