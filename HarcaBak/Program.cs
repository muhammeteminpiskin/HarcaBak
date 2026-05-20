using HarcaBak.Data;
using HarcaBak.Services;
using HarcaBak.Entities;
using Microsoft.EntityFrameworkCore;
using HarcaBak.DTOs;
using Microsoft.AspNetCore.Identity;

// Helper Method
TransactionListDto ConvertToTransactionListDto(Transaction transaction)
{
    return new TransactionListDto
    {
        Id = transaction.Id,
        Amount = transaction.Amount,
        Description = transaction.Description,
        Date = transaction.Date,
        Type = transaction.Type,
        CategoryId = transaction.CategoryId,
        CategoryName = transaction.Category.Name,
        UserId = transaction.UserId,
        UserName = transaction.User.Name
    };
}

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
var authGroup = app.MapGroup("/api/auth");

// Bütün işlemleri listele
transactionGroup.MapGet("/", (ITransactionService transactionService) =>
{
    var transactions = transactionService.GetAll();
    var results = transactions.Select(transaction => ConvertToTransactionListDto(transaction))
    .ToList();

    return Results.Ok(results);
});

// Bütün kullanıcıları listele (admin paneli için)
userGroup.MapGet("/", (IUserService userService) =>
{
    var users = userService.GetAll();
    var result = users.Select(user => new UserListDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    }).ToList();

    return Results.Ok(result);
});

// Bütün kategorileri listele
categoryGroup.MapGet("/", (ICategoryService categoryService) =>
{
    var categories = categoryService.GetAll();
    var results = categories.Select(category => new CategoryListDto
    {
        Id = category.Id,
        Name = category.Name
    }).ToList();

    return Results.Ok(results);
});

// Belirli bir işlemi getir
transactionGroup.MapGet("/{id}", (ITransactionService transactionService, int id) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction == null)
    {
        return Results.NotFound("İşlem bulunamadı.");
    }
    var result = ConvertToTransactionListDto(transaction);

    return Results.Ok(result);
});

// Belirli bir kategoriyi getir
categoryGroup.MapGet("/{id}", (int id, ICategoryService categoryService) =>
{
    var category = categoryService.GetById(id);
    if (category == null)
    {
        return Results.NotFound("Kategori bulunamadı");
    }
    var result = new CategoryListDto
    {
        Id = category.Id,
        Name = category.Name
    };

    return Results.Ok(result);
});

// Belirli bir kullanıcıyı getir
userGroup.MapGet("/{id}", (int id, IUserService userService) =>
{
    var user = userService.GetById(id);
    if (user == null)
    {
        return Results.NotFound("Eşleşen kullanıcı bulunamadı");
    }
    var result = new UserListDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    };

    return Results.Ok(result);
});

// Belirli bir kategoriye göre işlemleri getir
transactionGroup.MapGet("/filter/category/{categoryId}", (int categoryId, ITransactionService transactionService) =>
{
    var transactions = transactionService.GetByCategoryId(categoryId);
    var result = transactions.Select(transaction => ConvertToTransactionListDto(transaction))
    .ToList();
    return Results.Ok(result);
});

// Belirli bir kullanıcının işlemlerini getir
transactionGroup.MapGet("/filter/user/{userId}", (int userId, ITransactionService transactionService) =>
{
    var transactions = transactionService.GetByUserId(userId);
    var results = transactions
    .Select(transaction => ConvertToTransactionListDto(transaction))
    .ToList();

    return Results.Ok(results);
});
// Belirli bir kullanıcının total hesaplamalarını getir
transactionGroup.MapGet("/summary/user/{userId}", (int userId, ITransactionService transactionService) =>
{
    var result = new TransactionSummaryDto
    {
        UserId = userId,
        TotalExpense = transactionService.GetTotalExpenseByUserId(userId),
        TotalIncome = transactionService.GetTotalIncomeByUserId(userId),
        Balance = transactionService.GetBalanceByUserId(userId)
    };
    return Results.Ok(result);
});

// Bir işlem ekle
transactionGroup.MapPost("/", (TransactionCreateDto dto, ITransactionService transactionService
    , ICategoryService categoryService
    , IUserService userService) =>
{
    if (dto.Amount <= 0)
    {
        return Results.BadRequest("Tutar 0'dan büyük olmalıdır.");
    }
    if (dto.Description != null && dto.Description.Length > 100)
    {
        return Results.BadRequest("Açıklama en fazla 100 karakter olabilir.");
    }
    var user = userService.GetById(dto.UserId);
    if (user == null)
    {
        return Results.BadRequest("Kullanıcı bulunamadı");
    }
    var category = categoryService.GetById(dto.CategoryId);
    if (category == null)
    {
        return Results.BadRequest("Kategori bulunamadı");
    }
    var newTransaction = new Transaction
    {
        Amount = dto.Amount,
        Description = dto.Description,
        Date = dto.Date,
        Type = dto.Type,
        CategoryId = dto.CategoryId,
        UserId = dto.UserId,
        CreatedBy = dto.UserId
    };
    transactionService.Add(newTransaction);
    return Results.Ok("İşlem başarıyla eklendi.");

});

