# MVC Architecture Layers
## Indigenous Language Phrasebook & Translation Portal

**Version:** 1.0  
**Date:** July 2026  
**Project:** Geeked-On Incubation Program – Project 5

---

## 📋 Table of Contents

- [1. Architecture Overview](#1-architecture-overview)
- [2. Layer Definitions](#2-layer-definitions)
  - [2.1 Presentation Layer (Views)](#21-presentation-layer-views)
  - [2.2 Controller Layer](#22-controller-layer)
  - [2.3 Service Layer (Business Logic)](#23-service-layer-business-logic)
  - [2.4 Repository Layer (Data Access)](#24-repository-layer-data-access)
  - [2.5 Model Layer (Domain Models)](#25-model-layer-domain-models)
- [3. Folder Structure](#3-folder-structure)
- [4. Code Examples](#4-code-examples)
  - [4.1 Repository Interface & Implementation](#41-repository-interface--implementation)
  - [4.2 Service Interface & Implementation](#42-service-interface--implementation)
  - [4.3 Controller Example](#43-controller-example)
  - [4.4 Dependency Injection Configuration](#44-dependency-injection-configuration)
- [5. Summary](#5-summary)

---

## 1. Architecture Overview

The application follows a **clean layered architecture** with clear separation of concerns:
![MVC Diagram](../doc/diagrams/MVC.png)
<img src="docs/diagrams/MVC.png" alt="MVC Diagram" width="500"/>
![MVC Diagram](docs/diagrams/MVC.png)


## 2. Layer Definitions

### 2.1 Presentation Layer (Views)

**Location:** `Views/` folder

**Purpose:** Renders the user interface and handles user interaction.

**Components:**

| Component | Description |
|-----------|-------------|
| **Razor Views** | `.cshtml` files with HTML, CSS, and embedded C# |
| **ViewModels** | Data transfer objects specifically for views |
| **Partial Views** | Reusable UI components (e.g., navigation, forms) |
| **Layouts** | Master pages (`_Layout.cshtml`) for consistent design |
| **Static Files** | CSS, JavaScript, images in `wwwroot/` |

**Key View Files:**

```
Views/
├── Home/
│   ├── Index.cshtml          # Landing page with search
│   └── About.cshtml          # About page
├── Phrases/
│   ├── Index.cshtml          # Search results
│   ├── Details.cshtml        # Phrase detail with translations
│   ├── Create.cshtml         # Admin: Add phrase
│   ├── Edit.cshtml           # Admin: Edit phrase
│   └── _PhraseCard.cshtml    # Partial: Phrase card (reusable)
├── Translations/
│   ├── Create.cshtml         # Student: Submit translation
│   └── Approve.cshtml        # Admin: Approval workflow
├── Favourites/
│   └── Index.cshtml          # Student's favourites list
├── Submissions/
│   └── Index.cshtml          # Student's submitted translations
├── Admin/
│   ├── Dashboard.cshtml      # Admin dashboard
│   ├── Pending.cshtml        # Pending submissions
│   ├── ManagePhrases.cshtml  # Phrase management
│   ├── ManageCategories.cshtml # Category management
│   └── Statistics.cshtml     # Usage statistics report
├── Account/
│   ├── Login.cshtml          # Login page
│   ├── Register.cshtml       # Registration page
│   └── Profile.cshtml        # User profile
└── Shared/
    ├── _Layout.cshtml        # Master layout
    ├── _Navbar.cshtml        # Navigation partial
    ├── _Footer.cshtml        # Footer partial
    └── Error.cshtml          # Custom error page
```

---

### 2.2 Controller Layer

**Location:** `Controllers/` folder

**Purpose:** Handles HTTP requests, user authentication, and orchestrates the application flow.

**Responsibilities:**
- Receive HTTP requests (GET, POST, etc.)
- Authenticate and authorize users using `[Authorize]` attributes
- Validate input models using Data Annotations
- Call the Service layer for business logic
- Select and return appropriate Views or JSON responses
- Handle exceptions and return user-friendly error pages

**Controller Files:**

| Controller | Description | Key Actions |
|------------|-------------|-------------|
| **HomeController** | Landing page and general navigation | Index, About |
| **PhrasesController** | Phrase search and viewing | Index (search), Details |
| **TranslationsController** | Translation submission and admin approval | Create, Approve, Reject |
| **FavouritesController** | Favourite management | Add, Remove, Index |
| **SubmissionsController** | Student submission tracking | Index (profile) |
| **AdminController** | Admin dashboard and management | Dashboard, ManagePhrases, ManageCategories, Statistics |
| **AccountController** | Authentication (ASP.NET Identity) | Login, Register, Logout |
| **ProfileController** | User profile management | Index, Update |

**Controller Code Example:**

```csharp
[Authorize]
public class PhrasesController : Controller
{
    private readonly IPhraseService _phraseService;
    private readonly ILogger<PhrasesController> _logger;

    public PhrasesController(IPhraseService phraseService, ILogger<PhrasesController> logger)
    {
        _phraseService = phraseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searchTerm, int? categoryId)
    {
        try
        {
            var results = await _phraseService.SearchAsync(searchTerm, categoryId);
            var viewModel = new PhraseSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Categories = await _phraseService.GetCategoriesAsync(),
                Results = results
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching phrases");
            TempData["ErrorMessage"] = "An error occurred while searching. Please try again.";
            return View(new PhraseSearchViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var phrase = await _phraseService.GetPhraseByIdAsync(id);
        if (phrase == null)
        {
            return NotFound();
        }
        return View(phrase);
    }
}
```

---

### 2.3 Service Layer (Business Logic)

**Location:** `Services/` folder

**Purpose:** Implements the core business logic and workflows of the application.

**Responsibilities:**
- Enforce business rules (e.g., no duplicate favourites, approval workflow)
- Orchestrate multiple repositories (e.g., PhraseRepository + TranslationRepository)
- Apply validation beyond simple data annotations
- Convert between Domain Models and ViewModels/DTOs
- Handle complex operations (e.g., approving a submission creates a Translation)
- Track statistics (log searches and views)

**Service Interfaces & Implementations:**

| Service Interface | Implementation | Purpose |
|-------------------|----------------|---------|
| **IPhraseService** | PhraseService | Search phrases, get phrase details, CRUD operations |
| **ITranslationService** | TranslationService | Submit translations, approve/reject, get translations |
| **IFavouriteService** | FavouriteService | Add/remove favourites, get favourites list |
| **ISubmissionService** | SubmissionService | Manage translation submissions, approval workflow |
| **ICategoryService** | CategoryService | Manage categories (CRUD) |
| **IStatisticsService** | StatisticsService | Track usage, generate reports |
| **IUserService** | UserService | User profile management |

**Service Code Example:**

```csharp
public interface IPhraseService
{
    Task<IEnumerable<PhraseViewModel>> SearchAsync(string searchTerm, int? categoryId);
    Task<PhraseDetailViewModel> GetPhraseByIdAsync(int id);
    Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync();
    Task<Phrase> CreatePhraseAsync(PhraseCreateViewModel model);
    Task<Phrase> UpdatePhraseAsync(PhraseUpdateViewModel model);
    Task<bool> DeactivatePhraseAsync(int id);
    Task<bool> ReactivatePhraseAsync(int id);
    Task<bool> PhraseExistsAsync(string englishText);
}

public class PhraseService : IPhraseService
{
    private readonly IPhraseRepository _phraseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStatisticsService _statisticsService;

    public PhraseService(
        IPhraseRepository phraseRepository,
        ICategoryRepository categoryRepository,
        IStatisticsService statisticsService)
    {
        _phraseRepository = phraseRepository;
        _categoryRepository = categoryRepository;
        _statisticsService = statisticsService;
    }

    public async Task<IEnumerable<PhraseViewModel>> SearchAsync(string searchTerm, int? categoryId)
    {
        // Get phrases from repository
        var phrases = await _phraseRepository.SearchAsync(searchTerm, categoryId);

        // Log search for statistics
        await _statisticsService.LogSearchAsync(searchTerm, categoryId);

        // Map to ViewModels
        return phrases.Select(p => new PhraseViewModel
        {
            PhraseId = p.PhraseId,
            EnglishText = p.EnglishText,
            Category = p.Category?.Name,
            HasTranslation = p.Translations.Any(t => t.Status == "Approved")
        });
    }

    public async Task<PhraseDetailViewModel> GetPhraseByIdAsync(int id)
    {
        var phrase = await _phraseRepository.GetPhraseWithTranslationsAsync(id);
        if (phrase == null)
        {
            return null;
        }

        // Log view for statistics
        await _statisticsService.LogPhraseViewAsync(id);

        return new PhraseDetailViewModel
        {
            PhraseId = phrase.PhraseId,
            EnglishText = phrase.EnglishText,
            Category = phrase.Category?.Name,
            Translations = phrase.Translations
                .Where(t => t.Status == "Approved")
                .Select(t => new TranslationViewModel
                {
                    Language = t.Language,
                    Text = t.Text
                }),
            CanSubmitTranslation = true
        };
    }

    public async Task<Phrase> CreatePhraseAsync(PhraseCreateViewModel model)
    {
        // Validate
        if (await PhraseExistsAsync(model.EnglishText))
        {
            throw new InvalidOperationException("A phrase with this text already exists.");
        }

        var phrase = new Phrase
        {
            EnglishText = model.EnglishText,
            CategoryId = model.CategoryId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        return await _phraseRepository.AddAsync(phrase);
    }

    // ... other methods
}
```

---

### 2.4 Repository Layer (Data Access)

**Location:** `Data/Repositories/` folder

**Purpose:** Handles all database interactions using Entity Framework Core.

**Responsibilities:**
- Encapsulate database queries
- Provide CRUD operations for each entity
- Implement specific queries (search, filtering, etc.)
- Use Entity Framework Core for data access
- Maintain the Unit of Work pattern (DbContext)

**Repository Interfaces & Implementations:**

| Repository Interface | Implementation | Purpose |
|----------------------|----------------|---------|
| **IPhraseRepository** | PhraseRepository | Phrase CRUD and search queries |
| **ITranslationRepository** | TranslationRepository | Translation CRUD and querying |
| **ICategoryRepository** | CategoryRepository | Category CRUD |
| **IFavouriteRepository** | FavouriteRepository | Favourite CRUD and queries |
| **ISubmissionRepository** | SubmissionRepository | Submission CRUD and approval queries |
| **IStatisticsRepository** | StatisticsRepository | Statistics logging and reporting |

**Generic Repository Pattern (Optional):**

```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

**Repository Code Example:**

```csharp
public interface IPhraseRepository
{
    Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId);
    Task<Phrase> GetPhraseWithTranslationsAsync(int id);
    Task<IEnumerable<Phrase>> GetActivePhrasesAsync();
    Task<Phrase> GetByIdAsync(int id);
    Task<Phrase> AddAsync(Phrase phrase);
    Task<Phrase> UpdateAsync(Phrase phrase);
    Task<bool> DeactivateAsync(int id);
    Task<bool> ReactivateAsync(int id);
    Task<bool> ExistsAsync(string englishText);
    Task<int> GetPhraseCountAsync();
}

public class PhraseRepository : IPhraseRepository
{
    private readonly ApplicationDbContext _context;

    public PhraseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId)
    {
        var query = _context.Phrases
            .Include(p => p.Category)
            .Include(p => p.Translations)
            .Where(p => p.IsActive);

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Search by keyword (English text or translations)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.EnglishText.ToLower().Contains(searchTerm) ||
                p.Translations.Any(t =>
                    t.Status == "Approved" &&
                    t.Text.ToLower().Contains(searchTerm)));
        }

        return await query.ToListAsync();
    }

    public async Task<Phrase> GetPhraseWithTranslationsAsync(int id)
    {
        return await _context.Phrases
            .Include(p => p.Category)
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.PhraseId == id);
    }

    public async Task<Phrase> GetByIdAsync(int id)
    {
        return await _context.Phrases
            .FindAsync(id);
    }

    public async Task<Phrase> AddAsync(Phrase phrase)
    {
        await _context.Phrases.AddAsync(phrase);
        await _context.SaveChangesAsync();
        return phrase;
    }

    public async Task<Phrase> UpdateAsync(Phrase phrase)
    {
        phrase.ModifiedDate = DateTime.UtcNow;
        _context.Phrases.Update(phrase);
        await _context.SaveChangesAsync();
        return phrase;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var phrase = await GetByIdAsync(id);
        if (phrase == null) return false;

        phrase.IsActive = false;
        phrase.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateAsync(int id)
    {
        var phrase = await GetByIdAsync(id);
        if (phrase == null) return false;

        phrase.IsActive = true;
        phrase.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string englishText)
    {
        return await _context.Phrases
            .AnyAsync(p => p.EnglishText.ToLower() == englishText.ToLower());
    }

    public async Task<int> GetPhraseCountAsync()
    {
        return await _context.Phrases
            .Where(p => p.IsActive)
            .CountAsync();
    }
}
```

---

### 2.5 Model Layer (Domain Models)

**Location:** `Models/` folder

**Purpose:** Represents the application's data structures and business objects.

**Types of Models:**

| Model Type | Purpose | Examples |
|------------|---------|----------|
| **Domain Models** | Core business entities (database tables) | Category, Phrase, Translation, Favourite, Submission, LanguageStatistic |
| **ViewModels** | Data specifically for views | PhraseSearchViewModel, PhraseDetailViewModel, AdminDashboardViewModel |
| **DTOs** | Data Transfer Objects for APIs/services | TranslationDto, CategoryDto |
| **Request Models** | Form data models | CreatePhraseRequest, SubmitTranslationRequest |

**Domain Models (EF Core Entities):**

```
Models/
├── Domain/
│   ├── ApplicationUser.cs      # Identity user (extended)
│   ├── Category.cs
│   ├── Phrase.cs
│   ├── Translation.cs
│   ├── Favourite.cs
│   ├── Submission.cs
│   └── LanguageStatistic.cs
├── ViewModels/
│   ├── PhraseSearchViewModel.cs
│   ├── PhraseDetailViewModel.cs
│   ├── PhraseCreateViewModel.cs
│   ├── PhraseUpdateViewModel.cs
│   ├── TranslationSubmitViewModel.cs
│   ├── AdminDashboardViewModel.cs
│   ├── PendingSubmissionsViewModel.cs
│   └── StatisticsReportViewModel.cs
└── DTOs/
    ├── PhraseDto.cs
    ├── TranslationDto.cs
    └── CategoryDto.cs
```

---

## 3. Folder Structure

```
📁 Indigenous-Language-Phrasebook/
│
├── 📁 src/
│   ├── 📁 Controllers/
│   │   ├── HomeController.cs
│   │   ├── PhrasesController.cs
│   │   ├── TranslationsController.cs
│   │   ├── FavouritesController.cs
│   │   ├── SubmissionsController.cs
│   │   ├── AdminController.cs
│   │   ├── ProfileController.cs
│   │   └── AccountController.cs
│   │
│   ├── 📁 Views/
│   │   ├── 📁 Home/
│   │   ├── 📁 Phrases/
│   │   ├── 📁 Translations/
│   │   ├── 📁 Favourites/
│   │   ├── 📁 Submissions/
│   │   ├── 📁 Admin/
│   │   ├── 📁 Account/
│   │   └── 📁 Shared/
│   │
│   ├── 📁 Models/
│   │   ├── 📁 Domain/
│   │   ├── 📁 ViewModels/
│   │   └── 📁 DTOs/
│   │
│   ├── 📁 Services/
│   │   ├── 📁 Interfaces/
│   │   │   ├── IPhraseService.cs
│   │   │   ├── ITranslationService.cs
│   │   │   ├── IFavouriteService.cs
│   │   │   ├── ISubmissionService.cs
│   │   │   ├── ICategoryService.cs
│   │   │   ├── IStatisticsService.cs
│   │   │   └── IUserService.cs
│   │   └── 📁 Implementations/
│   │       ├── PhraseService.cs
│   │       ├── TranslationService.cs
│   │       ├── FavouriteService.cs
│   │       ├── SubmissionService.cs
│   │       ├── CategoryService.cs
│   │       ├── StatisticsService.cs
│   │       └── UserService.cs
│   │
│   ├── 📁 Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── SeedData.cs
│   │   └── 📁 Repositories/
│   │       ├── 📁 Interfaces/
│   │       │   ├── IPhraseRepository.cs
│   │       │   ├── ITranslationRepository.cs
│   │       │   ├── ICategoryRepository.cs
│   │       │   ├── IFavouriteRepository.cs
│   │       │   ├── ISubmissionRepository.cs
│   │       │   └── IStatisticsRepository.cs
│   │       └── 📁 Implementations/
│   │           ├── PhraseRepository.cs
│   │           ├── TranslationRepository.cs
│   │           ├── CategoryRepository.cs
│   │           ├── FavouriteRepository.cs
│   │           ├── SubmissionRepository.cs
│   │           └── StatisticsRepository.cs
│   │
│   ├── 📁 Helpers/
│   │   └── StringExtensions.cs
│   │
│   ├── 📁 wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   ├── lib/
│   │   └── images/
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   └── Startup.cs
│
└── 📁 docs/
    └── (Documentation files)
```

---

## 4. Code Examples

### 4.1 Repository Interface & Implementation

**Interface:**

```csharp
public interface ITranslationRepository
{
    Task<Translation> GetByIdAsync(int id);
    Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId);
    Task<IEnumerable<Translation>> GetPendingAsync();
    Task<Translation> AddAsync(Translation translation);
    Task<Translation> UpdateAsync(Translation translation);
    Task<bool> ApproveAsync(int id, string reviewedBy);
    Task<bool> RejectAsync(int id, string reviewedBy, string reason);
    Task<bool> ExistsApprovedAsync(int phraseId, string language);
}
```

**Implementation:**

```csharp
public class TranslationRepository : ITranslationRepository
{
    private readonly ApplicationDbContext _context;

    public TranslationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Translation> GetByIdAsync(int id)
    {
        return await _context.Translations
            .Include(t => t.Phrase)
            .Include(t => t.Submitter)
            .Include(t => t.Reviewer)
            .FirstOrDefaultAsync(t => t.TranslationId == id);
    }

    public async Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId)
    {
        return await _context.Translations
            .Where(t => t.PhraseId == phraseId && t.Status == "Approved")
            .ToListAsync();
    }

    public async Task<IEnumerable<Translation>> GetPendingAsync()
    {
        return await _context.Translations
            .Include(t => t.Phrase)
            .Include(t => t.Submitter)
            .Where(t => t.Status == "Pending")
            .OrderBy(t => t.SubmittedDate)
            .ToListAsync();
    }

    public async Task<Translation> AddAsync(Translation translation)
    {
        await _context.Translations.AddAsync(translation);
        await _context.SaveChangesAsync();
        return translation;
    }

    public async Task<Translation> UpdateAsync(Translation translation)
    {
        _context.Translations.Update(translation);
        await _context.SaveChangesAsync();
        return translation;
    }

    public async Task<bool> ApproveAsync(int id, string reviewedBy)
    {
        var translation = await GetByIdAsync(id);
        if (translation == null) return false;

        translation.Status = "Approved";
        translation.ReviewedBy = reviewedBy;
        translation.ReviewedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(int id, string reviewedBy, string reason)
    {
        var translation = await GetByIdAsync(id);
        if (translation == null) return false;

        translation.Status = "Rejected";
        translation.ReviewedBy = reviewedBy;
        translation.ReviewedDate = DateTime.UtcNow;
        translation.RejectionReason = reason;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsApprovedAsync(int phraseId, string language)
    {
        return await _context.Translations
            .AnyAsync(t => t.PhraseId == phraseId &&
                          t.Language == language &&
                          t.Status == "Approved");
    }
}
```

---

### 4.2 Service Interface & Implementation

**Interface:**

```csharp
public interface ITranslationService
{
    Task<IEnumerable<TranslationViewModel>> GetApprovedForPhraseAsync(int phraseId);
    Task<IEnumerable<TranslationViewModel>> GetPendingAsync();
    Task<bool> SubmitTranslationAsync(TranslationSubmitViewModel model, string userId);
    Task<bool> ApproveTranslationAsync(int translationId, string adminId);
    Task<bool> RejectTranslationAsync(int translationId, string adminId, string reason);
    Task<bool> DeleteTranslationAsync(int translationId);
    Task<int> GetPendingCountAsync();
}
```

**Implementation:**

```csharp
public class TranslationService : ITranslationService
{
    private readonly ITranslationRepository _translationRepository;
    private readonly IPhraseRepository _phraseRepository;
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(
        ITranslationRepository translationRepository,
        IPhraseRepository phraseRepository,
        IStatisticsService statisticsService,
        ILogger<TranslationService> logger)
    {
        _translationRepository = translationRepository;
        _phraseRepository = phraseRepository;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    public async Task<IEnumerable<TranslationViewModel>> GetApprovedForPhraseAsync(int phraseId)
    {
        var translations = await _translationRepository.GetByPhraseIdAsync(phraseId);
        return translations.Select(t => new TranslationViewModel
        {
            TranslationId = t.TranslationId,
            Language = t.Language,
            Text = t.Text,
            Status = t.Status
        });
    }

    public async Task<IEnumerable<TranslationViewModel>> GetPendingAsync()
    {
        var pending = await _translationRepository.GetPendingAsync();
        return pending.Select(t => new TranslationViewModel
        {
            TranslationId = t.TranslationId,
            PhraseId = t.PhraseId,
            PhraseText = t.Phrase?.EnglishText ?? "Unknown Phrase",
            Language = t.Language,
            Text = t.Text,
            Status = t.Status,
            SubmittedBy = t.Submitter?.Email ?? "Unknown User",
            SubmittedDate = t.SubmittedDate
        });
    }

    public async Task<bool> SubmitTranslationAsync(TranslationSubmitViewModel model, string userId)
    {
        try
        {
            // Check if phrase exists
            var phrase = await _phraseRepository.GetByIdAsync(model.PhraseId);
            if (phrase == null)
            {
                _logger.LogWarning("Phrase {PhraseId} not found for translation submission", model.PhraseId);
                return false;
            }

            // Check if an approved translation already exists for this phrase and language
            if (await _translationRepository.ExistsApprovedAsync(model.PhraseId, model.Language))
            {
                _logger.LogWarning("Translation already exists for Phrase {PhraseId} in {Language}", model.PhraseId, model.Language);
                throw new InvalidOperationException($"An approved translation already exists for this phrase in {model.Language}");
            }

            var translation = new Translation
            {
                PhraseId = model.PhraseId,
                Language = model.Language,
                Text = model.Text,
                Status = "Pending",
                SubmittedBy = userId,
                SubmittedDate = DateTime.UtcNow
            };

            await _translationRepository.AddAsync(translation);
            _logger.LogInformation("Translation submitted for Phrase {PhraseId} in {Language}", model.PhraseId, model.Language);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting translation for Phrase {PhraseId}", model.PhraseId);
            throw;
        }
    }

    public async Task<bool> ApproveTranslationAsync(int translationId, string adminId)
    {
        try
        {
            var translation = await _translationRepository.GetByIdAsync(translationId);
            if (translation == null)
            {
                _logger.LogWarning("Translation {TranslationId} not found for approval", translationId);
                return false;
            }

            // Check if already approved
            if (translation.Status == "Approved")
            {
                _logger.LogWarning("Translation {TranslationId} is already approved", translationId);
                return false;
            }

            // Check if an approved translation already exists
            if (await _translationRepository.ExistsApprovedAsync(translation.PhraseId, translation.Language))
            {
                _logger.LogWarning("Cannot approve - already exists for Phrase {PhraseId} in {Language}", 
                    translation.PhraseId, translation.Language);
                throw new InvalidOperationException($"An approved translation already exists for this phrase in {translation.Language}");
            }

            var result = await _translationRepository.ApproveAsync(translationId, adminId);
            if (result)
            {
                _logger.LogInformation("Translation {TranslationId} approved by Admin {AdminId}", translationId, adminId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving translation {TranslationId}", translationId);
            throw;
        }
    }

    public async Task<bool> RejectTranslationAsync(int translationId, string adminId, string reason)
    {
        try
        {
            var translation = await _translationRepository.GetByIdAsync(translationId);
            if (translation == null)
            {
                _logger.LogWarning("Translation {TranslationId} not found for rejection", translationId);
                return false;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Rejection reason is required");
            }

            var result = await _translationRepository.RejectAsync(translationId, adminId, reason);
            if (result)
            {
                _logger.LogInformation("Translation {TranslationId} rejected by Admin {AdminId}", translationId, adminId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting translation {TranslationId}", translationId);
            throw;
        }
    }

    public async Task<bool> DeleteTranslationAsync(int translationId)
    {
        try
        {
            var translation = await _translationRepository.GetByIdAsync(translationId);
            if (translation == null)
            {
                _logger.LogWarning("Translation {TranslationId} not found for deletion", translationId);
                return false;
            }

            _translationRepository.Delete(translation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Translation {TranslationId} deleted", translationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting translation {TranslationId}", translationId);
            throw;
        }
    }

    public async Task<int> GetPendingCountAsync()
    {
        return (await _translationRepository.GetPendingAsync()).Count();
    }
}
```

---

### 4.3 Controller Example

```csharp
[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly ITranslationService _translationService;
    private readonly IPhraseService _phraseService;
    private readonly ICategoryService _categoryService;
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ITranslationService translationService,
        IPhraseService phraseService,
        ICategoryService categoryService,
        IStatisticsService statisticsService,
        ILogger<AdminController> logger)
    {
        _translationService = translationService;
        _phraseService = phraseService;
        _categoryService = categoryService;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var dashboard = new AdminDashboardViewModel
        {
            TotalPhrases = await _phraseService.GetPhraseCountAsync(),
            TotalCategories = await _categoryService.GetCategoryCountAsync(),
            PendingTranslations = await _translationService.GetPendingCountAsync(),
            TotalUsers = await _userService.GetUserCountAsync(),
            RecentActivity = await _statisticsService.GetRecentActivityAsync(10)
        };
        return View(dashboard);
    }

    [HttpGet]
    public async Task<IActionResult> Pending()
    {
        var pending = await _translationService.GetPendingAsync();
        return View(pending);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveTranslation(int translationId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _translationService.ApproveTranslationAsync(translationId, userId);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Translation approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not approve translation. It may have been already processed.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving translation {TranslationId}", translationId);
            TempData["ErrorMessage"] = "An error occurred while approving the translation.";
        }

        return RedirectToAction(nameof(Pending));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectTranslation(int translationId, string rejectionReason)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["ErrorMessage"] = "Rejection reason is required.";
                return RedirectToAction(nameof(Pending));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _translationService.RejectTranslationAsync(translationId, userId, rejectionReason);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Translation rejected successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not reject translation.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting translation {TranslationId}", translationId);
            TempData["ErrorMessage"] = "An error occurred while rejecting the translation.";
        }

        return RedirectToAction(nameof(Pending));
    }

    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var stats = await _statisticsService.GetStatisticsReportAsync();
        return View(stats);
    }
}
```

---

### 4.4 Dependency Injection Configuration

**File:** `Program.cs` or `Startup.cs`

```csharp
// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity services
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register Repositories
builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<ITranslationRepository, TranslationRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();

// Register Services
builder.Services.AddScoped<IPhraseService, PhraseService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ... rest of configuration
```

---

## 5. Summary

### 5.1 Layer Responsibilities Summary

| Layer | Responsibility | Technology |
|-------|---------------|------------|
| **Presentation (Views)** | Render UI, user interaction | Razor, HTML, CSS, Bootstrap 5, JavaScript |
| **Controller** | Handle HTTP requests, orchestrate flow | ASP.NET MVC Controllers |
| **Service** | Business logic, workflows, validation | C# Service Classes |
| **Repository** | Data access, CRUD operations | Entity Framework Core |
| **Model** | Data structures, domain objects | C# Classes with Data Annotations |

### 5.2 Key Principles

1. **Separation of Concerns**: Each layer has a single responsibility
2. **Dependency Inversion**: High-level modules depend on abstractions (interfaces), not concrete implementations
3. **Single Responsibility**: Each class has one reason to change
4. **Open/Closed**: Open for extension, closed for modification
5. **Liskov Substitution**: Derived classes can substitute base classes
6. **Interface Segregation**: Many specific interfaces are better than one general interface
7. **Dependency Injection**: Dependencies are injected via constructors

### 5.3 Files to Create

| File | Path | Purpose |
|------|------|---------|
| Interfaces | `Services/Interfaces/*.cs` | Service layer interfaces |
| Services | `Services/Implementations/*.cs` | Service implementations |
| Repository Interfaces | `Data/Repositories/Interfaces/*.cs` | Repository interfaces |
| Repositories | `Data/Repositories/Implementations/*.cs` | Repository implementations |
| Controllers | `Controllers/*.cs` | MVC controllers |
| ViewModels | `Models/ViewModels/*.cs` | View models |
| Domain Models | `Models/Domain/*.cs` | Domain entities |

---

**Document Status:** ✅ Complete

**Last Updated:** July 2026
---

Would you like me to help you with any other layer or start writing any specific service or repository implementation?
