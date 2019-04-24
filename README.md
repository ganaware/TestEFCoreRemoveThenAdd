# Owned entities resurrect when an entity is added with the same key of an deleted entity.

## Step to reproduce

### 1. define a model(Book) with an owned entity

```csharp
public class Book {
    public long BookId { get; set; }
    public int Pages { get; set; }
    public Info EnglishInfo { get; set; }
    public static void OnModelCreatingelBuilder modelBuilder) {
        var e_tb = modelBuilder.Entity<Book>();
        e_tb.Property(e => e.BookId);
        e_tb.Property(e => e.Pages);
        e_tb.HasKey(e => e.BookId);
        e_tb.OwnsOne(e => e.EnglishInfo);
    }
}

public class Info {
    public string Title { get; set; }
    public static void OnModelCreatingelBuilder modelBuilder) {
        EntityTypeBuilder<Info> e_tb = lBuilder.Entity<Info>();
        e_tb.Property(e => e.Title);
    }
}

public class MyContext : DbContext {
    public DbSet<Book> Books { get; set; }
    protected override void OnModelCreatingelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        Book.OnModelCreating(modelBuilder);
        Info.OnModelCreating(modelBuilder);
    }
}
```

### 2. Save an entity

```csharp
const long MyBookId = 1234;
Book book1 = new Book {
    BookId = MyBookId,
    Pages = 99,
    EnglishInfo = new Info {
        Title = "MyBook",
    },
};
using (var db = new MyContext()) {
    db.Books.Add(book1);
    db.SaveChanges();
}
```

### 3. Load it, remove it, and add a new entity with the same key

```csharp
Book book2 = new Book {
    BookId = MyBookId,
    Pages = 100,
    EnglishInfo = new Info {
        Title = "MyBook Rev 2",
    },
};
Info book2_info = book2.EnglishInfo;
using (var db = new MyContext(options)) {
    Book old_book = db.Books.Find(MyBookId); // load it
    db.Remove(old_book); // remove it
    db.Add(book2); // add a new entity with the same key
}
```

The problem occurs at <code>db.Add(book2)</code>:

- Before it, <code>book2.EnglishInfo</code> is <code>book2_info</code>.
- After it, <code>book2.EnglishInfo</code> is <code>old_book.EnglishInfo</code>.  A owned entity value of the deleted entity has resurrected!  Why?
    - I think book2.EnglishInfo should be book2_info.

## Complete reprodusable project

1. Clone: https://github.com/ganaware/TestEFCoreRemoveThenAdd
2. Run: <code>dotnet test</code>

## Further technical details

- EF Core version: 2.1.4, 2.2.4
- Database Provider: Microsoft.EntityFrameworkCore.Sqlite 2.2.4,  Pomelo.EntityFrameworkCore.MySql 2.1.4
- Operationg system: Windows 7, 10
- IDE: Visual Studio 2017 15.8.7