// Bir kategori ekle
categoryGroup.MapPost("/", (CategoryCreateDto dto, ICategoryService categoryService) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("Kategori adı boş bırakılamaz");
    }
    if (dto.CreatedByUserId <= 0)
    {
        return Results.BadRequest("Geçersiz kullanıcı bilgisi");
    }
    var newCategory = new Category()
    {
        Name = dto.Name,
        CreatedBy = dto.CreatedByUserId
    };
    categoryService.Add(newCategory);
    return Results.Ok("Kategori başarıyla eklendi");
});

// Bir kullanıcı ekle
userGroup.MapPost("/", (UserCreateDto dto, IUserService userService) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("İsim boş bırakılamaz");
    }
    if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
    {
        return Results.BadRequest("Geçersiz email adresi.");
    }
    var existingUser = userService.GetByEmail(dto.Email);
    if (existingUser != null)
    {
        return Results.BadRequest("Bu email adresi zaten kullanılıyor");
    }
    if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
    {
        return Results.BadRequest("Şifre en az 6 karakter içermeli");
    }
    var newUser = new User()
    {
        Name = dto.Name,
        Email = dto.Email
    };

    var passwordHasher = new PasswordHasher<User>();
    newUser.PasswordHash = passwordHasher.HashPassword(newUser, dto.Password);

    userService.Add(newUser);
    return Results.Ok("Kullanıcı başarıyla eklendi");
});

// Belirli bir işlemi güncelle
transactionGroup.MapPut("/{id}", (
    int id,
    TransactionUpdateDto dto,
    ITransactionService transactionService,
    ICategoryService categoryService,
    IUserService userService) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction == null)
    {
        return Results.NotFound("İşlem bulunamadı.");
    }
    if (dto.Description != null && dto.Description.Length > 100)
    {
        return Results.BadRequest("Açıklama en fazla 100 karakter olabilir.");
    }
    if (dto.Amount <= 0)
    {
        return Results.BadRequest("Tutar 0'dan büyük olmalıdır.");
    }
    if (dto.UserId <= 0)
    {
        return Results.BadRequest("Geçersiz kullanıcı bilgisi.");
    }

    var user = userService.GetById(dto.UserId);
    if (user == null)
    {
        return Results.BadRequest("Kullanıcı bulunamadı");
    }
    var category = categoryService.GetById(dto.CategoryId);
    if (category == null)
    {
        return Results.BadRequest("Kategori bulunamadı");
    }

    transaction.Amount = dto.Amount;
    transaction.CategoryId = dto.CategoryId;
    transaction.Date = dto.Date;
    transaction.Description = dto.Description;
    transaction.Type = dto.Type;
    transaction.UserId = dto.UserId;
    transaction.UpdatedBy = dto.UserId;

    transactionService.Update(transaction);

    return Results.Ok("İşlem başarıyla güncellendi.");
});

// Belirli bir kategoriyi güncelle
categoryGroup.MapPut("/{id}", (int id, CategoryUpdateDto dto, ICategoryService categoryService) =>
{
    var category = categoryService.GetById(id);
    if (category == null)
    {
        return Results.NotFound("Eşleşen kategori bulunamadı");
    }
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("Kategori adı boş bırakılamaz");
    }
    if (dto.UpdatedByUserId <= 0)
    {
        return Results.BadRequest("Geçersiz kullanıcı bilgisi");
    }

    category.Name = dto.Name;
    category.UpdatedBy = dto.UpdatedByUserId;
    categoryService.Update(category);

    return Results.Ok("Kategori başarıyla güncellendi");
});

