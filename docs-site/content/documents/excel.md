# Excel

Generate and read `.xlsx` spreadsheets — backed by [ClosedXML](https://github.com/ClosedXML/ClosedXML)
(MIT), no Excel install required. Inject `IExcelWriter` / `IExcelReader` in handlers.

## Writing (export)

```csharp
public sealed class ExportUsersHandler(IApplicationDbContext context, IExcelWriter excel)
{
    public async Task<byte[]> Handle(ExportUsersQuery query, CancellationToken ct)
    {
        var users = await context.Users.AsNoTracking()
            .Select(u => new { u.NameEn, u.Email, u.IsActive })
            .ToListAsync(ct);

        return excel.Write(users, sheetName: "Users");
    }
}
```

By default one column is created per public property. Control headers, order, and values with
explicit columns:

```csharp
var bytes = excel.Write(users, "Users",
[
    new ExcelColumn<UserDto>("Full name", u => u.NameEn),
    new ExcelColumn<UserDto>("Email", u => u.Email),
    new ExcelColumn<UserDto>("Active", u => u.IsActive),
]);
```

Return the bytes from a controller as a file:

```csharp
return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
```

## Reading (import)

The first row is treated as headers and matched to property names (case-insensitive). Cells are
converted to each property's type; blank/unparseable cells are left at default.

```csharp
public IReadOnlyList<ImportRow> Handle(Stream upload, IExcelReader excel)
    => excel.Read<ImportRow>(upload);
```

## Related

- [PDF](/docs/documents/pdf)
- [File Storage](/docs/storage/overview)
