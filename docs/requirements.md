## Reading Time : 3 Min ;-) 
## 1. User Roles & User Stories

Based on the handbook, there are **two primary user roles** in the system:

| Role | Description |
| :--- | :--- |
| **Student** | A registered user who can search the phrasebook, view translations, submit new translation suggestions, and save favourite phrases. |
| **Administrator** | A privileged user responsible for managing categories/phrases, approving or rejecting student-contributed translations, and viewing usage statistics. |

### User Stories

**Student**
- As a Student, I want to search for phrases by keyword or category so I can quickly find the campus language I need in my preferred language.
- As a Student, I want to submit a translation suggestion for a phrase in my language so I can help other students who speak the same language as me.
- As a Student, I want to save phrases to my favourites list so I can come back to the ones I use most without searching each time.
- As a Student, I want to see my submitted translations and their current approval status so I know whether my contribution has been reviewed.

**Administrator**
- As an Administrator, I want to review all pending translation submissions and approve or reject each one with a reason so only accurate translations are published to students.
- As an Administrator, I want to add new phrases and categories to the phrasebook so the content stays relevant to current campus needs.
- As an Administrator, I want to view a language usage report showing which languages and categories are searched most frequently so I can prioritise which translations to source next.

---

## 2. Functional Requirements

These are the specific features and behaviours the system **must** implement:

**Authentication & Authorization**
- User registration and login using ASP.NET Identity.
- Role-based access control: distinguish between `Student` and `Administrator`.

**Phrase & Category Management**
- Store a multi-language phrase repository covering all **11 official South African languages**.
- Organise phrases into the following fixed categories: *Registration, Accommodation, Health Services, Library, and Academic Support*.
- Each phrase entry stores the original English text and separate translation records per language.
- **Admin only**: Add new phrases, edit existing approved content, deactivate outdated entries, and manage (CRUD) category definitions.

**Search & Discovery**
- Implement phrase search that simultaneously searches across English source text **and** all available translations.
- Allow filtering/searching by category.
- Highlight the matching language within the search results.

**Translation Submission Workflow**
- **Student only**: Submit a suggested translation for a specific phrase in a specific language.
- Submitted translations must be stored with a **Pending** status.
- Translations only appear in public search results after an Administrator approves them.

**Approval Workflow (Admin)**
- Display all pending student translation submissions in a dedicated admin panel.
- Allow Administrator to **Approve**, **Reject with a reason**, or **Request a correction** on each submission.

**Favourites System**
- **Student only**: Save any phrase to a personal favourites list.
- **Student only**: Remove phrases from favourites.
- Provide quick access to the favourites list without searching again.

**Student Profile / Dashboard**
- Display a list of translations the logged-in student has submitted.
- Show the current approval status (Pending, Approved, Rejected) for each submission.

**Usage Statistics & Reporting**
- Automatically track statistics every time a phrase is viewed or searched.
- Record which language was selected and which category the phrase belongs to.
- **Admin only**: View a language usage report showing most searched languages and categories.

---

## 3. Non-Functional Requirements (Quality Attributes)

These define *how* the system should behave and perform:

| Category | Requirement |
| :--- | :--- |
| **Security** | All restricted actions (submitting, favouriting, admin tasks) must be protected at the controller level using `[Authorize(Roles="...")]`. Never rely on hiding UI elements for security. Input validation must be enforced server-side to prevent SQL injection and XSS attacks. |
| **Performance** | Search queries must return results quickly, even as the phrase repository grows. Database indexes should be applied to frequently searched columns (e.g., `EnglishText`, `CategoryId`). |
| **Responsiveness / Usability** | The user interface must be fully responsive using Bootstrap 5 Razor Views, ensuring a seamless experience on mobile phones, tablets, and desktops. Error messages must be user-friendly—no "yellow screen of death." |
| **Reliability / Error Handling** | The system must gracefully handle edge cases: invalid search inputs, attempts to favourite a deleted phrase, or viewing non-existent records. Global exception handling should catch unhandled errors and display a custom error page. |
| **Maintainability** | Code must follow the MVC pattern with clear separation of concerns (Controllers, Services, Repository/Data). Business logic should be encapsulated in service classes. The project must include a professional README and inline comments for complex logic. |
| **Data Integrity** | The approval workflow ensures that only verified, accurate translations are published to the student body. Rejected translations must store a reason for transparency. |
| **Scalability** | (Implied for deployment) The application should be deployable to Azure App Service with Azure SQL Database, allowing it to handle multiple concurrent users during high-traffic periods (e.g., registration season). |