using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestEFCoreRemoveThenAdd.Models;

namespace TestEFCoreRemoveThenAdd {

    public class TestEFCoreRemoveThenAdd {
        [Test]
        public void RemoveThenAdd() {
            const long MyBookId = 1234;

            Book book1 = new Book {
                BookId = MyBookId,
                Pages = 99,
                EnglishInfo = new Info {
                    Title = "MyBook",
                },
            };
            Book book2 = new Book {
                BookId = MyBookId,
                Pages = 100,
                EnglishInfo = new Info {
                    Title = "MyBook Rev 2",
                },
            };
            Info book2_info = book2.EnglishInfo;

            DbContextOptions<MyContext> options;
            {
                var conn = new SqliteConnection("DataSource=:memory:");
                conn.Open();
                options = new DbContextOptionsBuilder<MyContext>()
                    .UseSqlite(conn)
                    .Options;
                using (var db = new MyContext(options)) {
                    db.Database.Migrate();
                    db.Books.Add(book1);
                    db.SaveChanges();
                }
            }

            using (var db = new MyContext(options)) {
                Book old_book = db.Books.Find(MyBookId);
                Info old_book_info = old_book.EnglishInfo;
                {
                    Assert.That(old_book.Pages, Is.EqualTo(99));
                    Assert.That(old_book.EnglishInfo.Title, Is.EqualTo("MyBook"));
                }
                db.Remove(old_book);

                {
                    Assert.That(book2.Pages, Is.EqualTo(100));
                    Assert.That(book2.EnglishInfo.Title, Is.EqualTo("MyBook Rev 2"));
                    Assert.That(object.ReferenceEquals(book2.EnglishInfo, book2_info), Is.True);
                    Assert.That(object.ReferenceEquals(book2.EnglishInfo, old_book_info), Is.False);
                }
                db.Add(book2); /* THE PROBLEM IS HERE */
                Assert.Multiple(() => {
                    Assert.That(book2.Pages, Is.EqualTo(100)); /* => Succeed */

                    Assert.That(book2.EnglishInfo.Title, Is.EqualTo("MyBook Rev 2")); /* => Fail */
                    // Message: Expected string length 12 but was 6.Strings differ at index 6.
                    //  Expected: "MyBook Rev 2"
                    //  But was:  "MyBook"
                    //  -----------------^

                    Assert.That(object.ReferenceEquals(book2.EnglishInfo, book2_info), Is.True); /* => Fail */
                    // Message: Expected: True
                    //  But was: False

                    Assert.That(object.ReferenceEquals(book2.EnglishInfo, old_book_info), Is.False); /* => Fail */
                    // Message: Expected: False
                    //  But was: True
                });
            }
        }
    }
}