// Kullanıcı bilgilerini güncelle
userGroup.MapPut("/{id}", (int id, UserUpdateDto dto, IUserService userService) =>
{
    var user = userService.GetById(id);
    if (user == null)
    {
        return Results.NotFound("Eşleşen kullanıcı bulunamadı");
    }
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("İsim boş bırakılamaz");
    }
    if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
    {
        return Results.BadRequest("Geçersiz mail tanımlaması");
    }

    var existingUserWithEmail = userService.GetByEmail(dto.Email);
    if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
    {
        return Results.BadRequest("Bu email adresi başka bir kullanıcı tarafından kullanılıyor");
    }

    user.Name = dto.Name;
    user.Email = dto.Email;
    user.UpdatedBy = id;
    userService.Update(user);
    return Results.Ok("Bilgileriniz başarıyla güncellendi");
});
// Belirli bir işlemi sil
transactionGroup.MapDelete("/{id}", (int id, ITransactionService transactionService) =>
{
    var transaction = transactionService.GetById(id);
    if (transaction == null)
    {
        return Results.NotFound("İşlem bulunamadı.");
    }
    transactionService.Delete(id);

    return Results.Ok("İşlem başarıyla silindi.");
});

// Belirli bir kategoriyi sil
categoryGroup.MapDelete("/{id}", (int id, ICategoryService categoryService, ITransactionService transactionService) =>
{
    var category = categoryService.GetById(id);
    if (category == null)
    {
        return Results.NotFound("Eşleşen kategori bulunamadı");
    }
    var hasTransactions = transactionService.HasTransactionsByCategoryId(id);
    if (hasTransactions)
    {
        return Results.BadRequest("Bu kategoriye bağlı işlemler olduğu için kategori silinemez");
    }
    categoryService.Delete(id);
    return Results.Ok("Kategori silme işlemi başarılı");
});

// Kullanıcıyı sil
userGroup.MapDelete("/{id}", (int id, IUserService userService, ITransactionService transactionService) =>
{
    var user = userService.GetById(id);
    if (user == null)
    {
        return Results.NotFound("Kullanıcı bulunamadı");
    }
    var hasTransactions = transactionService.HasTransactionsByUserId(id);
    if (hasTransactions)
    {
        return Results.BadRequest("Bu kullanıcıya bağlı işlemler olduğu için kullanıcı silinemez");
    }
    userService.Delete(id);
    return Results.Ok("Kullanıcı silme işlemi başarılı");
});

// Belirli bir tarih aralığındaki işlemleri getir
transactionGroup.MapGet("/filter/date", (DateTime startDate, DateTime endDate, ITransactionService transactionService) =>
{
    var filterTransactions = transactionService.GetByDateRange(startDate, endDate);
    var result = filterTransactions
    .Select(transaction => ConvertToTransactionListDto(transaction))
    .ToList();

    return Results.Ok(result);
});

// Belirli tipteki işlemleri getir (gelir, gider) 
transactionGroup.MapGet("/filter/type", (TransactionType type, ITransactionService transactionService) =>
{
    var filterTransactions = transactionService.GetByType(type);
    var result = filterTransactions
    .Select(transaction => ConvertToTransactionListDto(transaction))
    .ToList();
    return Results.Ok(result);
});
// Kullanıcı girişi
authGroup.MapPost("/login", (LoginDto dto, IUserService userService) =>
{
    if (string.IsNullOrWhiteSpace(dto.Email))
    {
        return Results.BadRequest("Email boş bırakılamaz");
    }
    if (string.IsNullOrWhiteSpace(dto.Password))
    {
        return Results.BadRequest("Şifre boş bırakılamaz");
    }
    var user = userService.GetByEmail(dto.Email);
    if (user == null)
    {
        return Results.BadRequest("Email veya şifre hatalı");
    }

    var passwordHasher = new PasswordHasher<User>();
    var verificationResult = passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        dto.Password);

    if (verificationResult == PasswordVerificationResult.Failed)
    {
        return Results.BadRequest("Email veya şifre hatalı");
    }

    var result = new LoginResponseDto
    {
        UserId = user.Id,
        Name = user.Name,
        Email = user.Email
    };

    return Results.Ok(result);
});

// Kullanıcı şifre değiştirme
authGroup.MapPost("/change-password", (ChangePasswordDto dto, IUserService userService) =>
{
    if (dto.UserId <= 0)
    {
        return Results.BadRequest("Geçersiz kullanıcı bilgisi");
    }

    if (string.IsNullOrWhiteSpace(dto.OldPassword))
    {
        return Results.BadRequest("Eski şifre boş bırakılamaz");
    }

    if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
    {
        return Results.BadRequest("Yeni şifre en az 6 karakter olmalıdır");
    }

    var isChanged = userService.ChangePassword(dto.UserId, dto.OldPassword, dto.NewPassword);

    if (!isChanged)
    {
        return Results.BadRequest("Kullanıcı bulunamadı veya eski şifre hatalı");
    }

    return Results.Ok("Şifre başarıyla değiştirildi");
});

// Kullanıcı çıkışı 
authGroup.MapPost("/logout", () =>
{
    return Results.Ok("Çıkış işlemi başarılı");
});

app.Run();