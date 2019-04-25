using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestEFCoreRemoveThenAdd.Models {
    public class Info {
        public string Title { get; set; }

        public static void OnModelCreating<T>(ReferenceOwnershipBuilder<T, Info> rob) where T : class {
            rob.Property(e => e.Title);
        }
    }
}
