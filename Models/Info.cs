using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestEFCoreRemoveThenAdd.Models {
    public class Info {
        public string Title { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder) {
            EntityTypeBuilder<Info> e_tb = modelBuilder.Entity<Info>();
            e_tb.Property(e => e.Title);
        }
    }
}
