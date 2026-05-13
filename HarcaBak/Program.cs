using HarcaBak.Data;
using HarcaBak.Services;
using HarcaBak.Entities;
using Microsoft.EntityFrameworkCore;
using HarcaBak.DTOs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var transactionGroup = app.MapGroup("/api/transactions");
var categoryGroup = app.MapGroup("/api/categories");
var userGroup = app.MapGroup("/api/users");

// Bütün işlemleri listele
transactionGroup.MapGet("/", (ITransactionService transactionService) =>
{
    var transactions = transactionService.GetAll();
    return Results.Ok(transactions);
});

// Bütün kullanıcıları listele (admin paneli için)
userGroup.MapGet("/", (IUserService userService) => {
    var users = userService.GetAll();
    return Results.Ok(users);
});

// Bütün kategorileri listele
categoryGroup.MapGet("/", (ICategoryService categoryService) =>
{
    var categories = categoryService.GetAll();
    return Results.Ok(categories);
});

// Belirli bir işlemi getir
transactionGroup.MapGet("/{id}", (ITransactionService transactionService, int id) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction != null)
    {
        return Results.Ok(transaction);
    }
    else
    {
        return Results.NotFound("İşlem Bulunamadı");
    }
});

// Bir işlem ekle
transactionGroup.MapPost("/", (TransactionCreateDto dto, ITransactionService transactionService) =>
{
    if (dto.Amount <= 0)
    {
        return Results.BadRequest("Tutar değeri 0'dan büyük olmalıdır.");
    }
    var newTransaction = new Transaction
    {
        Amount = dto.Amount,
        Description = dto.Description,
        Date = dto.Date,
        Type = dto.Type,
        CategoryId = dto.CategoryId,
        UserId = dto.UserId
    };
    transactionService.Add(newTransaction);
    return Results.Ok("İşlem başarıyla eklendi.");

});

// Bir kategori ekle
categoryGroup.MapPost("/", (CategoryCreateDto dto, ICategoryService categoryService) =>
{
    var newCategory = new Category()
    {
        Name = dto.Name
    };
    categoryService.Add(newCategory);
    return Results.Ok("Kategori başarıyla eklendi");
});

// Bir kullanıcı ekle
userGroup.MapPost("/", (UserCreateDto dto, IUserService userService) =>
{
    var newUser = new User()
    {
        Name = dto.Name,
        Email = dto.Email,
        Password = dto.Password
    };
    userService.Add(newUser);
    return Results.Ok("Kullanıcı başarıyla eklendi");
});

// Belirli bir işlemi güncelle
transactionGroup.MapPut("/{id}", (int id, TransactionUpdateDto dto, ITransactionService transactionService) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction == null)
    {
        return Results.NotFound("Eşleşen işlem bulunamadı");
    }
    else if (dto.Amount <= 0)
    {
        return Results.BadRequest("Tutar değeri sıfır veya daha küçük olamaz");
    }
    else
    {
        transaction.Amount = dto.Amount;
        transaction.CategoryId = dto.CategoryId;
        transaction.Date = dto.Date;
        transaction.Description = dto.Description;
        transaction.Type = dto.Type;
    }
    transactionService.Update(transaction);
    return Results.Ok("İşlem başarıyla güncelleştirildi");
});

// Belirli bir kategoriyi güncelle
categoryGroup.MapPut("/{id}", (int id, CategoryUpdateDto dto, ICategoryService categoryService) =>
{
    var category = categoryService.GetById(id);
    if (category == null)
    {
        return Results.NotFound("Eşleşen kategori bulunamadı");
    }
    else
    {
        category.Name = dto.Name;
        categoryService.Update(category);
    }
    return Results.Ok("Kategori başarıyla güncellendi");
});

// Kullanıcı bilgilerini güncelle (kullanıcı kendi bilgilerini güncellemek isteyebilir.)
userGroup.MapPut("/{id}", (int id, UserUpdateDto dto, IUserService userService) =>
{
    var user = userService.GetById(id);
    if (user == null)
    {
        return Results.NotFound("Eşleşen kullanıcı bulunamadı");
    }
    else
    {
        user.Name = dto.Name;
        user.Password = dto.Password;
        user.Email = dto.Email;
    }
    userService.Update(user);
    return Results.Ok("Bilgileriniz başarıyla güncellendi");
});
// Belirli bir işlemi sil
transactionGroup.MapDelete("/{id}", (int id, ITransactionService transactionService) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction == null)
    {
        return Results.NotFound("Eşleşen kayıt bulunamadı");
    }
    transactionService.Delete(id);

    return Results.Ok("Silme işlemi başarılı");
});

// Belirli bir kategoriyi sil (veritabanı hata verebilir)
categoryGroup.MapDelete("/{id}", (int id, ICategoryService categoryService) =>
{
    var category = categoryService.GetById(id);
    if (category == null)
    {
        return Results.NotFound("Eşleşen kategori bulunamadı");
    }
    categoryService.Delete(id);
    return Results.Ok("Kategori silme işlemi başarılı");
});

// Kullanıcıyı sil (veritabanı hata verebilir)
userGroup.MapDelete("/{id}", (int id, IUserService userService) =>
{
    var user = userService.GetById(id);
    if(user == null)
    {
        return Results.NotFound("Kullanıcı bulunamadı");
    }
    userService.Delete(id);
    return Results.Ok("Kullanıcı silme işlemi başarılı");
});

// Belirli bir tarih aralığındaki işlemleri getir
transactionGroup.MapGet("/filter/date", (DateTime startDate, DateTime endDate, ITransactionService transactionService) =>
{
    var filterTransactions = transactionService.GetByDateRange(startDate, endDate);
    return Results.Ok(filterTransactions);
});

// Belirli tipteki işlemleri getir (gelir, gider) 
transactionGroup.MapGet("/filter/type", (TransactionType type, ITransactionService transactionService) =>
{
    var filterTransactions = transactionService.GetByType(type);
    return Results.Ok(filterTransactions);
});

app.Run();