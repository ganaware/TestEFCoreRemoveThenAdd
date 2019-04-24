using Microsoft.EntityFrameworkCore;

namespace TestEFCoreRemoveThenAdd.Models {
    public class Book {
        public long BookId { get; set; }
        public int Pages { get; set; }
        public Info EnglishInfo { get; set; }
        public static void OnModelCreating(ModelBuilder modelBuilder) {
            var e_tb = modelBuilder.Entity<Book>();
            e_tb.Property(e => e.BookId);
            e_tb.Property(e => e.Pages);
            e_tb.HasKey(e => e.BookId);
            e_tb.OwnsOne(e => e.EnglishInfo);
        }
    }
}
